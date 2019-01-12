using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Muse.Test
{
	[TestFixture]
	public class MuseTest
	{
		private Muse Muse { get; set; }
		[SetUp]
		public void Setup() {
			Muse = new Muse("jantje", TestConstants.TEST_PORT);
		}
		[TearDown]
		public void TearDown() {
			Muse?.Dispose();
		}
		#region tests to verify subscription altering methods are working properly
		[Test]
		public void Connect_SubscriptionsSetCorrectly_Test() {
			var subscriptions = SignalAddress.Alpha_Rel | SignalAddress.Battery | SignalAddress.SensorData;
			Muse.Connect(subscriptions);

			Assert.IsTrue(Muse.Subscriptions == subscriptions);
		}

		[Test]
		public void SetSubscriptions_Test() {
			var subscriptions = SignalAddress.Alpha_Rel | SignalAddress.Battery | SignalAddress.SensorData;
			Muse.SetSubscriptions(subscriptions);
			Assert.IsTrue(Muse.Subscriptions == subscriptions);
		}

		[Test]
		public void AddSubscriptions_Test() {
			var subscriptions = SignalAddress.Alpha_Rel | SignalAddress.Battery | SignalAddress.SensorData;

			Muse.AddSubscriptions(subscriptions);

			Assert.IsTrue(Muse.Subscriptions == subscriptions);

			Muse.AddSubscriptions(SignalAddress.Acceleration);

			Assert.IsTrue(Muse.Subscriptions == (subscriptions | SignalAddress.Acceleration));
		}

		[Test]
		public void RemoveSubscriptions_Single_Test() {
			var initialSubs = Muse.Subscriptions;

			Muse.AddSubscriptions(SignalAddress.Battery);

			Muse.RemoveSubscriptions(SignalAddress.Battery);

			Assert.IsTrue(Muse.Subscriptions == initialSubs);
		}

		[Test]
		public void RemoveSubscriptions_Multiple_Test()
		{
			var initialSubs = Muse.Subscriptions;

			var subsToSubscribeTo = SignalAddress.Battery | SignalAddress.JawClench | SignalAddress.TouchingForehead | SignalAddress.NotchFilteredEeg;
			Muse.AddSubscriptions(subsToSubscribeTo);

			Muse.RemoveSubscriptions(SignalAddress.TouchingForehead);

			Assert.IsTrue(Muse.Subscriptions == (initialSubs | SignalAddress.Battery | SignalAddress.JawClench | SignalAddress.NotchFilteredEeg));
		}
		#endregion
	}
}
