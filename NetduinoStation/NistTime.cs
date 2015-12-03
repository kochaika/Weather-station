using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Microsoft.SPOT;

namespace NetduinoStation
{
	public class NistTime
	{
		public NistTime() : this(IPAddress.Parse("132.163.4.101"))
		{
		}

		public NistTime(IPAddress timeServerIpAddress)
		{
			TimeServerIpAddress = timeServerIpAddress;
			DateTime dt = GetDateTime(4);
		}

		public DateTime GetDateTime(int utc)
		{
			string nistTime = QueryNistTime();
			DateTime dateTime = ParseNistAnswer(nistTime);
			dateTime = dateTime.AddHours(utc);
			return dateTime;
		}

		bool SocketConnected(Socket socket)
		{
			bool part1 = socket.Poll(1000, SelectMode.SelectRead);
			bool part2 = (socket.Available == 0);

			if (part1 && part2)
				return false;
			else
				return true;
		}

		string QueryNistTime()
		{
			using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				IPEndPoint hostPoint = new IPEndPoint(TimeServerIpAddress, 13);
				socket.Connect(hostPoint);

				if (SocketConnected(socket))
				{
					int numberOfBytes = socket.Receive(_buffer);
					if (numberOfBytes == 51)
					{
						return new String(Encoding.UTF8.GetChars(_buffer, 0, numberOfBytes)).Trim();
					}
					else
						return string.Empty;
				}
				return string.Empty;
			}
		}

		char[] separators = new char[] { ' ' };

		CultureInfo EnglishUSACulture = new CultureInfo("en-US");

		DateTime ParseNistAnswer(string timeString)
		{
			string[] resultTokensRaw = timeString.Split(separators);
			string[] resultTokens = new string[resultTokensRaw.Length];
			int j = 0;

			for (int i = 0; i < resultTokensRaw.Length; i++)
				if (resultTokensRaw[i] != " " && resultTokensRaw[i] != "")
					resultTokens[j++] = resultTokensRaw[i];

			if (resultTokens[7] != "UTC(NIST)" || resultTokens[8] != "*")
			{
				throw new ApplicationException("Invalid RFC-867 daytime protocol string: " + timeString);
			}

			int mjd = int.Parse(resultTokens[0]);  // "JJJJ is the Modified Julian Date (MJD). The MJD has a starting point of midnight on November 17, 1858."
			DateTime now = new DateTime(1858, 11, 17);
			now = now.AddDays(mjd);

			string[] timeTokens = resultTokens[2].Split(':');
			int hours = int.Parse(timeTokens[0]);
			int minutes = int.Parse(timeTokens[1]);
			int seconds = int.Parse(timeTokens[2]);
			double millis = double.Parse(resultTokens[6]);     // this is speculative: official documentation seems out of date!

			now = now.AddHours(hours);
			now = now.AddMinutes(minutes);
			now = now.AddSeconds(seconds);
			now = now.AddMilliseconds(-millis);
			return now;
		}

		public IPAddress TimeServerIpAddress;
		byte[] _buffer = new byte[256];
	}
}
