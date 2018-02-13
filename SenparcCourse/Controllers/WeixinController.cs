using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MvcExtension;
using SenparcCourse.Service;
using System;
using System.IO;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SenparcCourse.Controllers
{
    public class WeixinController : Controller
    {
        public static readonly string Token = WebConfigurationManager.AppSettings["WeixinToken"];//与微信公众账号后台的Token设置保持一致，区分大小写。
        //public static readonly string EncodingAESKey = WebConfigurationManager.AppSettings["WeixinEncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = WebConfigurationManager.AppSettings["WeixinAppId"];//与微信公众账号后台的AppId设置保持一致，区分大小写。

        readonly Func<string> _getRandomFileName = () => DateTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6);

        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://sdk.weixin.senparc.com/weixin
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        /// <summary>
        /// 处理微信服务器转发过来的消息
        /// </summary>
        /// <param name="postModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel)
        {
            //消息加密
            //postModel.EncodingAESKey = EncodingAESKey;

            //创建MessageHandler消息处理实例，StorageModel 有效时间 10分钟  
            var messageHanlder = new CustomMessageHandler(Request.InputStream, postModel, 10);

            //消息去重,默认True
            messageHanlder.OmitRepeatedMessage = true;

            #region 日志记录：请求消息

            var logPath = Server.MapPath($"~/App_Data/MP/{DateTime.Now:yyyy-MM-dd}/");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            messageHanlder.RequestDocument.Save(Path.Combine(logPath,
                $"{_getRandomFileName()}_Request_{messageHanlder.RequestMessage.FromUserName}_{messageHanlder.RequestMessage.MsgType}.txt"));

            if (messageHanlder.UsingEcryptMessage)
            {
                messageHanlder.EcryptRequestDocument.Save(Path.Combine(logPath,
                    $"{_getRandomFileName()}_Request_Ecrypt_{messageHanlder.RequestMessage.FromUserName}_{messageHanlder.RequestMessage.MsgType}.txt"));
            }
            #endregion

            //执行消息处理
            messageHanlder.Execute();


            #region 日志记录：响应消息

            if (messageHanlder.ResponseDocument != null)
            {
                messageHanlder.ResponseDocument.Save(Path.Combine(logPath,
                    $"{_getRandomFileName()}_Response_{messageHanlder.ResponseMessage.FromUserName}_{messageHanlder.ResponseMessage.MsgType}.txt"));
            }

            if (messageHanlder.UsingEcryptMessage && messageHanlder.FinalResponseDocument != null)
            {
                messageHanlder.FinalResponseDocument.Save($"{_getRandomFileName()}_Response_Final_{messageHanlder.ResponseMessage.FromUserName}{messageHanlder.ResponseMessage.MsgType}.txt");
            }

            #endregion

            //返回消息
            //return Content(messageHanlder.FinalResponseDocument.ToString());
            return new FixWeixinBugWeixinResult(messageHanlder);
        }
    }
}