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
		public ICollection<double> Values { get; set; }
		public MusePacket(string name, SignalAddress address, IEnumerable<double> values)
		{
			this.Name = name;
			this.Address = address;
			this.Values = values.ToList();
		}
	}
}
