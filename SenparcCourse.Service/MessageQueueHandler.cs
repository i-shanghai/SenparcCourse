using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senparc.Weixin.MessageQueue;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;

namespace SenparcCourse.Service
{
    public class MessageQueueHandler
    {
        /// <summary>
        /// 客服消息返回用户信息
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public IResponseMessageBase SendKfMessage(string openId, IResponseMessageBase responseMessage)
        {
            var messageQueue = new SenparcMessageQueue();

            if (responseMessage is ResponseMessageText)
            {
                {
                    var myKey = SenparcMessageQueue.GenerateKey("MessageHandlerSendMessageAsync", responseMessage.GetType(),
                        Guid.NewGuid().ToString(), "SendMessage");

                    messageQueue.Add(myKey, () =>
                    {

                        var kfResponseMessage = responseMessage as ResponseMessageText;
                        kfResponseMessage.Content += "\r\n【客服消息队列-1】";

                        //在队列中发送消息
                        CustomApi.SendText(Config.AppId, openId, kfResponseMessage.Content);
                    });
                }
                {
                    var myKey = SenparcMessageQueue.GenerateKey("MessageHandlerSendMessageAsync", responseMessage.GetType(),
                        Guid.NewGuid().ToString(), "SendMessage");

                    messageQueue.Add(myKey, () =>
                    {

                        var kfResponseMessage = responseMessage as ResponseMessageText;
                        kfResponseMessage.Content += "\r\n【客服消息队列-2】";

                        //在队列中发送消息
                        CustomApi.SendText(Config.AppId, openId, kfResponseMessage.Content);
                    });
                }
            }

            return new ResponseMessageNoResponse();
        }
    }
}
