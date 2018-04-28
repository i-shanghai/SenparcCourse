using System;
using Senparc.Weixin.Cache;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.Helpers.Extensions;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Containers;
using SenparcCourse.Service;

namespace SenparcCourse.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        private static int _count = 0;

        public static int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        /// <summary>
        /// 读取出_count ,休1s, +1 返回结果
        /// </summary>
        /// <returns></returns>
        public ActionResult LockTest()
        {
            //Lock 实现 独占、锁住；解决数据冲突的问题
            var strategy = CacheStrategyFactory.GetObjectCacheStrategyInstance();

            //Lock 可以存放在本地，也可以存放在缓存中
            using (strategy.BeginCacheLock("SenparcCourse", "LockTest"))
            {
                var count = Count; //读取 Count 

                Thread.Sleep(100); //休0.1s , 模拟并发场景，所有线程都会在这里停留0.1秒

                Count = count + 1; //设置 Count

                return Content(Count.ToString());
            }
        }

        /// <summary>
        /// 实现发送客服消息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public ActionResult CustomeMesssage(string openId = "oifDGvmdSfltOJPL2QSuCdEIN0io")
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var result = CustomApi.SendText(Config.AppId, openId, $"{(i + 1)}这是一条客服消息。" + DateTime.Now.ToString("HH:mm:ss.ffff"));

                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    throw;
                }
            }

            var result2 = CustomApi.SendText(Config.AppId, openId, "这是一条客服消息。" + DateTime.Now.ToString("HH:mm:ss.ffff"));
            return Content(result2.ToJson());
        }

        /// <summary>
        /// 异步消息的发送
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<ActionResult> CustomeMesssageAsync(string openId = "oifDGvmdSfltOJPL2QSuCdEIN0io")
        {
            return await Task.Factory.StartNew(async () =>
            {
                for (int i = 0; i < 4; i++)
                {
                    await CustomApi.SendTextAsync(Config.AppId, openId, "这是第{0}条客服消息,时间:{1}".FormatWith(i + 1, DateTime.Now.ToString("HH:mm:ss.ffff")));
                }
                return Content("异步消息发送完毕");
            }).Result;
        }

        /// <summary>
        /// 刷新AccessToken
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<ActionResult> ChangeAccessToken(string openId = "oifDGvmdSfltOJPL2QSuCdEIN0io")
        {
            return await Task.Factory.StartNew(async () =>
            {
                for (int i = 0; i < 4; i++)
                {
                    await CustomApi.SendTextAsync(Config.AppId, openId, "这是第{0}条客服消息,时间:{1}".FormatWith(i + 1, DateTime.Now.ToString("HH:mm:ss.ffff")));
                }
                var accessToken = AccessTokenContainer.GetAccessToken(Config.AppId, true);
                return Content("异步消息发送完毕.AccessToken:" + accessToken);
            }).Result;
        }


    }
}