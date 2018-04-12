using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenparcCourse.Service
{
    //把Template独立出来一个类,并在构造函数中进行初始化
    //一个AppId可以有多个Template模板
    public class TempaleteMessageBag
    {

        public string TemplateName { get; set; }
        public string AppId { get; set; }

        public string MessageId { get; set; }

        public string MessageNumber { get; set; }

        public string SubscribeMsgTemplateId { get; set; }//一次性订阅模板

        public TempaleteMessageBag(string appId, string tempalteName, string messageId, string messageNumber, string subscribeMsgTemplateId)
        {
            AppId = appId;
            TemplateName = tempalteName;
            MessageId = messageId;
            MessageNumber = messageNumber;
            SubscribeMsgTemplateId = subscribeMsgTemplateId;
        }

    }


    public static class Config
    {
        public static string AppId = System.Configuration.ConfigurationManager.AppSettings["WeixinAppId"];

        public static  string AppSecret =  System.Configuration.ConfigurationManager.AppSettings["WeixinAppSecret"];

        public static int LogRecordCount = 0;

        public static string TemplateNumber = "";

        public static string TemplateId = "0KX8hioVH-OWJgIJusS9Ju5XkIS4rWphKXcDHY5R9Z4";

        public static string SubscribeMsgTemplateId = ""; //一次性订阅模板

        //一个AppId下面有多个模板消息，以不同的 TemplateName进行区分
        public static Dictionary<string, List<TempaleteMessageBag>> TempaleteMessageCollection;

        static Config()
        {
            TempaleteMessageCollection = new Dictionary<string, List<TempaleteMessageBag>>
            {
                [AppId] = new List<TempaleteMessageBag>()
                {
                    new TempaleteMessageBag(AppId,"课程进度通知",TemplateId,TemplateNumber,SubscribeMsgTemplateId)
                }
            };

            //TempaleteMessageCollection[AppId].FirstOrDefault(z => z.TemplateName == "课程提醒");
        }

        /// <summary>
        /// 根据appId 和 templateName 获取Tempalte基本信息，用于发送模板消息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="tempalteName"></param>
        /// <returns></returns>
        public static TempaleteMessageBag GetTempaleteMessageBag(string appId, string tempalteName)
        {
            //先检查AppId下是否有Template信息bingbing
            if (!TempaleteMessageCollection.ContainsKey(appId))
            {
                return null;
            }

            TempaleteMessageBag tmpBag = TempaleteMessageCollection[appId].FirstOrDefault(a => a.TemplateName == tempalteName);
            return tmpBag;
        }
    }
}
