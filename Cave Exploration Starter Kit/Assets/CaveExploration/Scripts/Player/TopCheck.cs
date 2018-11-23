using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Checks whether the player is colliding on the top with a wall tile.
	/// </summary>
	[RequireComponent (typeof(Collider2D))]
	public class TopCheck : MonoBehaviour
	{
		/// <summary>
		/// The ground mask.
		/// </summary>
		public LayerMask GroundMask;

		/// <summary>
		/// The force to apply when a player collides with a wall.
		/// </summary>
		public float KnockbackForce = 20f;

		/// <summary>
		/// The particle.
		/// </summary>
		public ParticleSystem particle;

		private Rigidbody2D playerRigidbody;
		private Player player;
		private BottomCheck groundCheck;
		private Jetpack jetpack;

		private const int hitY = -1;

		void Awake ()
		{
			playerRigidbody = transform.parent.GetComponent<Rigidbody2D> ();
			player = transform.parent.GetComponent<Player> ();
			jetpack = transform.parent.GetComponentInChildren<Jetpack> ();
			groundCheck = transform.parent.GetComponentInChildren<BottomCheck> ();
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (groundCheck.IsGrounded || player.IsDead)
				return;

			if (other.gameObject.layer == LayerMask.NameToLayer ("Cave")) {
				if (jetpack)
					jetpack.CanJet = false;

				if (playerRigidbody.velocity.y > 0.5f)
					particle.Emit (7); 

				playerRigidbody.AddForce (new Vector2 (0, hitY * KnockbackForce));
			}
		}

		void OnTriggerExit2D (Collider2D other)
		{
			if (player.IsDead || !jetpack)
				return;

			if (other.gameObject.layer == LayerMask.NameToLayer ("Cave")) {
				jetpack.CanJet = true;
			}
		}
	
	}
}
