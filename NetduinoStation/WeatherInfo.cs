using System;
using Microsoft.SPOT;

namespace NetduinoStation
{
    class WeatherInfo
    {
        public int ShadeTemperature { get; set; }
		public int LightTemperature { get; set; }
		public string Scale { get; set; }
		public string Illumination { get; set; }
		public DateTime DateTime { get; set; }

		public WeatherInfo()
		{
			ShadeTemperature = 0;
			LightTemperature = 0;
			Scale = "c";
			Illumination = "Cloudly";
			DateTime = DateTime.Now;
		}
    }
}