using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using Newtonsoft.Json;
using Senparc.Weixin.Helpers.Extensions;
using SenparcCourse.Service.Dtos;

namespace SenparcCourse.Controllers
{
    public class RequestController : Controller
    {
        /// <summary>
        ///  GET: Request 读取页面信息、接口信息、模拟登陆、从远程抓取数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult Get(string url = "https://www.baidu.com")
        {
            var html = Senparc.Weixin.HttpUtility.RequestUtility.HttpGet(url, Encoding.GetEncoding("GB2312"));
            html += "<script>alert('Get OK')</script>";
            return Content(html);
        }

        /// <summary>
        /// 模拟POST
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult Post(string url = "http://sdk.weixin.senparc.com/AsyncMethods/TemplateMessageTest", string code = "")
        {
            var formData = new Dictionary<string, string>
            {
                { "checkcode", code }
            };

            var html = Senparc.Weixin.HttpUtility.RequestUtility.HttpPost(url, null, formData);
            html += "<script>alert('Post OK')</script>";
            return Content(html);
        }
        /// <summary>
        /// Json 格式返回天气信息
        /// </summary>
        /// <returns></returns>
        //https://www.sojson.com/open/api/weather/json.shtml?city=%E4%B8%8A%E6%B5%B7
        public ActionResult GetJson()
        {
            string url = "https://www.sojson.com/open/api/weather/json.shtml?city=%E4%B8%8A%E6%B5%B7";

            var weatherResult = Senparc.Weixin.HttpUtility.Get.GetJson<WeatherAllInfoResult>(url, null, null);

            return Content(JsonConvert.SerializeObject(weatherResult));
        }
        /// <summary>
        /// 模拟登陆，获取Cookie
        /// </summary>
        /// <returns></returns>
        public ActionResult SimulateLogin()
        {
            var url = "https://www.baidu.com";
            var cookieContainer = new CookieContainer();

            var html = Senparc.Weixin.HttpUtility.RequestUtility.HttpGet(url, cookieContainer, Encoding.UTF8, null, null, false);


            return Content(html);
        }


        public ActionResult GetImage(string url = "http://sdk.weixin.senparc.com/images/book-cover-front-small-3d-transparent.png")
        {
            string fileName = Server.MapPath("~/App_Data/DownloadImage_{0}.jpg".FormatWith(DateTime.Now.Ticks));

            Senparc.Weixin.HttpUtility.Get.Download(url, fileName);

            return Content("图片保存：" + fileName);
        }

    }
}