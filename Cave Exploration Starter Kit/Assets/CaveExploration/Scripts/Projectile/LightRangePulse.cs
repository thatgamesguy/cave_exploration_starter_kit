using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Pulses a light range between two values over time.
	/// </summary>
	public class LightRangePulse : LightPulse
	{
		void Update ()
		{
			_light.range = Minimum + Mathf.PingPong (Time.time * Speed, Maximum - Minimum);
		}
	}
}
