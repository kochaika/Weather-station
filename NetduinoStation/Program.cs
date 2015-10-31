using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace NetduinoStation
{
    public class Program
    {
        public static void Main()
        {

            Thread.Sleep(1000);
            var prog = new WirelessTransmitter();
            prog.Start();
            Thread.Sleep(Timeout.Infinite);
        }

    }
}
