using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Senparc.Weixin.Cache;
using Senparc.Weixin.Cache.Redis;

namespace SenparcCourse
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RegisterWeixinCache(); //注册分布式缓存（按需，如果需要，必须放在第一个）
            RegisterThreads();  //激活微信缓存及队列线程（必须）,要在RegisterWeixin之前
            RegisterWeixin();  //注册Demo所用微信公众号的账号信息（按需）
        }

        /// <summary>
        /// 自定义缓存策
        /// </summary>
        private void RegisterWeixinCache()
        {
            //不同的网站 不同的命名空间
            Senparc.Weixin.Config.DefaultCacheNamespace = "PhysicalChinaWeixinCache_FW";

            #region Redis配置

            var redisConfiguration = System.Configuration.ConfigurationManager.AppSettings["Cache_Redis_Configuration"];
            RedisManager.ConfigurationOption = redisConfiguration;

            if (!string.IsNullOrEmpty(redisConfiguration) && redisConfiguration != "Redis配置")
            {
                //启用Redis缓存 配置
                CacheStrategyFactory.RegisterObjectCacheStrategy(() => RedisObjectCacheStrategy.Instance);
            }

            #endregion
        }

        /// <summary>
        /// 注册线程
        /// </summary>
        private void RegisterThreads()
        {
            Senparc.Weixin.Threads.ThreadUtility.Register();
        }

        private void RegisterWeixin()
        {
            var appId = System.Configuration.ConfigurationManager.AppSettings["WeixinAppId"];
            var appSecret = System.Configuration.ConfigurationManager.AppSettings["WeixinAppSecret"];

            Senparc.Weixin.MP.Containers.AccessTokenContainer.Register(appId, appSecret, "公众号-测试号-AccessToken");

            //Senparc.Weixin.MP.Containers.JsApiTicketContainer.Register(appId, appSecret, "公众号-测试号-JsApiTicket"); 
        }
    }
}
