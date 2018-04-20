using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP.Helpers;
using SenparcCourse.Models.ViewData;

namespace SenparcCourse.Controllers
{
    public class JSSDKController : OAuthBaseController
    { 
        /// <summary>
        /// 放到OnActionExecuting中执行
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //检查ViewData.Model 是否是 Base_JSSDK ，如果是，获取jssdkUiPackage数据给到model
            if (ViewData.Model is Base_JSSDKVD)
            {
                var model = ViewData.Model as Base_JSSDKVD;

                ViewData["AbsolutePath"] = Request.Url.AbsolutePath;
                ViewData["AbsoluteUri"] = Request.Url.AbsoluteUri;

                var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(Service.Config.AppId, Service.Config.AppSecret, Request.Url.AbsolutePath);

                model.JsSdkUiPackage = jssdkUiPackage;
            }

            base.OnActionExecuted(filterContext);
        }

        // GET: JSSDK
        public ActionResult Index()
        {
            var vd = new JSSDK_Index
            {  
                UserName = "TEST"
            };

            return View(vd);
        }
    }
}