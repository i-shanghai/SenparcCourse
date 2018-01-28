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

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        { 
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "当前服务器时间：" + DateTime.Now;
            return responseMessage;
        }
    }
}
