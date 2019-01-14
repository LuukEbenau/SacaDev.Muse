using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using OscParser;
using System.Linq;

namespace Muse
{
	public class MuseListener:IDisposable
	{
		private UdpClient Client { get; set; }
		private int Port { get; }
		public event EventHandler<MusePacket> PacketReceived;

		public void StopListening() {
			this.Client?.Dispose();//should make client cancel the Receive operation,//CHECKME: is this the right way to go?
			this.Client = null;
		}

		public void StartListening() {
			StopListening();
			this.Client = new UdpClient(Port);
			
			Task.Run(() => {
				var endpoint = new IPEndPoint(IPAddress.Any, Port);
				try
				{
					//TODO: this is an initial version, i feel like this can be done more elegant? (atleast async, not thread blocking like now)
					while (true)
					{
						try
						{
							byte[] bytes = Client.Receive(ref endpoint);
							var message = OscPacket.GetPacket(bytes) as OscMessage;

							var musePacket = ParsePacket(message);
							if (musePacket.Address == SignalAddress.Unknown)
							{
								Console.WriteLine($"unknown packet with address '{message.Address}', skipping it...");
								continue;
							}

							PacketReceived?.Invoke(this, musePacket);
						}
						catch (ObjectDisposedException) {
							Console.WriteLine("disposed early");
						}
						catch (Exception ex)
						{
							Console.WriteLine($"Corrupt packet received: '{ex.Message}'");
						}
					}
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

		private SignalAddress ParseSignalAdressFromMessageAdress(string address)
		{
			var firstSlashIndex = address.IndexOf('/');
			var absAdressPart = address.Substring(firstSlashIndex);

			SignalAddress parsedAddress;
			switch (absAdressPart)
			{
				case "/notch_filtered_eeg":
					parsedAddress = SignalAddress.NotchFilteredEeg;
					break;
				case "/eeg":
					parsedAddress = SignalAddress.Eeg;
					break;
				case "/elements/touching_forehead":
					parsedAddress = SignalAddress.TouchingForehead;
					break;
				case "/elements/delta_relative":
					parsedAddress = SignalAddress.Delta_Rel;
					break;
				case "/gyro":
					parsedAddress = SignalAddress.Gyro;//filter out
					break;
				case "/acc":
					parsedAddress = SignalAddress.Acceleration;
					break;
				case "/elements/blink":
					parsedAddress = SignalAddress.Blink;
					break;
				case "/elements/jaw_clench":
					parsedAddress = SignalAddress.JawClench;
					break;
				case "/elements/alpha_absolute":
					parsedAddress = SignalAddress.Alpha_Abs;
					break;
				case "/elements/beta_absolute":
					parsedAddress = SignalAddress.Beta_Abs;
					break;
				case "/elements/theta_absolute":
					parsedAddress = SignalAddress.Theta_Abs;
					break;
				case "/elements/delta_absolute":
					parsedAddress = SignalAddress.Delta_Abs;
					break;
				case "/elements/gamma_absolute":
					parsedAddress = SignalAddress.Gamma_Abs;
					break;
				case "/elements/alpha_relative":
					parsedAddress = SignalAddress.Alpha_Rel;
					break;
				case "/elements/beta_relative":
					parsedAddress = SignalAddress.Beta_Rel;
					break;
				case "/elements/theta_relative":
					parsedAddress = SignalAddress.Theta_Rel;
					break;
				case "/elements/gamma_relative":
					parsedAddress = SignalAddress.Gamma_Rel;
					break;
				case "/elements/alpha_session_score":
					parsedAddress = SignalAddress.Alpha_Session_Score;
					break;
				case "/elements/beta_session_score":
					parsedAddress = SignalAddress.Beta_Session_Score;
					break;
				case "/elements/delta_session_score":
					parsedAddress = SignalAddress.Delta_Session_Score;
					break;
				case "/elements/theta_session_score":
					parsedAddress = SignalAddress.Theta_Session_Score;
					break;
				case "/elements/gamma_session_score":
					parsedAddress = SignalAddress.Gamma_Session_Score;
					break;
				case "/elements/is_good":
					parsedAddress = SignalAddress.IsGood;
					break;
				case "/elements/horseshoe":
					parsedAddress = SignalAddress.Horsehoe;
					break;
				case "/drlref":
					parsedAddress = SignalAddress.Drlref;
					break;
				case "/batt":
					parsedAddress = SignalAddress.Battery;
					break;
				default:
					parsedAddress = SignalAddress.Unknown;
					break;
			};

			return parsedAddress;
		}

		private MusePacket ParsePacket(OscMessage message)
		{
			var signalAddress = ParseSignalAdressFromMessageAdress(message.Address);
			string name = message.Address.Substring(0, message.Address.IndexOf('/'));
			return new MusePacket(name, signalAddress, message.Arguments.Select(a => (a as IConvertible).ToDouble(null)));
		}

		public void Dispose()
		{
			Client?.Dispose();
		}
	}
}
