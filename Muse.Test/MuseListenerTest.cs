using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using OscParser;

namespace Muse.Test
{
	[TestFixture]
	public class MuseListenerTest
	{
		private const string MuseName = "Sacation";

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
		}
		[SetUp]
		public void Setup()
		{

		}
		[TearDown]
		public void Teardown() {

		}

		[OneTimeTearDown]
		public void OneTimeTeardown() {
			
		}

		[TestCase(MuseAddress.BETARELATIVE, SignalAddress.Beta_Rel)]
		[TestCase(MuseAddress.ACCELERATION, SignalAddress.Acceleration)]
		[TestCase(MuseAddress.ALPHAABSOLUTE, SignalAddress.Alpha_Abs)]
		[TestCase(MuseAddress.NOTCHFILTEREDEEG, SignalAddress.NotchFilteredEeg)]
		[TestCase(MuseAddress.BLINK, SignalAddress.Blink)]
		[TestCase(MuseAddress.TOUCHINGFOREHEAD, SignalAddress.TouchingForehead)]
		public void PacketParse_Test(string stringAddress, SignalAddress expectedResult) {
			var packet = new OscMessage(MuseName + stringAddress);
			using (var listener = new MuseListener(TestConstants.TEST_PORT))
			{
				var parsedPacket = listener.ParsePacket(packet);

				Assert.IsTrue(parsedPacket.Address == expectedResult);
			}
		}
		[Test]
		public void PacketParse_ValueParse_Test() {
			var (val1, val2, val3, val4, val5) = (1.2d, 1.1d, 1.112d, 1.32d, 0.03d);

			var stringAddress = MuseAddress.BETARELATIVE;

			var packet = new OscMessage(MuseName + stringAddress, val1, val2, val3, val4, val5);

			using (var listener = new MuseListener(TestConstants.TEST_PORT))
			{
				var parsedPacket = listener.ParsePacket(packet);
				
				Assert.IsTrue(parsedPacket.Values.Contains(val1));
				Assert.IsTrue(parsedPacket.Values.Contains(val2));
				Assert.IsTrue(parsedPacket.Values.Contains(val3));
				Assert.IsTrue(parsedPacket.Values.Contains(val4));
				Assert.IsTrue(parsedPacket.Values.Contains(val5));
			}
		}
	}
}
