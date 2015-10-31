using Microsoft.SPOT;
using NETMF.Nordic;
using SecretLabs.NETMF.Hardware.Netduino;
using System;
using System.Text;
using System.Threading;


namespace NetduinoStation
{
    class WirelessTransmitter
    {
        private NRF24L01Plus _radio;
        private Timer _timer;

        public WirelessTransmitter()
        {
            _radio = new NRF24L01Plus();
        }

        public void Start()
        {
            /* This code assumes a pin configuration as follows:
             *
             * nRF24    ->  Netduino 2  Purpose
             * --------     ----------  --------------
             *  1 (GND) ->  GND         Ground
             *  2 (VCC) ->  3V3         3v Power
             *  3 (CE)  ->  D9          Chip Enable
             *  4 (CSN) ->  D10         Chip Select
             *  5 (SCK) ->  D13         SPI Serial Clock (SCLK)
             *  6 (MOSI)->  D11         SPI Transmit (MOSI)
             *  7 (MISO)->  D12         SPI Recieve (MISO)
             *  8 (IRQ) ->  D8          Interupt              *
             */

            // Initialize the radio on our pins
            _radio.Initialize(SPI_Devices.SPI1, Pins.GPIO_PIN_D10, Pins.GPIO_PIN_D9, Pins.GPIO_PIN_D8);
            _radio.Configure(new byte[] { 0xF0, 0xF0, 0xF0, 0xF0, 0xD2 }, 76, NRFDataRate.DR2Mbps);
            _radio.OnDataReceived += _radio_OnDataReceived;
            _radio.Enable();

            Debug.Print("Listening on: " +
                        ByteArrayToHexString(_radio.GetAddress(AddressSlot.Zero, 5)) + " | " +
                        ByteArrayToHexString(_radio.GetAddress(AddressSlot.One, 5)) + " | " +
                        ByteArrayToHexString(_radio.GetAddress(AddressSlot.Two, 5)) + " | " +
                        ByteArrayToHexString(_radio.GetAddress(AddressSlot.Three, 5)) + " | " +
                        ByteArrayToHexString(_radio.GetAddress(AddressSlot.Four, 5)) + " | " +
                        ByteArrayToHexString(_radio.GetAddress(AddressSlot.Five, 5)));

            _timer = new Timer(TimerFire, null, new TimeSpan(0, 0, 0, 10), new TimeSpan(0, 0, 0, 3));
        }

        private static string ByteArrayToHexString(byte[] Bytes)
        {
            StringBuilder Result = new StringBuilder(Bytes.Length * 2);
            string HexAlphabet = "0123456789ABCDEF";

            foreach (byte B in Bytes)
            {
                Result.Append(HexAlphabet[(int)(B >> 4)]);
                Result.Append(HexAlphabet[(int)(B & 0xF)]);
            }

            return Result.ToString();
        }

        private void _radio_OnDataReceived(byte[] data)
        {
            byte[] arr = new byte[2];
            arr[0] = data[31];
            arr[1] = data[30];
            Debug.Print("Received: " + BitConverter.ToInt16(arr,0));
        }

        private void TimerFire(object state)
        {
            short request = 101;
            byte[] arr = BitConverter.GetBytes(request);
            byte temp = arr[0];
            arr[0] = arr[1];
            arr[1] = temp;
            _radio.SendTo(new byte[] { 0xF0, 0xF0, 0xF0, 0xF0, 0xE1 }, arr);
            Debug.Print("Sent: " + request);
        }
    }
}
