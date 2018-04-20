using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SenparcCourse.Filters;

namespace SenparcCourse.Controllers
{
    /// <summary>
    /// 定义OAuthBaseController 借助SenparcOAuthAttribute实现 登陆认证功能
    /// </summary>
    [CustomOAuth(null, "/OAuth/CallBackNew", Senparc.Weixin.MP.OAuthScope.snsapi_base)]
    public class OAuthBaseController : Controller
    {
         
    }
}