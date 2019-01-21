using System;
using System.Collections.Generic;
using System.Text;

namespace SacaDev.Muse
{
	public class MuseBatteryInfo
	{
		private double LastBatteryLevelTreshhold { get; set; }
		/// <summary>
		/// Percentage of power left, ranges between 0.00 and 100.00
		/// </summary>
		public double PercentsPowerLeft { get; private set; }
		/// <summary>
		/// Fuel Gauge Battery Voltage, measured in mV (milivolts), values between 3000 - 4200
		/// </summary>
		public int FuelGaugeBatteryVoltage { get; private set; }
		/// <summary>
		/// ADC Battery Voltage, measured in mV (milivolts), values between 3000 - 4200
		/// </summary>
		public int AdcBatteryBatteryVoltage { get; private set; }

		public event EventHandler<double> BatteryLevelDecreased;

		/// <summary>
		/// Temperature of the battery, measured in degrees Celcius (C)
		/// Values range between -40 and +125C
		/// </summary>
		public int Temperature { get; private set; }
		/// <summary>
		/// Updates the battery info using the received data in the format from the muse.
		/// </summary>
		/// <param name="values">array containing the 4 int values received by the muse</param>
		public void Update(int[] values) {
			var percentPowerLeft = values[0] / 100d;
			this.PercentsPowerLeft = percentPowerLeft;
			if (LastBatteryLevelTreshhold == default)
				this.LastBatteryLevelTreshhold = percentPowerLeft;

			double batteryLevelUpdateInterval = 2.0d;
			if (Math.Abs(this.PercentsPowerLeft - LastBatteryLevelTreshhold) > batteryLevelUpdateInterval) {
				this.LastBatteryLevelTreshhold -= batteryLevelUpdateInterval;
				this.BatteryLevelDecreased?.Invoke(this, this.LastBatteryLevelTreshhold);
			}

			this.FuelGaugeBatteryVoltage = values[1];
			this.AdcBatteryBatteryVoltage = values[2];
			this.Temperature = values[3];
		}

	}
}
