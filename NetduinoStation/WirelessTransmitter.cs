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
		NRF24L01Plus _radio;
		short _response;
		bool _responsed;

		public WirelessTransmitter()
		{
			_radio = new NRF24L01Plus();
			_responsed = false;
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
			_radio.OnDataReceived += OnDataReceived;
			_radio.Enable();

			Debug.Print("Listening on: " +
						ByteArrayToHexString(_radio.GetAddress(AddressSlot.Zero, 5)) + " | " +
						ByteArrayToHexString(_radio.GetAddress(AddressSlot.One, 5)) + " | " +
						ByteArrayToHexString(_radio.GetAddress(AddressSlot.Two, 5)) + " | " +
						ByteArrayToHexString(_radio.GetAddress(AddressSlot.Three, 5)) + " | " +
						ByteArrayToHexString(_radio.GetAddress(AddressSlot.Four, 5)) + " | " +
						ByteArrayToHexString(_radio.GetAddress(AddressSlot.Five, 5)));

		}

		string ByteArrayToHexString(byte[] bytes)
		{
			StringBuilder result = new StringBuilder(bytes.Length * 2);
			string HexAlphabet = "0123456789ABCDEF";

			foreach (byte b in bytes)
			{
				result.Append(HexAlphabet[(int)(b >> 4)]).Append(HexAlphabet[(int)(b & 0xF)]);
			}

			return result.ToString();
		}

		void OnDataReceived(byte[] data)
		{
			byte[] array = new byte[2];
			array[0] = data[31];
			array[1] = data[30];
			_response = BitConverter.ToInt16(array, 0);
			_responsed = true;
			Debug.Print("Received: " + _response);
		}

		void SendRequest(RequestCodes request)
		{
			byte[] array = BitConverter.GetBytes((short)request);
			byte temp = array[0];
			array[0] = array[1];
			array[1] = temp;
			_radio.SendTo(new byte[] { 0xF0, 0xF0, 0xF0, 0xF0, 0xE1 }, array);
			Debug.Print("Sent: " + request);
		}

		public short GetShadeTemperature()
		{
			_responsed = false;
			SendRequest(RequestCodes.ShadeTemperature);
			while (!_responsed);
			return _response;
		}

		public short GetLightTemperature()
		{
			_responsed = false;
			SendRequest(RequestCodes.LightTemperature);
			while (!_responsed);
			return _response;
		}

		public string GetIllumination()
		{
			_responsed = false;
			SendRequest(RequestCodes.Light);
			while (!_responsed);
			
			switch (_response)
			{
				case (short)Illumination.Clear: return "Clear";
				case (short)Illumination.Cloudly: return "Cloudly";
				case (short)Illumination.Dark: return "Dark";
				case (short)Illumination.VeryCloudly: return "VeryCloudly";
				case (short)Illumination.VerySunny: return "VerySunny";
			}

			return string.Empty;
		}
	}
}
