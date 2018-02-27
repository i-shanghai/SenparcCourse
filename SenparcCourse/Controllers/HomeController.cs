using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Senparc.Weixin.Cache;

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
            //Lock 实现 独占、锁住
            var strategy = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (strategy.BeginCacheLock("SenparcCourse","LockTest"))
            {
                var count = Count;//读取 Count 

                Thread.Sleep(100); //休0.1s , 模拟并发场景，所有线程都会在这里停留0.1秒

                Count = count + 1; //设置 Count

                return Content(Count.ToString());
            } 
        }
    }
}