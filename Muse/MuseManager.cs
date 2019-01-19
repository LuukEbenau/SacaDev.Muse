using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SacaDev.Muse
{
	/// <summary>
	/// Manager of all muses
	/// </summary>
	public class MuseManager : IDisposable
	{
		private ICollection<Muse> Muses { get; }

		public event EventHandler<MusePacket> MusePacketReceived;

		public MuseManager() {
			Muses = new List<Muse>();
		}

		#region subscription management
		public void SetSubscriptions(string alias, SignalAddress subscriptionFlags) => _SetSubscriptions(Muses.Where(m => m.Alias == alias), subscriptionFlags);
		public void SetSubscriptions(SignalAddress subscriptionFlags) => _SetSubscriptions(Muses, subscriptionFlags);
		private void _SetSubscriptions(IEnumerable<Muse> targetMuses, SignalAddress subscriptionFlags)
		{
			foreach (var muse in targetMuses)
			{
				muse.SetSubscriptions(subscriptionFlags);
			}
		}

		public void AddSubscriptions(string alias, SignalAddress subscriptionFlags) => _AddSubscriptions(Muses.Where(m => m.Alias == alias), subscriptionFlags);
		public void AddSubscriptions(SignalAddress subscriptionFlags) => _AddSubscriptions(Muses, subscriptionFlags);
		private void _AddSubscriptions(IEnumerable<Muse> targetMuses, SignalAddress subscriptionFlags)
		{
			foreach (var muse in targetMuses)
			{
				muse.AddSubscriptions(subscriptionFlags);
			}
		}

		public void RemoveSubscriptions(string alias, SignalAddress subscriptionFlags) => _RemoveSubscriptions(Muses.Where(m => m.Alias == alias), subscriptionFlags);
		public void RemoveSubscriptions(SignalAddress subscriptionFlags) => _RemoveSubscriptions(Muses, subscriptionFlags);
		private void _RemoveSubscriptions(IEnumerable<Muse> targetMuses, SignalAddress subscriptionFlags)
		{
			foreach (var muse in targetMuses)
			{
				muse.RemoveSubscriptions(subscriptionFlags);
			}
		}
		#endregion

		#region connection management
		/// <summary>
		/// Connect to a muse with the given name and port
		/// </summary>
		/// <param name="port">portnumber to listen to</param>
		/// <param name="alias">the alias you want to give this muse</param>
		/// <exception cref="MuseException">When something goes whrong with connection</exception>
		/// <exception cref="MusePortAlreadyInUseException">When the given port is already in use</exception>
		/// <exception cref="MuseAliasAlreadyInUseException">When the manager is already connected with an muse with the same alias</exception>
		public void Connect(string alias, int port)
		{
			if (Muses.Any(m => m.Alias == alias))
				throw new MuseAliasAlreadyInUseException();
			if (Muses.Any(m => m.Port == port))
				throw new MusePortAlreadyInUseException();

			var muse = new Muse(alias, port);
			muse.Connect();
			Muses.Add(muse);

			muse.PacketReceived += (object sender, MusePacket e)
				=> MusePacketReceived?.Invoke(sender, e);
		}



		/// <summary>
		/// Closes the connection to the muse with the given alias, and removes it
		/// </summary>
		/// <param name="alias">alias of the muse</param>
		public void CloseConnection(string alias) {
			var muse = Muses.FirstOrDefault(m => m.Alias == alias);
			if (muse != null)
			{
				CloseConnection(muse);
			}
		}

		/// <summary>
		/// Closes the connection to the muse with the given name, and removes it
		/// </summary>
		/// <param name="muse">the muse to close the connection from</param>
		private void CloseConnection(Muse muse)
		{
			if (!Muses.Contains(muse))
				return;

			muse.Dispose();
			Muses.Remove(muse);
		}

		/// <summary>
		/// Closes all connections to the muses
		/// </summary>
		public void CloseConnections()
		{
			foreach (var muse in Muses.ToList())
			{
				CloseConnection(muse);
			}
		}
		#endregion

		public void Dispose()
		{
			CloseConnections();
		}
	}
}
