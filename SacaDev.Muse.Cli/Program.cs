using System;
using SacaDev.Muse;

namespace SacaDev.Muse.Cli
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			var manager = new MuseManager();
			manager.MusePacketReceived += Manager_MusePacketReceived;
			var muse = manager.Connect("jantje", 7000);

			muse.AllElectrodesConnectedChanged += Muse_AllElectrodesConnectedChanged;

			//var listener = new MuseListener(7000);
			//listener.PacketReceived += Listener_PacketReceived;
			Console.Read();
		}

		private static void Muse_AllElectrodesConnectedChanged(object sender, bool e)
		{
			
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
