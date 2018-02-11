using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Helpers;
using System.IO;
using System.Xml.Linq;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.AppStore;
using Senparc.Weixin.Helpers.Extensions;

namespace SenparcCourse.Service
{
    public class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        public CustomMessageHandler(Stream inputStream, PostModel postModel = null, int maxRecordCount = 0, DeveloperInfo developerInfo = null) : base(inputStream, postModel, maxRecordCount, developerInfo)
        {
            base.CurrentMessageContext.ExpireMinutes = 10;
        }

        public CustomMessageHandler(XDocument requestDocument, PostModel postModel = null, int maxRecordCount = 0, DeveloperInfo developerInfo = null) : base(requestDocument, postModel, maxRecordCount, developerInfo)
        {
        }

        public CustomMessageHandler(RequestMessageBase requestMessageBase, PostModel postModel = null, int maxRecordCount = 0, DeveloperInfo developerInfo = null) : base(requestMessageBase, postModel, maxRecordCount, developerInfo)
        {
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

                var storageModel = CurrentMessageContext.StorageData as StorageModel;
                if (storageModel != null)
                {
                    if (storageModel.IsInCmd)
                    {
                        storageModel.CmdCount += 1;
                        responseMessage.Content = responseMessage.Content + "\r\n进入CMD状态:"+ storageModel.CmdCount.ToString();
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
            responseMessage.Content = "您发送的是：Lat-{0},Lon-{1}".FormatWith(requestMessage.Location_X.ToString(), requestMessage.Location_Y.ToString());
            return responseMessage;
        }


        /// <summary>
        /// 返回用户发送的消息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您发送的是：" + requestMessage.Content.ToString();

            //输入cmd 进入cmd状态 ， exit退出cmd状态 
            if (requestMessage.Content == "cmd")
            {
                CurrentMessageContext.StorageData = new StorageModel()
                {
                    IsInCmd = true
                };
            }
            if (requestMessage.Content == "exit")
            {
                var storageModel = CurrentMessageContext.StorageData as StorageModel;
                if (storageModel != null)
                {
                    storageModel.IsInCmd = false;
                }
            }

            return responseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "当前服务器时间：" + DateTime.Now;
            return responseMessage;
        }
    }
}
