﻿using Senparc.Weixin.Entities.Request;
using Senparc.Weixin.Helpers.Extensions;
using Senparc.Weixin.MP.AppStore;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Xml.Linq;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Containers;
using SenparcCourse.Service.Dtos;

namespace SenparcCourse.Service
{
    public class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        private DateTime StartTime;
        private DateTime EndTime;

        public CustomMessageHandler(Stream inputStream, PostModel postModel = null, int maxRecordCount = 0, DeveloperInfo developerInfo = null) : base(inputStream, postModel, maxRecordCount, developerInfo)
        {
        }

        public CustomMessageHandler(XDocument requestDocument, PostModel postModel = null, int maxRecordCount = 0, DeveloperInfo developerInfo = null) : base(requestDocument, postModel, maxRecordCount, developerInfo)
        {
        }

        public CustomMessageHandler(RequestMessageBase requestMessageBase, PostModel postModel = null, int maxRecordCount = 0, DeveloperInfo developerInfo = null) : base(requestMessageBase, postModel, maxRecordCount, developerInfo)
        {
        }

        public override XDocument Init(XDocument postDataDocument, object postData = null)
        {
            StartTime = DateTime.Now;

            return base.Init(postDataDocument, postData);
        }

        /// <summary>
        /// 关注后，欢迎消息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            //获取关注人员信息  
            // var accessToken = AccessTokenContainer.GetAccessToken(Config.AppId);
            var userInfo = Senparc.Weixin.MP.AdvancedAPIs.UserApi.Info(Config.AppId, base.WeixinOpenId);
            var nickName = userInfo.nickname;
            var title = userInfo.sex == 1 ? "先生" : (userInfo.sex == 0 ? "女士" : "");

            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "欢迎{0}{1} 关注!".FormatWith(nickName, title);

            return responseMessage;

            //System.Threading.Thread.Sleep(1000);

            //CustomApi.SendText(Config.AppId, base.WeixinOpenId, "客服消息，马上回复.进入<a href=\"https://www.baidu.com\">进入百度</a>");
            //return new ResponseMessageNoResponse(); //不返回任何消息
        }

        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            if (requestMessage.EventKey == "KONG")
            {
                //return null;
                return new ResponseMessageNoResponse(); //不返回任何消息
            }
            else if (requestMessage.EventKey == "NEWS")
            {
                //通过客服消息接口返回一条消息
                //  CustomApi.SendTextAsync(Config.AppId, base.WeixinOpenId, "客服消息，马上回复.进入<a href=\"https://www.baidu.com\">进入百度</a>");

                var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageNews>();
                var articleNew = new Article()
                {
                    Title = "图文消息",
                    PicUrl = "http://sdk.weixin.senparc.com/images/book-cover-front-small-3d-transparent.png",
                    Url = "http://www.baidu.com",
                    Description = "这是一篇文章\r\n换行了\r\n哈哈",
                };
                responseMessage.Articles.Add(articleNew);
                return responseMessage;
            }
            else
            {
                var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = "您点击了按钮：" + requestMessage.EventKey;

                //根据是否进入 CMD状态，返回信息
                var storageModel = CurrentMessageContext.StorageData as StorageModel;
                if (storageModel != null)
                {
                    if (storageModel.IsInCmd)
                    {
                        //storageModel.CmdCount += 1;
                        responseMessage.Content = responseMessage.Content + "\r\n进入CMD状态.";//+ storageModel.CmdCount.ToString()
                        responseMessage.Content += "\r\n上一条请求消息的类型:" + CurrentMessageContext.RequestMessages.Last().MsgType;//上一条请求消息的类型
                    }
                    else
                    {
                        responseMessage.Content = responseMessage.Content + "\r\n退出CMD状态";
                    }
                }

                return responseMessage;
            }
        }


        /// <summary> 
        /// 返回地理位置
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您发送的是：Lat-{0},Lon-{1}".FormatWith(requestMessage.Location_X.ToString(CultureInfo.CurrentCulture), requestMessage.Location_Y.ToString(CultureInfo.CurrentCulture));
            return responseMessage;
        }


        /// <summary>
        /// 根据用户发送的消息，返回消息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            //IResponseMessageBase responseMessage = null;

            ////输入cmd 进入cmd状态 ， exit退出cmd状态 
            //if (requestMessage.Content == "cmd")
            //{
            //    CurrentMessageContext.StorageData = new StorageModel()
            //    {
            //        IsInCmd = true
            //    };
            //}
            //if (requestMessage.Content == "exit")
            //{
            //    var storageModel = CurrentMessageContext.StorageData as StorageModel;
            //    if (storageModel != null)
            //    {
            //        storageModel.IsInCmd = false;
            //    }
            //}

            //根据关键字、正则表达式，返回不同的消息内容
            RequestMessageTextKeywordHandler handler = requestMessage.StartHandler()
                .Keyword("cmd", () =>
                {
                    var responseMessageText = requestMessage.CreateResponseMessage<ResponseMessageText>();

                    CurrentMessageContext.StorageData = new StorageModel()
                    {
                        IsInCmd = true
                    };
                    responseMessageText.Content += "\r\n您已进入cmd模式";

                    return responseMessageText;

                })
                .Keyword("电子卡", () =>
                {
                    CustomApi.SendCard(Config.AppId, WeixinOpenId, "pifDGvpJKLpOc2vQxZHPslkxItqw", null, 10000);
                    return new ResponseMessageNoResponse();
                })
                .Keywords(new[] { "exit", "close", "quit" }, () =>
                 {
                     var responseMessageText = requestMessage.CreateResponseMessage<ResponseMessageText>();

                     CurrentMessageContext.StorageData = new StorageModel()
                     {
                         IsInCmd = false
                     };
                     responseMessageText.Content += "\r\n您已退出cmd模式";

                     return responseMessageText;

                 }).Regex(@"天气 \S+", () =>
                {
                    //Get Json Result 获取天气信息。 正则表达式 使用
                    var city = Regex.Match(requestMessage.Content, @"(?<=天气 )(\S+)").Value;
                    var url = "https://www.sojson.com/open/api/weather/json.shtml?city={0}".FormatWith(city.UrlEncode());

                    //直接返回Json结果
                    var weatherResult = Get.GetJson<WeatherResult>(url, null, null);

                    var responseMessageText = requestMessage.CreateResponseMessage<ResponseMessageText>();
                    responseMessageText.Content = @"天气查询结果
==========
城市：{0}的天气信息
时期：{1}
结果：{2}
时间：{3}".FormatWith(weatherResult.city, weatherResult.date,
                        weatherResult.message, weatherResult.AddTime);

                    return responseMessageText;

                }).Regex(@"^http", () =>
                 {
                     var responseMessageNews = requestMessage.CreateResponseMessage<ResponseMessageNews>();
                     var articleNew = new Article()
                     {
                         Title = "您输入了" + requestMessage.Content,
                         PicUrl = "http://sdk.weixin.senparc.com/images/book-cover-front-small-3d-transparent.png",
                         Url = "http://www.baidu.com",
                         Description = "这是一篇文章\r\n换行了\r\n哈哈",
                     };
                     responseMessageNews.Articles.Add(articleNew);

                     return responseMessageNews;
                 }).Default(() =>
                {
                    var responseMessageText = requestMessage.CreateResponseMessage<ResponseMessageText>();
                    responseMessageText.Content = "这是一条默认的消息回复";
                    return responseMessageText;
                });

            //如果输入的是文字，再回复中添加输入的文字 
            var responseMessage = handler.ResponseMessage;
            if (responseMessage is ResponseMessageText)
            {
                var storageModel = CurrentMessageContext.StorageData as StorageModel;
                if (storageModel != null)
                {
                    requestMessage.Content += "\r\nCount:" + storageModel.CmdCount.ToString();
                }

                (responseMessage as ResponseMessageText).Content += "\r\n您输入了：" + requestMessage.Content;
            }

            //返回内容转为基类
            return responseMessage as IResponseMessageBase;
        }


        /// <summary>
        /// 文本消息或者点击事件消息，会 覆盖单独的OnTextRequest
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnTextOrEventRequest(RequestMessageText requestMessage)
        {
            if (requestMessage.Content == "123")
            {
                //创建回复消息对象
                var responseMessageText = requestMessage.CreateResponseMessage<ResponseMessageText>();
                responseMessageText.Content = "进入OnTextOrEventRequest，并返回消息.\r\n您输入的是" + requestMessage.Content;
                return responseMessageText;
            }

            return base.OnTextOrEventRequest(requestMessage);
        }

        /// <summary>
        /// 处理请求消息之前,统一处理事件
        /// 此处的业务逻辑处理：根据用户的WeixinOpenId提示，提醒用户操作
        /// 处理敏感词
        /// </summary>
        public override void OnExecuting()
        {
            if (CurrentMessageContext.StorageData is StorageModel storageModel && storageModel.IsInCmd)
            {
                storageModel.CmdCount++;

                //接收到第5条消息的时候 提示重复
                if (storageModel.CmdCount >= 5)
                {
                    //使用全局的RequestMessage 和 ResponseMessage
                    var responseMessageText = RequestMessage.CreateResponseMessage<ResponseMessageText>();
                    responseMessageText.Content = WeixinOpenId + " 您已经发送超过5条消息，请您10分钟以后再试";
                    ResponseMessage = responseMessageText;

                    //取消后续的关键字匹配执行
                    CancelExcute = true;
                }
            }
            //继续执行
            base.OnExecuting();
        }

        /// <summary>
        /// 业务上适用于添加消息签名，或者消息过滤处理
        /// </summary>
        public override void OnExecuted()
        {
            if (ResponseMessage is ResponseMessageText)
            {
                ((ResponseMessageText)ResponseMessage).Content += "\r\n【消息签名】";

                //开队列或者线程，处理数据库相关问题 

                //Thread.Sleep(5000);//这样超出5s没有返回消息给微信Server。

                //有可能超时的时候，借用客服消息返回用户信息
                EndTime = DateTime.Now;
                int runTime = (EndTime - StartTime).Seconds;
                if (runTime > 4)
                {
                    //使用客服消息返回信息
                    ////CustomApi.SendText(Config.AppId, base.WeixinOpenId,
                    ////   ((ResponseMessageText)ResponseMessage).Content + "\r\n" + "【客服消息】");

                    //ResponseMessage = new ResponseMessageNoResponse();

                    MessageQueueHandler messageQueueHandler = new MessageQueueHandler();
                    messageQueueHandler.SendKfMessage(base.WeixinOpenId, ResponseMessage);
                }

            }
            base.OnExecuted();
        }


        public override IResponseMessageBase OnEvent_TemplateSendJobFinishRequest(RequestMessageEvent_TemplateSendJobFinish requestMessage)
        {
            string strAdminOpenId = "oifDGvmdSfltOJPL2QSuCdEIN0io";
            if (requestMessage.Status != "success")
            {
                //发送失败，进行处理 
                Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(Config.AppId, strAdminOpenId, "消息发送失败，MsgID:" + requestMessage.MsgID);
            }
            else
            {
                //再用一条客服消息通知用户模板消息已经发送完成
                Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(Config.AppId, strAdminOpenId, "消息发送成功，MsgID:" + requestMessage.MsgID);

            }

            return new SuccessResponseMessage();
        }
         


        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "当前服务器时间：" + DateTime.Now;
            return responseMessage;
        }
    }
}
