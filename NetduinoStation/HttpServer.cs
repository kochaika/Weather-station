using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO;
using System.Text;
using Json.NETMF;
using Json;
using System.Collections;

namespace NetduinoStation
{
    /// <summary>
    /// on error event arguments
    /// </summary>
    public class OnErrorEventArgs : EventArgs
    {
        private string EVENT_MESSAGE;
        /// <summary>
        /// property containing event message
        /// </summary>
        public string EventMessage
        {
            get { return EVENT_MESSAGE; }
        }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="EVENT_MESSAGE">message</param>
        public OnErrorEventArgs(string EVENT_MESSAGE)
        {
            this.EVENT_MESSAGE = EVENT_MESSAGE;
        }
    }
    /// <summary>
    /// interface used for event handeling
    /// </summary>
    /// <param name="sender">object sender</param>
    /// <param name="e">arguments passed</param>
    public delegate void OnErrorDelegate(object sender, OnErrorEventArgs e);
    /// <summary>
    /// http server class
    /// </summary>
    public class HttpServer
    {
        private Thread SERVER_THREAD;
        private Socket LISTEN_SOCKET;
        private Socket ACCEPTED_SOCKET;
        private string LISTEN_IP;
        private int LISTEN_PORT;
        private bool IS_SERVER_RUNNING;
        private string STORAGE_PATH;
        private FileStream FILE_STREAM;
        private StreamReader FILE_READER;
        private StreamWriter FILE_WRITER;
        private byte[] RECEIVE_BUFFER;
        private byte[] SEND_BUFFER;
        WeatherInfo weatherInfo;
		Updater updater;
        /// <summary>
        /// property returns true if server is running and listening
        /// </summary>
        public bool IsServerRunning
        {
            get { return IS_SERVER_RUNNING; }
        }
        /// <summary>
        /// returns the ip address obtained from the dhcp server
        /// </summary>
        public string ObtainedIp
        {
            get { return LISTEN_IP; }
        }
        /// <summary>
        /// returns the running thread, use this if you want to set server thread priority
        /// </summary>
        public Thread RunningThread
        {
            get { return SERVER_THREAD; }
        }
        private enum FileType { JPEG = 1, GIF=2, Html = 3, CSS = 4, PNG = 5 };

        /// <summary>
        /// event fired when an error occurs
        /// </summary>
        public event OnErrorDelegate OnServerError;
        string HtmlPageHeader = "HTTP/1.0 200 OK\r\nContent-Type: ";
        /// <summary>
        /// event fire function
        /// </summary>
        /// <param name="e">event passed</param>
        protected virtual void OnServerErrorFunction(OnErrorEventArgs e)
        {
            OnServerError(this, e);
        }
        private void FragmentateAndSend(string FILE_NAME, FileType Type)
        {
            byte[] HEADER;
            long FILE_LENGTH;

            try
            {
                FILE_STREAM = new FileStream(STORAGE_PATH + "\\" + FILE_NAME, FileMode.Open, FileAccess.Read);
                FILE_READER = new StreamReader(FILE_STREAM);
                FILE_LENGTH = FILE_STREAM.Length;
            }
            catch (IOException e)
            {
				Debug.Print("Ooops File:   " + STORAGE_PATH + "\\" + FILE_NAME + e.Message);
                return;
            }
            
            
            switch (Type)
            {
                case FileType.Html:
                    HEADER = UTF8Encoding.UTF8.GetBytes(HtmlPageHeader + "text/html" + "; charset=utf-8\r\nContent-Length: " + FILE_LENGTH.ToString() + "\r\n\r\n");
                    break;
                case FileType.GIF:
                    HEADER = UTF8Encoding.UTF8.GetBytes(HtmlPageHeader + "image/gif" + "; charset=utf-8\r\nContent-Length: " + FILE_LENGTH.ToString() + "\r\n\r\n");
                    break;
                case FileType.JPEG:
                    HEADER = UTF8Encoding.UTF8.GetBytes(HtmlPageHeader + "image/jpeg" + "; charset=utf-8\r\nContent-Length: " + FILE_LENGTH.ToString() + "\r\n\r\n");
                    break;
                case FileType.CSS:
                    HEADER = UTF8Encoding.UTF8.GetBytes(HtmlPageHeader + "text/css" + "; charset=utf-8\r\nContent-Length: " + FILE_LENGTH.ToString() + "\r\n\r\n");
                    break;
                case FileType.PNG:
                    HEADER = UTF8Encoding.UTF8.GetBytes(HtmlPageHeader + "image/png" + "; charset=utf-8\r\nContent-Length: " + FILE_LENGTH.ToString() + "\r\n\r\n");
                    Debug.Print(HtmlPageHeader + "image/png" + "; charset=utf-8\r\nContent-Length: " + FILE_LENGTH.ToString());
                    break;
                default:
                    HEADER = UTF8Encoding.UTF8.GetBytes(HtmlPageHeader + "text/html" + "; charset=utf-8\r\nContent-Length: " + FILE_LENGTH.ToString() + "\r\n\r\n");
                    break;
            }
           
            ACCEPTED_SOCKET.Send(HEADER, 0, HEADER.Length, SocketFlags.None);
            
            while (FILE_LENGTH > SEND_BUFFER.Length)
            {
                FILE_STREAM.Read(SEND_BUFFER, 0, SEND_BUFFER.Length);
                ACCEPTED_SOCKET.Send(SEND_BUFFER, 0, SEND_BUFFER.Length, SocketFlags.None);
                FILE_LENGTH -= SEND_BUFFER.Length;
            }
            FILE_STREAM.Read(SEND_BUFFER, 0, (int)FILE_LENGTH);
            ACCEPTED_SOCKET.Send(SEND_BUFFER, 0, (int)FILE_LENGTH, SocketFlags.None);
            
            FILE_READER.Close();
            FILE_STREAM.Close();
        }
        private string GetFileName(string RequestStr)
        {
            RequestStr = RequestStr.Substring(RequestStr.IndexOf("GET /") + 5);
            RequestStr = RequestStr.Substring(0, RequestStr.IndexOf("HTTP"));
            return RequestStr.Trim();
        }
        private bool RequestContains(string Request, string Str)
        {
            return (Request.IndexOf(Str) >= 0);
        }

        private string GetFileExtention(string FILE_NAME)
        {
            string x=FILE_NAME;
            x = x.Substring(x.LastIndexOf('.') + 1);
            return x;
        }
        private void ProcessRequest()
        {
            bool exist = false;
            string filename = "";
            string[] FILES;
            string REQUEST = "";
            string FILE_NAME = "";
            string FILE_EXTENTION = "";
            ACCEPTED_SOCKET.Receive(RECEIVE_BUFFER);
            REQUEST = new string(UTF8Encoding.UTF8.GetChars(RECEIVE_BUFFER));
            Debug.Print("\n     * * * "+REQUEST);
            FILES = Directory.GetFiles(STORAGE_PATH);
            FILE_NAME = GetFileName(REQUEST).ToLower();
            if (FILE_NAME.IndexOf("/") > 0)
            {
                filename = FILE_NAME.Substring(0, FILE_NAME.IndexOf("/"));
                filename += @"\";
                filename += FILE_NAME.Substring(FILE_NAME.IndexOf("/") + 1);

                FILE_NAME = filename;             
            }          
            if (FILE_NAME == "" || RequestContains(FILE_NAME, "index"))
            {
                FragmentateAndSend("index.html", FileType.Html);
            }
            else if ( FILE_NAME.Equals("weatherinfo.json"))
            {
				weatherInfo = updater.WeatherInfo;
                Hashtable hashtable = new Hashtable();
                hashtable.Add("Shade_temperature",weatherInfo.ShadeTemperature);
                hashtable.Add("Light_temperature", weatherInfo.LightTemperature);
                hashtable.Add("Scale", weatherInfo.Scale);
                hashtable.Add("Illumination", weatherInfo.Illumination);
                hashtable.Add("DateTime", weatherInfo.DateTime);
                string json = JsonSerializer.SerializeObject(hashtable);
                Debug.Print(json);
                byte[] response = UTF8Encoding.UTF8.GetBytes(HtmlPageHeader + "text/json;charset=UTF-8;\r\nContent-Length: " + UTF8Encoding.UTF8.GetBytes("[" + json + "]").Length + "\r\n\r\n" + "[" + json + "]");
                ACCEPTED_SOCKET.Send(response, response.Length, 0);
            }
			else if (FILE_NAME.Equals("datetime.json"))
			{
				string json = JsonSerializer.SerializeObject(DateTime.Now);
				Debug.Print(json);
				byte[] response = UTF8Encoding.UTF8.GetBytes(HtmlPageHeader + "text/json;charset=UTF-8;\r\nContent-Length: " + UTF8Encoding.UTF8.GetBytes("[" + json + "]").Length + "\r\n\r\n" + "[" + json + "]");
				ACCEPTED_SOCKET.Send(response, response.Length, 0);
			}
            else
            {
                try
                {
                    exist = File.Exists(STORAGE_PATH + @"\" + FILE_NAME);
                }
                catch(Exception)
                {
                    exist = false;
                }
                if (exist)
                {
                    FILE_EXTENTION = GetFileExtention(FILE_NAME.ToLower());
                    switch (FILE_EXTENTION)
                    {
                        case "gif":
                            FragmentateAndSend(FILE_NAME, FileType.GIF);
                            break;
                        case "txt":
                            FragmentateAndSend(FILE_NAME, FileType.Html);
                            break;
                        case "jpg":
                            FragmentateAndSend(FILE_NAME, FileType.JPEG);
                            break;
                        case "jpeg":
                            FragmentateAndSend(FILE_NAME, FileType.JPEG);
                            break;
                        case "htm":
                            FragmentateAndSend(FILE_NAME, FileType.Html);
                            break;
                        case "html":
                            FragmentateAndSend(FILE_NAME, FileType.Html);
                            break;
                        case "css":
                            FragmentateAndSend(FILE_NAME, FileType.CSS);
                            break;
                        case "png":
                            FragmentateAndSend(FILE_NAME, FileType.PNG);
                            break;
                        default:
                            FragmentateAndSend(FILE_NAME, FileType.Html);
                            break;
                    }
                }
                else
                {
                    FragmentateAndSend("NotFound.txt",FileType.Html);
                }

            }
        }
        private void RunServer()
        {
            
            try
            {
                LISTEN_SOCKET = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint BindingAddress = new IPEndPoint(IPAddress.Any, LISTEN_PORT);
                LISTEN_SOCKET.Bind(BindingAddress);
                LISTEN_SOCKET.Listen(1);
                IS_SERVER_RUNNING = true;
                while (true)
                {
                    ACCEPTED_SOCKET = LISTEN_SOCKET.Accept();
                    ProcessRequest();
                    ACCEPTED_SOCKET.Close();
                }
            }
            catch (Exception)
            {
                IS_SERVER_RUNNING = false;
                ACCEPTED_SOCKET.Close();
                LISTEN_SOCKET.Close();
                OnServerErrorFunction(new OnErrorEventArgs("Server Error\r\nCheck Connection Parameters"));
            }
        }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="port">listening port</param>
        /// <param name="ReceiveBufferSize">receive buffer size must be > 50 to handle the http request correctly</param>
        /// <param name="SendBufferSize">send Buffer size, data is fragmented hence the fragment size depends on this</param>
        /// <param name="pages_folder">the directory where pages are placed, if using sd card fill this with @"\SD"</param>
        public HttpServer(int port, int ReceiveBufferSize, int SendBufferSize, string pages_folder)
        {
            SERVER_THREAD = null;
            LISTEN_SOCKET = null;
            ACCEPTED_SOCKET = null;
            IS_SERVER_RUNNING = false;
            LISTEN_PORT = port;
            STORAGE_PATH = pages_folder;
            RECEIVE_BUFFER = new byte[ReceiveBufferSize];
            SEND_BUFFER = new byte[SendBufferSize];
            LISTEN_IP = "No Ip Address";
            SERVER_THREAD = new Thread(new ThreadStart(RunServer));
            if (!File.Exists(STORAGE_PATH + "\\NotFound.txt"))
            {
                FILE_STREAM = new FileStream(STORAGE_PATH + "\\NotFound.txt", FileMode.Create, FileAccess.Write);
                FILE_WRITER = new StreamWriter(FILE_STREAM);
                FILE_WRITER.WriteLine("<html>");
                FILE_WRITER.WriteLine("<head>");
                FILE_WRITER.WriteLine("<title>");
                FILE_WRITER.WriteLine("Page Not Found");
                FILE_WRITER.WriteLine("</title>");
                FILE_WRITER.WriteLine("<body>");
                FILE_WRITER.WriteLine("<h1 align=center>");
                FILE_WRITER.WriteLine("Page Not Found");
                FILE_WRITER.WriteLine("</h1>");
                FILE_WRITER.WriteLine("</body>");
                FILE_WRITER.WriteLine("</html>");
                FILE_WRITER.Close();
                FILE_STREAM.Close();
            }
            Thread.Sleep(5000);
			LISTEN_IP = NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress;
			
			updater = new Updater();
			updater.Start();         
        }
        /// <summary>
        /// starts the server and listens to connections
        /// </summary>
        public void Start()
        {
            SERVER_THREAD.Start();
        }
        /// <summary>
        /// stops the server activity 
        /// </summary>
        public void Stop()
        {
            LISTEN_SOCKET.Close();
        }
    }
}


 

    