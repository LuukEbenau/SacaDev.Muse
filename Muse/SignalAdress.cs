using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muse
{
	/// <summary>
	/// All available message adresses
	/// </summary>
	[Flags]
	public enum SignalAddress
	{
		Unknown					= 0b0,
		//Sensors
		Acceleration			= 0b1,
		Gyro					= 0b10,
		Battery					= 0b100,
		//Status
		TouchingForehead		= 0b1000,
		Horsehoe				= 0b10000,
		IsGood					= 0b100000,
		//values
		Drlref					= 0b1000000,
		Eeg						= 0b10000000,
		NotchFilteredEeg		= 0b100000000,
		//Raw
		Alpha_Abs				= 0b1000000000,
		Alpha_Rel				= 0b10000000000,
		Beta_Abs				= 0b100000000000,
		Beta_Rel				= 0b1000000000000,
		Delta_Abs				= 0b10000000000000,
		Delta_Rel				= 0b100000000000000,
		Theta_Abs				= 0b1000000000000000,
		Theta_Rel				= 0b10000000000000000,
		Gamma_Abs				= 0b100000000000000000,
		Gamma_Rel				= 0b1000000000000000000,
		//Session scores
		Beta_Session_Score		= 0b10000000000000000000,
		Delta_Session_Score		= 0b100000000000000000000,
		Gamma_Session_Score		= 0b1000000000000000000000,
		Theta_Session_Score		= 0b10000000000000000000000,
		Alpha_Session_Score		= 0b100000000000000000000000,
		//Other
		JawClench				= 0b1000000000000000000000000,
		Blink					= 0b10000000000000000000000000,
		//Groups
		SensorData = Acceleration | Gyro | Battery,
		RawData = Alpha_Abs | Alpha_Rel | Beta_Abs | Beta_Rel | Delta_Abs | Delta_Rel | Theta_Abs | Theta_Rel | Gamma_Abs | Gamma_Rel,
		SessionScores = Beta_Session_Score | Delta_Session_Score | Gamma_Session_Score | Theta_Session_Score | Alpha_Session_Score,
		MuseStatus = TouchingForehead | Horsehoe | IsGood,

		All = SensorData | RawData | SessionScores | MuseStatus | Drlref | Eeg | NotchFilteredEeg | JawClench | Blink,
	}
}
