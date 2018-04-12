using System;
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
            var callbackUrl = "http://physicalchina.club/Oauth/CallBack";//?returnUrl=" + "http://physicalchina.club".UrlEncode()
            var state = "WXOauthState." + Guid.NewGuid().ToString("n");
            Session["OAuthState"] = state;

            //OAuth URL 
            var oauthOfficalUrl = OAuthApi.GetAuthorizeUrl(Config.AppId, callbackUrl, state, Senparc.Weixin.MP.OAuthScope.snsapi_userinfo, "code");

            return View("Index",model: oauthOfficalUrl);
        }

        /// <summary>
        /// 从官方授权页面接受回调信息，并根据code进一步获取用户基本信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="returnUrl">, string returnUrl</param>
        /// <returns></returns>
        public ActionResult CallBack(string code, string state)
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

            return View(userInfo);
        }
    }
}