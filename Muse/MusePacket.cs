using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Muse
{
	/// <summary>
	/// Packet format received by the muse
	/// </summary>
	public class MusePacket
	{
		/// <summary>
		/// Name of the muse
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Package identifying address
		/// </summary>
		public SignalAddress Address { get; }
		/// <summary>
		/// Values send to the address
		/// </summary>
		public ICollection<double> Values { get; set; }

		public MusePacket(string name, SignalAddress address, IEnumerable<double> values)
		{
			this.Name = name;
			this.Address = address;
			this.Values = values.ToList();
		}
	}
}
