using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Pulses a lights intensity between two values over time.
	/// </summary>
	public class LightIntensityPulse : LightPulse
	{
		void Update ()
		{
			_light.intensity = Minimum + Mathf.PingPong (Time.time * Speed, Maximum - Minimum);
		}
	}
}
