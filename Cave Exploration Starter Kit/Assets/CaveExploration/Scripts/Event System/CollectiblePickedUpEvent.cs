using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	public class CollectiblePickedUpEvent : GameEvent
	{
		private float lightAmount;
		public float LightAmount { get { return lightAmount; } }

		private float radiusAmount;
		public float RadiusAmount { get { return radiusAmount; } }

		public CollectiblePickedUpEvent (float lightAmount, float radiusAmount)
		{
			this.lightAmount = lightAmount;
			this.radiusAmount = radiusAmount;
		}
	}
}
