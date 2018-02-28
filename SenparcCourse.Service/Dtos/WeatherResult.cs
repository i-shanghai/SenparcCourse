using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenparcCourse.Service.Dtos
{
    public class WeatherResult
    {
        public string date { get; set; }

        public string message { get; set; }

        public string status { get; set; }

        public string city { get; set; }

        public int count { get; set; }

        public string AddTime { get; set; }

        public WeatherResult()
        {
            AddTime = DateTime.Now.ToString("HH:mm:ss");
        }
    }


    public class WeatherAllInfoResult
    {
        public string date { get; set; }
        public string message { get; set; }
        public int status { get; set; }
        public string city { get; set; }
        public int count { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string shidu { get; set; }
        public float pm25 { get; set; }
        public float pm10 { get; set; }
        public string quality { get; set; }
        public string wendu { get; set; }
        public string ganmao { get; set; }
        public Yesterday yesterday { get; set; }
        public Forecast[] forecast { get; set; }
    }

    public class Yesterday
    {
        public string date { get; set; }
        public string sunrise { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string sunset { get; set; }
        public float aqi { get; set; }
        public string fx { get; set; }
        public string fl { get; set; }
        public string type { get; set; }
        public string notice { get; set; }
    }

    public class Forecast
    {
        public string date { get; set; }
        public string sunrise { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string sunset { get; set; }
        public float aqi { get; set; }
        public string fx { get; set; }
        public string fl { get; set; }
        public string type { get; set; }
        public string notice { get; set; }
    }

}
