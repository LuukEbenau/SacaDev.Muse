using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Muse
{
	public class Muse : IDisposable
	{
		/// <summary>
		/// Alias given to this muse
		/// </summary>
		public string Alias { get; }
		/// <summary>
		/// Actual name of the muse, determined by the muse itself(more strictly speaking, muse Direct)
		/// </summary>
		public string Name { get; private set; }
		public int Port { get; }

		public bool IsConnected { get; private set; }
		public bool IsTouchingForehead { get; private set; }

		/// <summary>
		/// all subscribed signal flags
		/// </summary>
		public SignalAddress Subscriptions { get; private set; }

		private MuseListener _listener;
		/// <summary>
		/// gets triggered when a packet is received from the muse
		/// </summary>
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

		/// <summary>
		/// start listening to the port of the connected muse
		/// </summary>
		/// <param name="subscriptionFlags">The subscribed flags; can be seperated with a |</param>
		public void Connect(SignalAddress subscriptionFlags) {
			this.Subscriptions = subscriptionFlags;
			this.IsConnected = true;
			this._listener = new MuseListener(Port);
			this._listener.PacketReceived += _listener_PacketReceived;
		}

		private void _listener_PacketReceived(object sender, MusePacket musePacket)
		{
			try { 
				if (musePacket.Address == SignalAddress.TouchingForehead)
				{
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

		public void Connect() => Connect(SignalAddress.All);

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
