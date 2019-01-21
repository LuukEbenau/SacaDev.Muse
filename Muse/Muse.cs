using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SacaDev.Muse
{
	public class Muse : IDisposable
	{
		#region Properties, Events and Fields
		/// <summary>
		/// Alias given to this muse
		/// </summary>
		public string Alias { get; }
		/// <summary>
		/// Actual name of the muse, determined by the muse itself(more strictly speaking, muse Direct)
		/// </summary>
		public string Name { get; private set; }
		public int Port { get; }
		public MuseBatteryInfo Battery { get; } = new MuseBatteryInfo();
		public MuseHorsehoeStatus Status { get; } = new MuseHorsehoeStatus();

		public event EventHandler<bool> IsTouchingForeheadChanged;
		public event EventHandler<bool> IsConnectedChanged;

		private bool _isConnected;
		//TODO: think about if the current way of setting this is sufficient, or that there needs to be some kind of timeout detection (x amount of inactive seconds or something)  since it doesn't detect when losing connection.
		public bool IsConnected {
			get => _isConnected;
			private set {
				if (_isConnected != value)
				{
					_isConnected = value;
					IsConnectedChanged?.Invoke(this, value);
				}
			}
		}

		private bool _isTouchingForehead;
		public bool IsTouchingForehead {
			get => _isTouchingForehead;
			private set {
				if (_isTouchingForehead != value)
				{
					_isTouchingForehead = value;
					IsTouchingForeheadChanged?.Invoke(this, value);
				}
			} 
		}

		public bool AllElectrodesConnected;
		public event EventHandler<bool> AllElectrodesConnectedChanged;
		/// <summary>
		/// all subscribed signal flags
		/// </summary>
		public SignalAddress Subscriptions { get; private set; }

		private MuseListener _listener;
		/// <summary>
		/// gets triggered when a packet is received from the muse
		/// </summary>
		public event EventHandler<MusePacket> PacketReceived;
		#endregion

		public Muse(string alias, int port) {
			this.Alias = alias;
			this.Port = port;
			this.Status.AllElectrodesConnectedChanged += Status_AllElectrodesConnectedChanged;
		}

		#region subscriptions
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
		#endregion

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
		public void Connect() => Connect(SignalAddress.All);
		private void Status_AllElectrodesConnectedChanged(object sender, bool e)
		{
			this.AllElectrodesConnected = e;
			AllElectrodesConnectedChanged?.Invoke(sender, e);
		}
		private void _listener_PacketReceived(object sender, MusePacket musePacket)
		{
			try {
				switch (musePacket.Address) {
					case SignalAddress.TouchingForehead:
						var touchingForehead = musePacket.Values.FirstOrDefault() == 1;
						if (IsTouchingForehead != touchingForehead) 
							IsTouchingForehead = touchingForehead;
						break;
					case SignalAddress.Battery:
						this.Battery.Update(musePacket.Values.Select(v => (int)v).ToArray());
						break;
					case SignalAddress.Horsehoe:
						this.Status.Update(musePacket.Values.ToArray());
						break;
				}

				//only send data if subscribed to the given packet type
				if (Subscriptions.HasFlag(musePacket.Address))
					PacketReceived?.Invoke(this, musePacket);
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
