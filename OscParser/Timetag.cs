using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OscParser
{
	public struct Timetag
	{
		public UInt64 Tag;

		public DateTime Timestamp
		{
			get
			{
				return Utils.TimetagToDateTime(Tag);
			}
			set
			{
				Tag = Utils.DateTimeToTimetag(value);
			}
		}

		public Timetag(UInt64 value)
		{
			this.Tag = value;
		}

		public override bool Equals(System.Object obj)
		{
			if (obj.GetType() == typeof(Timetag))
			{
				if (this.Tag == ((Timetag)obj).Tag)
					return true;
				else
					return false;
			}
			else if (obj.GetType() == typeof(UInt64))
			{
				if (this.Tag == ((UInt64)obj))
					return true;
				else
					return false;
			}
			else
				return false;
		}

		public override int GetHashCode()
		{
			return (int)( ((uint)(Tag >> 32) + (uint)(Tag & 0x00000000FFFFFFFF)) / 2);
		}
	}
}
