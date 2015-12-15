using System;
using Microsoft.SPOT;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetduinoStation
{
	/**
	* Позволяет получить текущее время с сервера синхронизации времени
	*/
	public class NistTime
	{
		/**
		* По-умолчанию используется сервер с IP-адресом 132.163.4.101 (time-a.timefreq.bldrdoc.gov)
		*/
		public NistTime() : this(IPAddress.Parse("132.163.4.101"))
		{
		}
	
		/**
		* Можно указатьь желаемый адрес сервера синхронизации времени
		* <param name="timeServerIpAddress">IP-адорес сервера синхронизации времени</param>
		*/
		public NistTime(IPAddress timeServerIpAddress)
		{
			TimeServerIpAddress = timeServerIpAddress;
		}
		
		/**
		* Получить текущее время, полученное с сервера синхронизации времени
		* <param name="utc">Позволяет задать смещение часового пояса. По-умолчанию = 0</param>
		* <returns>Текущее время в виде объекта стандартного класса <c>DateTime</c></returns>
		*/
		public DateTime GetDateTime(int utc = 0)
		{
			string nistTime = QueryNistTime();
			if (!nistTime.Equals(string.Empty))
			{
				DateTime dateTime = ParseNistAnswer(nistTime);
				dateTime = dateTime.AddHours(utc);
				return dateTime;
			}
			else
			{
				return DateTime.Now;
			}
		}
		
		/**
		* 
		*/
		private bool SocketConnected(Socket socket)
		{
			bool part1 = socket.Poll(1000, SelectMode.SelectRead);
			bool part2 = (socket.Available == 0);

			if (part1 && part2)
				return false;
			else
				return true;
		}
		
		/**
		* Запрашивает время и дату с сервера синхронизации времени. Получет ее в виде заданном NIST ACTS
		* Формат строки: JJJJJ YR-MO-DA HH:MM:SS TT L DUT1 msADV UTC(NIST) OTM
		* Подробнее: http://www.nist.gov/pml/div688/grp40/acts.cfm
		* <returns>Строка с текущими временными настройками, полученными от сервера</returns>
		*/
		private string QueryNistTime()
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
		
		/**
		* Разделитель строки, согласно которым ее требуется разбивать
		*/
		char[] separators = new char[] { ' ' };
		
		/**
		* Разбирает строку, содержащую дату и время в виде, определенном NIST ACTS.
		* <param name="timeString">Строка, полученная от сервера синхронизации времени</param>
		* <returns>Время, и дата полученные в качетсве параметра в виде объекта стандартного класса <c>DateTime</c></returns>
		*/
		private DateTime ParseNistAnswer(string timeString)
        {
            Debug.Print(timeString);
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

            string[] dateTokens = resultTokens[1].Split('-');
            string[] timeTokens = resultTokens[2].Split(':');
            DateTime now = new DateTime(int.Parse(dateTokens[0])+2000,
                                        int.Parse(dateTokens[1]),
                                        int.Parse(dateTokens[2]),
                                        int.Parse(timeTokens[0]),
                                        int.Parse(timeTokens[1]),
                                        int.Parse(timeTokens[2])
                                        );
            double millis = double.Parse(resultTokens[6]);
            now = now.AddMilliseconds(millis);

            return now;
        }

		public IPAddress TimeServerIpAddress;
		byte[] _buffer = new byte[256];
	}
}
