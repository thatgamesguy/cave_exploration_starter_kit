using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Used to determine if the player is grounded and apply fall damage.
	/// </summary>
	[RequireComponent (typeof(CircleCollider2D))]
	public class BottomCheck : MonoBehaviour
	{
		/// <summary>
		/// The ground mask.
		/// </summary>
		public LayerMask GroundMask;

		/// <summary>
		/// The height at which the player can fall from and damage applied.
		/// </summary>
		public float FallingHeightToDamage = 2f;

		/// <summary>
		/// The damage applied when fall damage is applied.
		/// </summary>
		public int FallDamage = 1;

		/// <summary>
		/// The particle system used when the player becomes grounded.
		/// </summary>
		public ParticleSystem particle;

		/// <summary>
		/// Gets or sets a value indicating whether the player is grounded.
		/// </summary>
		/// <value><c>true</c> if this instance is grounded; otherwise, <c>false</c>.</value>
		public bool IsGrounded {
			get;
			set;
		}	

		private float radius;
		private ParticleSystem.MinMaxGradient startColour;
		private Rigidbody2D playerRigidbody;
		private float fallHeight = 0f;
		private float hitHeight;
		private bool enter = false;
		private bool velocityRecorded = false;

		void Awake ()
		{
			var collider = GetComponent<CircleCollider2D> ();
			collider.isTrigger = true;
			radius = collider.radius;
			startColour = particle.colorOverLifetime.color;
			playerRigidbody = transform.parent.GetComponent<Rigidbody2D> ();
		}

		void OnEnable ()
		{
			IsGrounded = Physics2D.OverlapCircle (transform.position, radius, GroundMask);
			fallHeight = transform.position.y;
		}

		void Update ()
		{
			if (IsGrounded || velocityRecorded)
				return;

			var velocity = Mathf.Floor (playerRigidbody.velocity.y);

			if (velocity < 0 && velocity > -3) {
				fallHeight = transform.position.y;
				velocityRecorded = true;
			}
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer ("Cave")) {

				if (!enter) {
					enter = true;
					velocityRecorded = false;

					hitHeight = fallHeight - transform.position.y;

					var col = particle.colorOverLifetime;
						
					col.color = startColour;

					if (playerRigidbody.velocity.y < -0.5f) {
						particle.Emit (15);
					}

					if (hitHeight > FallingHeightToDamage && playerRigidbody.velocity.y < -0.8f) {
						Events.instance.Raise (new PlayerDamagedEvent (FallDamage));
					}
				}

				IsGrounded = true;

			}
		}

		void OnTriggerExit2D (Collider2D other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer ("Cave")) {
				hitHeight = 0f;
				fallHeight = transform.position.y;
				enter = false;
				velocityRecorded = false;

				IsGrounded = false;
			}
		}
	}
}