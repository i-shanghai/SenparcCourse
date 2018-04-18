using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

    }
}