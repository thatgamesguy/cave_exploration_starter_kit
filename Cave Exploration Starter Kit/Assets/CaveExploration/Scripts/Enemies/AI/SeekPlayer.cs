using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// AI for the "SeekPlayer" enemy. Handles animation, audio, and AI states.
	/// </summary>
	[RequireComponent (typeof(Rigidbody2D))]
	[RequireComponent (typeof(Animator))]
	[RequireComponent (typeof(AudioPlayer))]
	public class SeekPlayer : EnemyAI
	{
		/// <summary>
		/// The burst force to use when seeking player.
		/// </summary>
		public float MoveBurstForce = 1f;

		/// <summary>
		/// The maximum speed at which the enemy can rotate towards player.
		/// </summary>
		public float RotationSpeed = 0.5f;

		/// <summary>
		/// The time between movement bursts.
		/// </summary>
		public float TimeBetweenMovementBursts = 0.2f;

		/// <summary>
		/// The maximum movement force.
		/// </summary>
		public float MaxForce = 2f;

		/// <summary>
		/// Audio clip to play on enemy movement.
		/// </summary>
		public AudioClip MoveAudio;

		private float currentTime = 0f;
		private Rigidbody2D _rigidbody2D;
		private Animator _animator;
		private AudioPlayer _audio;

		void Awake ()
		{
			state = AIState.Idle;

			_rigidbody2D = GetComponent<Rigidbody2D> ();
			_animator = GetComponent<Animator> ();
			_audio = GetComponent<AudioPlayer> ();
		}


		protected override void Idle ()
		{

		}

		protected override void AttackPlayer ()
		{
			currentTime += Time.deltaTime;

			var heading = transform.position - player.position;
			float angle = Mathf.Atan2 (heading.y, heading.x) * Mathf.Rad2Deg;
			Quaternion q = Quaternion.AngleAxis (angle, Vector3.forward);
			transform.rotation = Quaternion.Slerp (transform.rotation, q, Time.deltaTime * RotationSpeed);
			
			if (!PlayerInSight ()) {
				_animator.SetBool ("turning", true);
				return;
			}
			
			if (currentTime >= TimeBetweenMovementBursts) {
				currentTime = 0f;
				_animator.SetTrigger ("moveBurst");
			} 
		}
	
		public override void Update ()
		{
			_animator.SetBool ("turning", false);

			base.Update ();
		}

		/// <summary>
		/// Burst moves towards the player. Called by animation.
		/// </summary>
		public void BurstMoveTowardsPlayer ()
		{
			_rigidbody2D.AddForce ((player.position - transform.position).normalized * (MoveBurstForce * Time.deltaTime), ForceMode2D.Impulse);
			_audio.PlaySound (MoveAudio, 0.5f);
		}

		protected override void DecideState ()
		{
			state = (PlayerNearby ()) ? AIState.PlayerInSight : AIState.Idle;
		}


		private bool PlayerInSight ()
		{
			var distance = (player.position - transform.position).magnitude;

			Ray2D ray = new Ray2D (transform.position, -transform.right);

			if (Utilities.instance.IsDebug)
				Debug.DrawRay (ray.origin, ray.direction, Color.white);

			var hit = Physics2D.Raycast (ray.origin, ray.direction, distance, 1 << LayerMask.NameToLayer ("Player"));

			return hit.collider != null;
		}

		void OnCollisionEnter2D (Collision2D other)
		{
			if (other.gameObject.CompareTag ("Player")) {
				var heading = other.transform.position - transform.position;
				var distance = heading.magnitude;
				var direction = heading / distance;
		
				_rigidbody2D.AddForce (-direction * 0.1f, ForceMode2D.Impulse);
			}
		}

	}
}