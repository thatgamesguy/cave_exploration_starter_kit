using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Used to pulse a light variable.
	/// </summary>
	[RequireComponent (typeof(Light))]
	public class LightPulse : MonoBehaviour
	{
		/// <summary>
		/// The speed to pulse variable.
		/// </summary>
		public float Speed = 10f;

		/// <summary>
		/// The minimum value.
		/// </summary>
		public float Minimum = 1f;

		/// <summary>
		/// The maximum value.
		/// </summary>
		public float Maximum = 8f;

		protected Light _light;

		void Awake ()
		{
			_light = GetComponent<Light> ();
		}

	}
}
