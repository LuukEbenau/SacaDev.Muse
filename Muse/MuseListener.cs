using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using OscParser;
using System.Linq;

namespace SacaDev.Muse
{
	public class MuseListener : IDisposable
	{
		private UdpClient Client { get; set; }
		private int Port { get; }
		public event EventHandler<MusePacket> PacketReceived;

		public void StopListening() {
			//this.Client?.Close();
			this.Client?.Dispose();//should make client cancel the Receive operation,//CHECKME: is this the right way to go? it's surely hacky but hey, the client doen't really provide an much better alternative way :)
			this.Client = null;
		}

		public void StartListening() {
			StopListening();
			this.Client = new UdpClient(Port);
			
			Task.Run(async () => {
				var endpoint = new IPEndPoint(IPAddress.Any, Port);
				try
				{
					//TODO: this is an initial version, i feel like this can be done more elegant? (atleast async, not thread blocking like now)
					while(true)
					{
						try
						{
							var result = await Client.ReceiveAsync();
							var bytes = result.Buffer;

							var message = OscPacket.GetPacket(bytes) as OscMessage;

							var musePacket = ParsePacket(message);
							if (musePacket.Address == SignalAddress.Unknown)
							{
								Console.WriteLine($"unknown packet with address '{message.Address}', skipping it...");
								continue;
							}

							PacketReceived?.Invoke(this, musePacket);
						}
						catch (ObjectDisposedException)
						{
							Console.WriteLine("disposed early");
							throw;
						}
						catch (Exception ex)
						{
							Console.WriteLine($"Corrupt packet received: '{ex.Message}'");
						}
					}
				}
				catch (ObjectDisposedException) {
					//is fine, atm this is the escape condition when the listener needs to stop listening to the port. far from elegant but it works i suppose :)
					Console.WriteLine("exited");
				}
				catch (SocketException e)
				{
					Console.WriteLine(e);
				}
				catch (Exception ex)
				{
					Console.WriteLine("something else threw an exception... " + ex.Message);
				}
				finally
				{
					this.Client?.Dispose();
					this.Client = null;
				}
			});
		}
		public MuseListener(int port) {
			this.Port = port;
			StartListening();
		}

		public MusePacket ParsePacket(OscMessage message)
		{
			var signalAddress = ParseSignalAdressFromMessageAdress(message.Address);
			string name = message.Address.Substring(0, message.Address.IndexOf('/'));
			return new MusePacket(name, signalAddress, message.Arguments.Select(a => (a as IConvertible).ToDouble(null)));
		}

		public void Dispose()
		{
			Client?.Dispose();
		}

		#region private methods
		private SignalAddress ParseSignalAdressFromMessageAdress(string address)
		{
			var firstSlashIndex = address.IndexOf('/');
			var absAdressPart = address.Substring(firstSlashIndex);

			SignalAddress parsedAddress;
			switch (absAdressPart)
			{
				case MuseAddress.NOTCHFILTEREDEEG:
					parsedAddress = SignalAddress.NotchFilteredEeg;
					break;
				case MuseAddress.EEG:
					parsedAddress = SignalAddress.Eeg;
					break;
				case MuseAddress.TOUCHINGFOREHEAD:
					parsedAddress = SignalAddress.TouchingForehead;
					break;
				case MuseAddress.DELTARELATIVE:
					parsedAddress = SignalAddress.Delta_Rel;
					break;
				case MuseAddress.GYRO:
					parsedAddress = SignalAddress.Gyro;
					break;
				case MuseAddress.ACCELERATION:
					parsedAddress = SignalAddress.Acceleration;
					break;
				case MuseAddress.BLINK:
					parsedAddress = SignalAddress.Blink;
					break;
				case MuseAddress.JAWCLENCH:
					parsedAddress = SignalAddress.JawClench;
					break;
				case MuseAddress.ALPHAABSOLUTE:
					parsedAddress = SignalAddress.Alpha_Abs;
					break;
				case MuseAddress.BETAABSOLUTE:
					parsedAddress = SignalAddress.Beta_Abs;
					break;
				case MuseAddress.THETAABSOLUTE:
					parsedAddress = SignalAddress.Theta_Abs;
					break;
				case MuseAddress.DELTAABSOLUTE:
					parsedAddress = SignalAddress.Delta_Abs;
					break;
				case MuseAddress.GAMMAABSOLUTE:
					parsedAddress = SignalAddress.Gamma_Abs;
					break;
				case MuseAddress.ALPHARELATIVE:
					parsedAddress = SignalAddress.Alpha_Rel;
					break;
				case MuseAddress.BETARELATIVE:
					parsedAddress = SignalAddress.Beta_Rel;
					break;
				case MuseAddress.THETARELATIVE:
					parsedAddress = SignalAddress.Theta_Rel;
					break;
				case MuseAddress.GAMMARELATIVE:
					parsedAddress = SignalAddress.Gamma_Rel;
					break;
				case MuseAddress.ALHPASESSIONSCORE:
					parsedAddress = SignalAddress.Alpha_Session_Score;
					break;
				case MuseAddress.BETASESSIONSCORE:
					parsedAddress = SignalAddress.Beta_Session_Score;
					break;
				case MuseAddress.DELTASESSIONSCORE:
					parsedAddress = SignalAddress.Delta_Session_Score;
					break;
				case MuseAddress.THETASESSIONSCORE:
					parsedAddress = SignalAddress.Theta_Session_Score;
					break;
				case MuseAddress.GAMMASESSIONSCORE:
					parsedAddress = SignalAddress.Gamma_Session_Score;
					break;
				case MuseAddress.ISGOOD:
					parsedAddress = SignalAddress.IsGood;
					break;
				case MuseAddress.HORSEHOE:
					parsedAddress = SignalAddress.Horsehoe;
					break;
				case MuseAddress.DRLREF:
					parsedAddress = SignalAddress.Drlref;
					break;
				case MuseAddress.BATTERY:
					parsedAddress = SignalAddress.Battery;
					break;
				default:
					parsedAddress = SignalAddress.Unknown;
					break;
			};

			return parsedAddress;
		}
		#endregion
	}
}
