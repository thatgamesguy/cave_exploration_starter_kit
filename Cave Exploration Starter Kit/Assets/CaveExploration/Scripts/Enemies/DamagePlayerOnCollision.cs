using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Attach to any object to damage the player when a collision with this object occurs.
	/// </summary>
	[RequireComponent (typeof(Collider2D))]
	public class DamagePlayerOnCollision : MonoBehaviour
	{
		/// <summary>
		/// The damage amount.
		/// </summary>
		public int DamageAmount = 1;

		/// <summary>
		/// The damage knockback force.
		/// </summary>
		public float DamageForce = 50f;

		void OnCollisionStay2D (Collision2D other)
		{
			if (other.gameObject.CompareTag ("Player")) {
				var heading = other.transform.position - transform.position;
				var distance = heading.magnitude;
				var direction = heading / distance;
			
				Events.instance.Raise (new PlayerDamagedEvent (DamageAmount, direction, DamageForce));
			}
		}

	}
}
