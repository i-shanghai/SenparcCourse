using System;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using SenparcCourse.Service;

namespace SenparcCourse.Controllers
{
    public class OAuthController : Controller
    {
        // GET
        public ActionResult Index()
        {
            //var callbackUrl = "http://physicalchina.club/Oauth/CallBack?returnUrl=" +
            //                  "http://physicalchina.club".UrlEncode() + "&msg=Index";
            //var state = "WXOauthState." + Guid.NewGuid().ToString("n");
            //Session["OAuthState"] = state;


            ////OAuth URL 
            //var oauthOfficalUrl = OAuthApi.GetAuthorizeUrl(Config.AppId, callbackUrl, state, Senparc.Weixin.MP.OAuthScope.snsapi_userinfo, "code");

            //定义 model 为 ：oauthOfficalUrl
            //return View("Index", model: oauthOfficalUrl);


            //如果当前用户没有登陆，则自动跳转到Oauth认证页面
            //带上当前页面的URL 作为 redirectUrl的参数
            //这样就只可以在通过微信的OAuth2认证以后再跳转回来

            string strCurrentUrl = Request.Url.AbsolutePath.UrlEncode();//"http://physicalchina.club/Oauth/Index".UrlEncode();
            string strOauthUrl = "Login?redirectUrl=" + strCurrentUrl + "&msg=Index";
            ViewData["LoginUrl"] = strOauthUrl;

            ViewData["IsLogined"] = User.Identity.IsAuthenticated;
            ViewData["UserLoginId"] = User.Identity.Name;

            return View("Index");
        }

        /// <summary>
        /// 实现登陆
        /// </summary>
        public ActionResult Login(string redirectUrl, string msg)
        {
            var callbackUrl = "http://physicalchina.club/Oauth/CallBack?redirectUrl=" + redirectUrl + "&Msg=" + msg;
            var state = "WXOauthState." + Guid.NewGuid().ToString("n");
            Session["OAuthState"] = state;

            //OAuth URL 
            var oauthOfficalUrl = OAuthApi.GetAuthorizeUrl(Config.AppId, callbackUrl, state, Senparc.Weixin.MP.OAuthScope.snsapi_userinfo, "code");

            //直接跳转到认证登陆页面,OAuth认证完毕以后 ，跳转到 callbackUrl
            return Redirect(oauthOfficalUrl);
        }

        /// <summary>
        /// 从官方授权页面接受回调信息，并根据code进一步获取用户基本信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="returnUrl">, string returnUrl</param>
        /// <returns></returns>
        public ActionResult CallBack(string code, string state, string redirectUrl, string msg)
        {
            //没有Code
            if (string.IsNullOrEmpty(code))
            {
                return Content("用户拒绝授权");
            }

            if (state == null || state != (Session["OAuthState"] as string))
            {
                return Content("未授权用户，请重新登陆");
            }

            OAuthAccessTokenResult oauthResult = null;
            try
            {
                oauthResult = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(Config.AppId, Config.AppSecret, code);
            }
            catch (ErrorJsonResultException e)
            {
                Console.WriteLine(e);
                throw;
            }

            var oauthAccessToken = oauthResult.access_token;
            var openId = oauthResult.openid;

            //关注以后，才能获取到
            //var userInfo = UserApi.Info(Config.AppId, openId);

            //根据授权以后的AccessToken和OpendId来获取UserInfo
            OAuthUserInfo userInfo = OAuthApi.GetUserInfo(oauthAccessToken, openId);

            //微信服务器的OAuth2认证已经完成，可以进行其他的业务逻辑了，如认证登陆
            //openId 作为Cookie返回
            //Response.Cookies.Add(new HttpCookie("loginid",userInfo.openid));

            //OAuth2成功以后，借助ASP.NET Form实现登陆功能
            //可以通过：User.Identity.IsAuthenticated , User.Identity.Name获取登陆信息
            System.Web.Security.FormsAuthentication.SetAuthCookie(userInfo.openid, false);

            ViewData["Msg"] = msg;

            if (string.IsNullOrEmpty(redirectUrl) == false)
            {
                //再次跳转到 OAuth2认证前的页面
                return Redirect(redirectUrl);
            }
            else
            {
                return View(userInfo);
            }
        }


        /// <summary>
        /// 从官方授权页面接受回调信息，并根据code进一步获取用户基本信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="returnUrl">, string returnUrl</param>
        /// <returns></returns>
        public ActionResult CallBackNew(string code, string state, string redirectUrl, string msg)
        {
            //没有Code
            if (string.IsNullOrEmpty(code))
            {
                return Content("用户拒绝授权");
            }
             
            OAuthAccessTokenResult oauthResult = null;
            try
            {
                oauthResult = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(Config.AppId, Config.AppSecret, code);
            }
            catch (ErrorJsonResultException e)
            {
                Console.WriteLine(e);
                throw;
            }

            var oauthAccessToken = oauthResult.access_token;
            var openId = oauthResult.openid;

            //关注以后，才能获取到
            //var userInfo = UserApi.Info(Config.AppId, openId);

            //根据授权以后的AccessToken和OpendId来获取UserInfo
            OAuthUserInfo userInfo = OAuthApi.GetUserInfo(oauthAccessToken, openId);

            //微信服务器的OAuth2认证已经完成，可以进行其他的业务逻辑了，如认证登陆
            //openId 作为Cookie返回
            //Response.Cookies.Add(new HttpCookie("loginid",userInfo.openid));

            //OAuth2成功以后，借助ASP.NET Form实现登陆功能
            //可以通过：User.Identity.IsAuthenticated , User.Identity.Name获取登陆信息
            System.Web.Security.FormsAuthentication.SetAuthCookie(userInfo.openid, false);
             
            ViewData["Msg"] = msg;
            ViewData["Code"] = code;

            ViewData["isLogined"] = User.Identity.IsAuthenticated;

            if (string.IsNullOrEmpty(redirectUrl) == false)
            {
                //再次跳转到 OAuth2认证前的页面
                return Redirect(redirectUrl);
            }
            else
            {
                return View(userInfo);
            }
        }

        /// <summary>
        /// 退出登陆以后，跳转到Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("Index", "OAuth");
        }

    }
}