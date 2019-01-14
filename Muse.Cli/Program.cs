using System;
using Muse;

namespace Muse.Cli
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			var manager = new MuseManager();
			manager.MusePacketReceived += Manager_MusePacketReceived;
			manager.Connect("jantje", 7000);

			//var listener = new MuseListener(7000);
			//listener.PacketReceived += Listener_PacketReceived;
			Console.Read();
		}

		private static void Manager_MusePacketReceived(object sender, MusePacket e)
		{
			Console.WriteLine($"{e.Name} send an packet with address '{e.Address}', containing:");
			foreach (var val in e.Values)
				Console.Write($"{val}, ");
			Console.WriteLine();
		}
	}
}
