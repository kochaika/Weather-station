using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using MicroTweet;

namespace NetduinoStation
{
    public class Program
    {
        private static HttpServer Server;//server object
        private static OutputPort LedPin;//led pin (onboared)
        public static void Main()
        {
            var interf = Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0];
			//interf.EnableStaticIP("172.18.4.61", "255.255.244.0", "172.18.0.1");
            interf.EnableDhcp();
            Thread.Sleep(1000);
			Debug.Print("IP:" + interf.IPAddress.ToString() + " Mask: " + interf.SubnetMask.ToString() + " Mac: " + interf.PhysicalAddress.ToString());
            //var prog = new WirelessTransmitter();
            //prog.Start();

		
            LedPin = new OutputPort(Pins.ONBOARD_LED, false);
            Server = new HttpServer(80, 256, 512, @"\SD");
            Server.OnServerError += new OnErrorDelegate(Server_OnServerError);
            Server.Start();
            Debug.Print("IP ADDRESS OBTAINED : " + Server.ObtainedIp);

            while (true)
            {
                LedPin.Write(true);
                Thread.Sleep(100);
                LedPin.Write(false);
                Thread.Sleep(100);
            }

        }
        static void Server_OnServerError(object sender, OnErrorEventArgs e)
        {
            Debug.Print(e.EventMessage);
        }
    }
}
