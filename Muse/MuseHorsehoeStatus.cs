using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SacaDev.Muse
{
	public class MuseHorsehoeStatus
	{
		public MuseElectrodeStatus[] Statusses { get; }
		public bool AllElectrodesConnected { get; private set; }
		public event EventHandler<bool> AllElectrodesConnectedChanged;
		public MuseHorsehoeStatus() {
			Statusses = new MuseElectrodeStatus[] { 0, 0, 0, 0 };
			this.AllElectrodesConnected = false;
		}

		public event EventHandler StatusChanged;

		public void Update(double[] statusses) {
			if (statusses.Length != Statusses.Length) {
				Console.WriteLine($"invalid amount of statusses received: only {statusses.Length} instead of {Statusses.Length}, corrupt data?");
			}

			for (int i = 0; i < statusses.Length; i++) {
				var oldval = this.Statusses[i];
				var newVal = (MuseElectrodeStatus)statusses[i];
				if (oldval != newVal) {
					//status updated
					this.Statusses[i] = newVal;
					StatusChanged?.Invoke(this, new EventArgs());
				}

				var allElectrodesConnected = !this.Statusses.Any(s => s != MuseElectrodeStatus.Good);
				if (allElectrodesConnected != AllElectrodesConnected) {
					this.AllElectrodesConnected = allElectrodesConnected;
					AllElectrodesConnectedChanged?.Invoke(this, allElectrodesConnected);
				}
			}
		}
	}
}
