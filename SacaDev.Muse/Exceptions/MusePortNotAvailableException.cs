using System;
using System.Collections.Generic;
using System.Text;

namespace SacaDev.Muse
{
	public class MusePortAlreadyInUseException:MuseException
	{
		public MusePortAlreadyInUseException() : base("There is already a muse bound to this adress!") { }
	}
}
