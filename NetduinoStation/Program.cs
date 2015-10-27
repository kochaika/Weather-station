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

            OutputPort onboardLed = new OutputPort(Pins.ONBOARD_LED, false);
            InputPort inputButton = new InputPort(Pins.ONBOARD_BTN,false, ResistorModes.Disabled);
 

            while (true)
            {
                if (inputButton.Read())
                {
                    onboardLed.Write(true);
                    Thread.Sleep(1000);
                    onboardLed.Write(false);
                }
            } 
        }

    }
}
