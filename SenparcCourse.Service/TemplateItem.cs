using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;

namespace SenparcCourse.Service
{
    /// <summary>
    /// 模板内容属性
    /// </summary>
    public class TemplateItem 
    {
        public TemplateDataItem first { get; set; } 
        public TemplateDataItem keyword1 { get; set; }
        public TemplateDataItem keyword2 { get; set; }
        public TemplateDataItem keyword3 { get; set; }
        public TemplateDataItem remark { get; set; } 
    }

    //public class TemplateItemData
    //{
    //    public string value { get; set; }

    //    public string color { get; set; }
      
    //    //默认颜色 
    //    public TemplateItemData()
    //    {
    //        color = "#cccccc";
    //    }
    //}
}