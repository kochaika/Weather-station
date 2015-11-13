using System;
using Microsoft.SPOT;

namespace NetduinoStation
{
    class WeatherInfo
    {
        public int Shade_temperature {get; set;}
        public int Light_temperature {get; set;}
        public string Scale {get; set;}
        public string Illumination {get; set;}
        public DateTime DateTime {get; set;}
    }
}
