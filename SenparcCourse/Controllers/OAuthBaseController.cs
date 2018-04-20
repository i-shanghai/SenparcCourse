using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SenparcCourse.Filters;
using SenparcCourse.Models.ViewData;

namespace SenparcCourse.Controllers
{
    /// <summary>
    /// 定义OAuthBaseController 借助SenparcOAuthAttribute实现 登陆认证功能
    /// 认证完成以后 ，给ViewModel赋值
    /// </summary>
    [CustomOAuth(null, "/OAuth/CallBackNew", Senparc.Weixin.MP.OAuthScope.snsapi_base)]
    public class OAuthBaseController : Controller
    {
        //BaseController执行完毕以后，给UserName赋值
        public string UserName { get; set; }
         
        /// <summary>
        /// 给UserName 赋值
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;
            }
        }

        /// <summary>
        /// 构建Model给View使用
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            if (ViewData.Model is OAuthBaseVD)
            {
                var model = ViewData.Model as OAuthBaseVD;
                model.UserName = UserName;
                model.PageRenderTime = DateTime.Now;
            }
        }
    }
}