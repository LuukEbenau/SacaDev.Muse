using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacaDev.Muse.Test
{
	[TestFixture]
	public class MuseManagerTest
	{
		private MuseManager Manager { get; set; }
		[SetUp]
		public void Setup() {
			Manager = new MuseManager();
		}
		[TearDown]
		public void TearDown() {
			Manager?.Dispose();
		}
		#region connecting tests
		[Test]
		public void Connect_DoubleAlias_Test() {
			Manager.Connect("Jantje1", TestConstants.TEST_PORT);

			Assert.Throws(typeof(MuseAliasAlreadyInUseException), () => {
				Manager.Connect("Jantje1", TestConstants.TEST_PORT + 1);
			});
			
		}

		[Test]
		public void Connect_PortOccupied_Test() {
			Manager.Connect("Jantje1", TestConstants.TEST_PORT);

			Assert.Throws(typeof(MusePortAlreadyInUseException), () => {
				Manager.Connect("Jantje2", TestConstants.TEST_PORT);
			});

		}

		[Test]
		public void CloseConnection_Single_Test() {
			string alias = "Jantje";
			Manager.Connect(alias, TestConstants.TEST_PORT);

			Manager.CloseConnection(alias);

			Assert.DoesNotThrow(() =>
			{
				Manager.Connect(alias, TestConstants.TEST_PORT);
			});
		}

		[Test]
		public void CloseConnection_OnlyOneOutOfMultipleGetsRemoved_Test()
		{
			string alias = "Jantje";
			Manager.Connect(alias, TestConstants.TEST_PORT);
			Manager.Connect(alias + "1", TestConstants.TEST_PORT + 1);

			Manager.CloseConnection(alias + "1");

			Assert.DoesNotThrow(() =>
			{
				Manager.Connect(alias + "1", TestConstants.TEST_PORT + 1);
			});
			Assert.Throws(typeof(MuseAliasAlreadyInUseException), () =>
			{
				Manager.Connect(alias, TestConstants.TEST_PORT + 2);
			});
		}

		[Test]
		public void CloseConnections_Test()
		{
			string alias = "Jantje";
			Manager.Connect(alias, TestConstants.TEST_PORT);
			Manager.Connect(alias + "1", TestConstants.TEST_PORT + 1);

			Manager.CloseConnections();

			Assert.DoesNotThrow(() =>
			{
				Manager.Connect(alias, TestConstants.TEST_PORT);
				Manager.Connect(alias + "1", TestConstants.TEST_PORT + 1);
			});
		}
		#endregion
		//TODO: We need to create a test for the event 'Manager.MusePacketReceived', which is actually the most important thing to test. the problem is it needs some more work, since it is listening to the udp port, and private methods can't been tested.
	}
}
