using Senparc.Weixin.Entities.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;

namespace SenparcCourse.Service.TemplateMessage
{
    /// <summary>
    /// 发送课程通知的模板消息 
    /// </summary>
    public class TemplateMessageCourseNotice:TemplateMessageBase
    {
        //定义消息发送的内容习性

        public TemplateDataItem first { get; set; }

        public TemplateDataItem keyword1 { get; set; }

        public TemplateDataItem keyword2 { get; set; }

        public TemplateDataItem keyword3 { get; set; }

        public TemplateDataItem remark { get; set; }

        /// <summary>
        /// 构造函数中，直接给基类设置固定的参数（课程消息的TemplateId和TemplateName）
        /// 然后从外部接收：TemplateContnet的内容，并把参数名称设置为容易理解的名称替代：keyword
        /// </summary>
        /// <param name="strUrl">模板消息详情跳转的URL</param>
        /// <param name="first"></param>
        /// <param name="courseName"></param>
        /// <param name="courseStatus"></param>
        /// <param name="learningProgress"></param>
        /// <param name="remark"></param>
        public TemplateMessageCourseNotice( string strUrl,string strFirst,string courseName,string courseStatus,string learningProgress,string strRemark,string templateId= "0KX8hioVH-OWJgIJusS9Ju5XkIS4rWphKXcDHY5R9Z4") : base(templateId, strUrl,
            "课程进度通知")
        {
            first = new TemplateDataItem(strFirst);
            keyword1 = new TemplateDataItem(courseName);
            keyword2 = new TemplateDataItem(courseStatus);
            keyword3 = new TemplateDataItem(learningProgress);
            remark = new TemplateDataItem(strRemark);
        }
    }
}