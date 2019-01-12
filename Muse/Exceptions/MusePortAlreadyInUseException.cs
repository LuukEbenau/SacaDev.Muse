using System;
using System.Collections.Generic;
using System.Text;

namespace Muse
{
	public class MuseAliasAlreadyInUseException:MuseException
	{
		public MuseAliasAlreadyInUseException() : base("There's already a muse connected with the same alias!") { }
	}
}
