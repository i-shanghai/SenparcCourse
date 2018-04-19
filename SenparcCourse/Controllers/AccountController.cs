using Senparc.Weixin.MP.MvcExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP;
using SenparcCourse.Filters;
using SenparcCourse.Service;

namespace SenparcCourse.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            //如果没有Form认证，则直接跳转到认证页面
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "OAuth", new { redirectUrl = Request.Url.PathAndQuery });
            }

            ViewData["Msg"] = "您已经登陆，用户名："+User.Identity.Name;

            return View();
        }

        public ActionResult Index2()
        { 
            //如果没有Form认证，则直接跳转到认证页面
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "OAuth", new { redirectUrl = Request.Url.PathAndQuery });
            }
            ViewData["Msg"] = "您已经登陆，用户名：" + User.Identity.Name;

            return View();
        }

        /// <summary>
        /// 借助SenparcOAuthAttribute实现 登陆认证功能
        /// "/OAuth/Login?redirectUrl='http://physicalchina.club/OAuth/Index'&msg=index3"
        /// </summary>
        /// <returns></returns>
        [CustomOAuth(null, "/OAuth/CallBackNew", Senparc.Weixin.MP.OAuthScope.snsapi_base)]
        public ActionResult Index3()
        { 
            ViewData["Msg"] = "您已经登陆，用户名：" + User.Identity.Name;
            return View();
        } 
    }
}