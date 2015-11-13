using System;
using System.Collections;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetduinoStation
{
    public class NistTime
    {
        public NistTime()
            : this(IPAddress.Parse("132.163.4.101"))
        {
        }

        public NistTime(IPAddress timeServerIpAddress)
        {
            this.timeServerIpAddress = timeServerIpAddress;
        }

        public DateTime getDateTime(int UTC = 0)
        {
            string dateTimeString = QueryNistTime();
            DateTime dateTime = ParceNistAnswer(dateTimeString);
            dateTime = dateTime.AddHours(UTC);
            return dateTime;
        }

        private string QueryNistTime()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                IPEndPoint hostPoint = new IPEndPoint(timeServerIpAddress, 13);
                socket.Connect(hostPoint);
                int numberOfBytes = socket.Receive(buffer);
                if (numberOfBytes == 51)
                {
                    string timeString = new String(System.Text.Encoding.UTF8.GetChars(buffer, 0, numberOfBytes)).Trim();
                    return timeString;
                }
                else
                    return "";
            }
        }

        private char[] separators = new char[] { ' ' };

        private System.Globalization.CultureInfo EnglishUSACulture = new System.Globalization.CultureInfo("en-US");

        private DateTime ParceNistAnswer(string timeString)
        {
            string[] resultTokensRaw = timeString.Split(separators/*, StringSplitOptions.RemoveEmptyEntries*/);
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
            double millis = double.Parse(resultTokens[6]/*, EnglishUSACulture*/);     // this is speculative: official documentation seems out of date!
            
            now = now.AddHours(hours);
            now = now.AddMinutes(minutes);
            now = now.AddSeconds(seconds);
            now = now.AddMilliseconds(-millis);

            return now;
        }

        private IPAddress timeServerIpAddress;
        private byte[] buffer = new byte[256];
    }
}
