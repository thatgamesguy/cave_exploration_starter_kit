using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Checks whether the player is colliding on the sides with a wall tile.
	/// </summary>
	[RequireComponent (typeof(BoxCollider2D))]
	public class SideCheck : MonoBehaviour
	{
		/// <summary>
		/// The ground mask.
		/// </summary>
		public LayerMask GroundMask;

		/// <summary>
		/// The force to apply when a player walks into a wall.
		/// </summary>
		public float KnockbackForce = 200f;

		private Rigidbody2D playerRigidbody;
		private Player player;
		private BottomCheck groundCheck;

		void Awake ()
		{
			playerRigidbody = transform.parent.GetComponent<Rigidbody2D> ();
			player = transform.parent.GetComponent<Player> ();
			groundCheck = transform.parent.GetComponentInChildren<BottomCheck> ();
		}
	
		void OnTriggerEnter2D (Collider2D other)
		{
			if (groundCheck.IsGrounded || player.IsDead)
				return;

			if (other.gameObject.layer == LayerMask.NameToLayer ("Cave")) {

				var hitX = (player.IsFacingRight) ? -1 : 1;

				playerRigidbody.AddForce (new Vector2 (hitX * KnockbackForce, 0));
			}
		}



	}
}
