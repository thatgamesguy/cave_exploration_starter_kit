using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Raised when the players light should be decreased e.g. on damage.
	/// </summary>
	public class LightDecreaseEvent : GameEvent
	{
		private float? lightAmount;

		/// <summary>
		/// The amount to decrease the players light.
		/// </summary>
		/// <value>The light amount.</value>
		public float? LightAmount { get { return lightAmount; } }
		
		private float? radiusAmount;

		/// <summary>
		/// The amount to decrease the players radius.
		/// </summary>
		/// <value>The radius amount.</value>
		public float? RadiusAmount { get { return radiusAmount; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="CaveExploration.LightDecreaseEvent"/> class.
		/// </summary>
		/// <param name="lightAmount">Amount to decrease the players light intensity.</param>
		/// <param name="radiusAmount">Amount to decrease the players light radius.</param>
		public LightDecreaseEvent (float? lightAmount = null, float? radiusAmount = null)
		{
			this.lightAmount = lightAmount;
			this.radiusAmount = radiusAmount;
		}
	}
}
