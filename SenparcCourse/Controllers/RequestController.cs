using Newtonsoft.Json;
using Senparc.Weixin.Helpers.Extensions;
using SenparcCourse.Service.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.Containers;
using Config = SenparcCourse.Service.Config;

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
        public ActionResult Post(string url = "http://sdk.weixin.senparc.com/AsyncMethods/TemplateMessageTest",
            string code = "")
        {
            var formData = new Dictionary<string, string>
            {
                {"checkcode", code}
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

            var html = Senparc.Weixin.HttpUtility.RequestUtility.HttpGet(url, cookieContainer, Encoding.UTF8, null,
                null, false);


            return Content(html);
        }

        /// <summary>
        /// 直接从URL下载图片
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult GetImage(
            string url = "http://sdk.weixin.senparc.com/images/book-cover-front-small-3d-transparent.png")
        {
            //string fileName = Server.MapPath("~/App_Data/DownloadImage_{0}.jpg".FormatWith(DateTime.Now.Ticks));

            string filePath = Server.MapPath("~/App_Data/"); //下载保存路径
            string fileName = Senparc.Weixin.HttpUtility.Get.Download(url, filePath); //执行下载
            string newFileName = fileName + ".png"; //重命名文件

            System.IO.File.Move(fileName, newFileName);


            return Content("图片保存成功");
        }


        #region 使用流的方式从URL下载图片,然后通过流的方式上传。分为 两步

        /// <summary>
        /// 使用流的方式从URL下载图片,然后通过流的方式上传。分为 两步
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns> 
        public ActionResult GetUploadImage(
            string url = "http://sdk.weixin.senparc.com/images/book-cover-front-small-3d-transparent.png")
        {
            //string fileName = Server.MapPath("~/App_Data/DownloadImage_{0}.jpg".FormatWith(DateTime.Now.Ticks)); 

            using (var ms = new MemoryStream())
            {
                Senparc.Weixin.HttpUtility.Get.Download(url, ms);
                ms.Seek(0, SeekOrigin.Begin); //指针归到0 的位置

                var uploadUrl = "http://localhost:8376/Request/UploadImage";
                Senparc.Weixin.HttpUtility.RequestUtility.HttpPost(uploadUrl, null, ms, timeOut: 30000);
            }

            return Content("图片下载,然后上传完成");
        }

        /// <summary>
        /// 使用流的方式上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadImage()
        {
            var stream = Request.InputStream; //保存：Post 过来 的二进制流

            //创建新的文件
            var fileName = Server.MapPath("~/App_Data/UploadImage.jpg");
            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                stream.CopyTo(fileStream);
                stream.Flush();
            }

            return Content("图片下载、并以流的方式上传到Server:" + fileName);
        }

        #endregion

        #region HttpPostedFileBase 的方式上传

        /// <summary>
        /// 图片塞进Form中上传
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAImageAndUpload(
            string url = "http://sdk.weixin.senparc.com/images/book-cover-front-small-3d-transparent.png")
        {
            string filePath = Server.MapPath("~/App_Data/"); //下载保存路径
            string fileName = Senparc.Weixin.HttpUtility.Get.Download(url, filePath); //先执行下载
            string newFileName = fileName + ".png";

            System.IO.File.Move(fileName, newFileName); //重命名文件

            //修改为Form表单的形式，Post上传
            var dictionary = new Dictionary<string, string>
            {
                {"file", newFileName}
            };

            var uploadUrl = "http://localhost:8376/Request/UploadFormImage";
            var uploadResult =
                Senparc.Weixin.HttpUtility.RequestUtility.HttpPost(uploadUrl, null, null, dictionary); //再执行上传

            return Content("图片上传完成:<br/>" + uploadResult);
        }

        /// <summary>
        /// 以HttpPostedFileBase的方式，接收文件并保存
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFormImage(HttpPostedFileBase file)
        {
            var stream = file.InputStream;

            //创建新的文件
            var fileName = Server.MapPath("~/App_Data/UploadImageByHttpPostFile.jpg");
            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                stream.CopyTo(fileStream);
                stream.Flush();
            }

            return Content("图片下载、并以HttpPostedFile的方式上传到Server:" + fileName);
        }

        #endregion


        public class MyClass
        {
            public string Data;
        }

        public ActionResult GetAccessToken()
        {
            //WeixinNullReferenceException 处理
            var myclass = new MyClass();
            try
            {
                if (myclass.Data == null)
                {
                    throw new WeixinNullReferenceException("MyClass Data Is Null", myclass);
                }
            }
            catch (WeixinNullReferenceException ex)
            {
                var obj = ex.ParentObject as MyClass;
                obj.Data = "Data Is Null";

                Senparc.Weixin.WeixinTrace.SendCustomLog("系统日志", "MyClass Data Is Null"); 
            }


            var accessToken = AccessTokenContainer.GetAccessToken(Config.AppId, true);

            //自定义 Log
            WeixinTrace.SendCustomLog("TestLog", "哈哈哈");

            return Content("accessToken获取成功：" + accessToken.Substring(0, 20) + "\n" + Config.LogRecordCount.ToString());
        }



    }
}