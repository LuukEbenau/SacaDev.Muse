using System;
using System.Collections.Generic;
using System.Text;

namespace Muse
{
	/// <summary>
	/// Baseclass of all custom exceptions thrown by the muse
	/// </summary>
	public class MuseException:Exception
	{
		public MuseException(string message) : base(message) { }
	}
}
