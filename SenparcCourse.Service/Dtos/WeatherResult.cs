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
}
