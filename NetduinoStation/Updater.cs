using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System;
using System.Threading;
using System.Net;

namespace NetduinoStation
{
	class Updater
	{
		#region Fields

		public int TimeUpdateInterval { get; private set; }
		public int WeatherUpdateInterval { get; private set; }
		public WeatherInfo WeatherInfo { get; private set; }
		public string NistServerAddress { get; private set; }
		NistTime nistTime;
		Timer timerUpdateTime;
		Timer timerUpdateWeather;
		WirelessTransmitter wirelessTransmitter;

		#endregion

		public Updater()
		{
			WeatherInfo = new WeatherInfo();
			TimeUpdateInterval = 1;
			WeatherUpdateInterval = 5;
			NistServerAddress = "132.163.4.101";
			nistTime = new NistTime(IPAddress.Parse(NistServerAddress));
			wirelessTransmitter = new WirelessTransmitter();
			wirelessTransmitter.Start();
		}

		public void Start()
		{
			TimerCallback callbackUpdateTime = UpdateTime;
			TimerCallback callbackUpdateWeather = UpdateWeather;

			if (timerUpdateTime == null)
			{
				timerUpdateTime = new Timer(callbackUpdateTime, null, new TimeSpan(0, 0, 2),
				new TimeSpan(0, TimeUpdateInterval, 0));
			}

			if (timerUpdateWeather == null)
			{
				timerUpdateWeather = new Timer(callbackUpdateWeather, null, new TimeSpan(0, 0, 2),
				new TimeSpan(0, WeatherUpdateInterval, 0));
			}
		}


		public void UpdateConfiguration(int timeUpdateInterval, int weatherUpdateInterval, string scale, string nistServerAddress)
		{
			nistTime.TimeServerIpAddress = IPAddress.Parse(nistServerAddress);
			NistServerAddress = nistServerAddress;

			if (timeUpdateInterval != TimeUpdateInterval)
			{
				TimeUpdateInterval = timeUpdateInterval;

				if (timerUpdateTime != null)
				{
					timerUpdateTime.Change(new TimeSpan(0, 0, 0), new TimeSpan(0, timeUpdateInterval, 0));
				}
			}

			if (weatherUpdateInterval != WeatherUpdateInterval || WeatherInfo.Scale != scale)
			{
				WeatherInfo.Scale = scale;
				WeatherUpdateInterval = weatherUpdateInterval;

				if (timerUpdateWeather != null)
				{
					timerUpdateWeather.Change(new TimeSpan(0, 0, 0), new TimeSpan(0, weatherUpdateInterval, 0));
				}
			}
		}

		void UpdateTime(Object state)
		{
			DateTime time = nistTime.GetDateTime(3);
			Utility.SetLocalTime(time);
			Debug.Print("Time has been just updated: " + DateTime.Now.ToString());
		}

		public void UpdateWeather(Object state)
		{
			WeatherInfo.DateTime = DateTime.Now;
			if (WeatherInfo.Scale.Equals("c"))
			{
				WeatherInfo.ShadeTemperature = (int)(wirelessTransmitter.GetShadeTemperature()/100);
				WeatherInfo.LightTemperature = (int)(wirelessTransmitter.GetLightTemperature()/100);
			}
			else
			{
				WeatherInfo.ShadeTemperature = 32 + 9 * (int)(wirelessTransmitter.GetShadeTemperature()/100) / 5;
				WeatherInfo.LightTemperature = 32 + 9 * (int)(wirelessTransmitter.GetLightTemperature()/100) / 5;
			}
			
			WeatherInfo.Illumination = wirelessTransmitter.GetIllumination();
			Debug.Print("Weather has just been updated...");
		}

	}
}
