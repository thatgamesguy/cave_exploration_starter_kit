using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Responsible to updating enemy projectiles. And applying damage on collision.
	/// </summary>
	public class EnemyProjectile : MonoBehaviour
	{
		/// <summary>
		/// The amount of damage to apply to the player.
		/// </summary>
		public int DamageAmount = 1;

		/// <summary>
		/// The knockback force of the projectile.
		/// </summary>
		public float DamageForce = 50f;

		/// <summary>
		/// The maximum time the projectile can be in the scene. It is added to the pool once this time has been reached.
		/// </summary>
		public float MaxTimeAlive = 2f;

		private float currentTimeAlive;

		void OnEnable ()
		{
			currentTimeAlive = 0f;
		}

		void Update ()
		{
			currentTimeAlive += Time.deltaTime;

			if (currentTimeAlive >= MaxTimeAlive) {
				ReturnToPool ();
			}
		}

		void OnCollisionEnter2D (Collision2D other)
		{
			if (other.transform.CompareTag ("Player")) {
				var heading = other.transform.position - transform.position;
				var distance = heading.magnitude;
				var direction = heading / distance;

				Events.instance.Raise (new PlayerDamagedEvent (DamageAmount, direction, 50f));

				Events.instance.Raise (new LightDecreaseEvent (0.2f, 0.2f));

				ReturnToPool ();
			}
		}

		/// <summary>
		/// Returns to pool. 
		/// </summary>
		public void ReturnToPool ()
		{
			ObjectManager.instance.RemoveObject (gameObject);
		}
	}
}
