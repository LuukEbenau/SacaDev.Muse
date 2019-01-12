using SharpOSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Muse
{
	public class Muse : IDisposable
	{
		public string Alias { get; }
		public string Name { get; private set; }
		public int Port { get; }

		public bool IsConnected { get; private set; }
		public bool IsTouchingForehead { get; private set; }

		public SignalAddress Subscriptions { get; private set; }

		private UDPListener _listener;
		public event EventHandler<MusePacket> PacketReceived;

		public Muse(string alias, int port) {
			this.Alias = alias;
			this.Port = port;
		}
		/// <summary>
		/// Adds aditional subscriptions to the already active subscriptions
		/// </summary>
		public SignalAddress AddSubscriptions(SignalAddress subscriptionFlags) => Subscriptions = Subscriptions | subscriptionFlags;
		/// <summary>
		/// Removes the given subscriptions from the excisting subscriptions
		/// </summary>
		public SignalAddress RemoveSubscriptions(SignalAddress subscriptionFlags) => Subscriptions = Subscriptions & ~subscriptionFlags;
		/// <summary>
		/// Replace the excisting subsciptions with a new set of subscriptions
		/// </summary>
		public SignalAddress SetSubscriptions(SignalAddress subscriptionFlags) => Subscriptions = subscriptionFlags;

		private SignalAddress ParseSignalAdressFromMessageAdress(string address) {
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

		private MusePacket ParsePacket(OscMessage message) {
			var signalAddress = ParseSignalAdressFromMessageAdress(message.Address);
			return new MusePacket(Name, signalAddress, message.Arguments.Select(a => (a as IConvertible).ToDecimal(null) ) );
		}
		/// <summary>
		/// start listening to the port of the connected muse
		/// </summary>
		/// <param name="subscriptionFlags">The subscribed flags; can be seperated with a |</param>
		public void Connect(SignalAddress subscriptionFlags) {
			this.Subscriptions = subscriptionFlags;
			this.IsConnected = true;
			this._listener = new UDPListener(Port, OnPacketReceived);
		}
		public void Connect() => Connect(SignalAddress.All);

		private void OnPacketReceived(OscPacket packet)
		{
			try
			{
				var message = (OscMessage)packet;

				//automatically detect name from the first packet received
				if (Name == null){
					Name = message.Address.Substring(0, message.Address.IndexOf('/'));
				}

				var musePacket = ParsePacket(message);
				if (musePacket.Address == SignalAddress.Unknown)
				{
					Console.WriteLine($"unknown packet with address '{message.Address}', skipping it...");
					return;
				}
				if (musePacket.Address == SignalAddress.TouchingForehead) {
					var firstVal = musePacket.Values.FirstOrDefault();

					IsTouchingForehead = firstVal > 0;
				}
				//only send data if subscribed to the given packet type
				if (Subscriptions.HasFlag(musePacket.Address))
				{
					PacketReceived?.Invoke(this, musePacket);
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Corrupt packet received");
			}
		}

		public void Disconnect() {
			_listener?.Dispose();
			_listener = null;
			IsConnected = false;
		}

		public void Dispose()
		{
			Disconnect();
		}
	}
}
