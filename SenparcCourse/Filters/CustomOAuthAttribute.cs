using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.MvcExtension;

namespace SenparcCourse.Filters
{
    public class CustomOAuthAttribute : SenparcOAuthAttribute
    {
        public CustomOAuthAttribute(string appId, string oauthCallbackUrl, OAuthScope oauthScope = OAuthScope.snsapi_userinfo) : base(appId, oauthCallbackUrl, oauthScope)
        {
            base._appId = base._appId ?? Service.Config.AppId;
        }

        public override bool IsLogined(HttpContextBase httpContext)
        {
            bool isLogined = httpContext.User.Identity.IsAuthenticated;
            return isLogined;
        }
    }
}