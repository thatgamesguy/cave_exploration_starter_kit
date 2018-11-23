using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// AI for the "ShootPlayer" enemy. Handles animation, audio, and AI states.
	/// </summary>
	[RequireComponent (typeof(Animator))]
	[RequireComponent (typeof(AudioPlayer))]
	public class ShootPlayer : EnemyAI
	{
		/// <summary>
		/// The maximum movement speed.
		/// </summary>
		public float MoveSpeed = 1f;

		/// <summary>
		/// The force used to shoot projectiles towards player.
		/// </summary>
		public float ProjectileForce = 1f;

		/// <summary>
		/// The projectile prefab.
		/// </summary>
		public GameObject Projectile;

		/// <summary>
		/// The projectile spawn location relative to enemy.
		/// </summary>
		public Transform ProjectileSpawnLocation;

		/// <summary>
		/// Determines whether enemy is facing right. Used to flip sprite to face movement direction.
		/// </summary>
		public bool IsFacingRight = true;

		/// <summary>
		/// Audio clip to play on shoot.
		/// </summary>
		public AudioClip ShootAudioClip;

		private int direction = 1;
		private Animator _animator;
		private static readonly int attackingHash = Animator.StringToHash ("attacking");
		private BobSprite spriteBob;
		private AudioPlayer _audio;

		void Awake ()
		{
			spriteBob = GetComponent<BobSprite> ();
			_animator = GetComponent<Animator> ();
			_audio = GetComponent<AudioPlayer> ();

			if (!Projectile) {
				Debug.LogError ("Projectile not set, disabling script");
				enabled = false;
			}
		}

		protected override void DecideState ()
		{
			state = (PlayerNearby ()) ? AIState.PlayerInSight : AIState.Idle;
		}

		protected override void Idle ()
		{
			spriteBob.Enabled = true;
			_animator.SetBool (attackingHash, false);
			var moveDir = new Vector2 (direction, 0) * MoveSpeed;
			transform.Translate (moveDir * Time.deltaTime);
		}

		protected override void AttackPlayer ()
		{
			//spriteBob.Enabled = false;
			_animator.SetBool (attackingHash, true);

		}

		/// <summary>
		/// Shoots the projectile in direction of player.
		/// </summary>
		public void ShootProjectile ()
		{
			var projectile = ObjectManager.instance.GetObject (Projectile.name, ProjectileSpawnLocation.position);

			var projRigidbody = projectile.GetComponent<Rigidbody2D> ();

			if (!projRigidbody) {
				Debug.LogError ("Projectile should have Rigidbody2D attached");
				return;
			}

			var heading = player.position - transform.position;
			var distance = heading.magnitude;
			var direction = heading / distance;

			projRigidbody.AddForce (direction * ProjectileForce);

			_audio.PlaySound (ShootAudioClip, 1f);
		}

		private void Flip ()
		{
			IsFacingRight = !IsFacingRight;
			var theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer ("Cave") || 
				other.gameObject.layer == LayerMask.NameToLayer ("Enemy")) {
				direction = -direction;
				Flip ();
			}
		}
	}
}
