using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Muse
{
	public class MusePacket
	{
		public string Name { get; }
		public SignalAddress Address { get; }
		public ICollection<decimal> Values { get; set; }
		public MusePacket(string museName, SignalAddress address, IEnumerable<decimal> values)
		{
			this.Name = Name;
			this.Address = address;
			this.Values = values.ToList();
		}
	}
}
