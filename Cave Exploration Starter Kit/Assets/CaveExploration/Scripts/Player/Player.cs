using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Handles player movement.
	/// </summary>
	[RequireComponent (typeof(Rigidbody2D))]
	[RequireComponent (typeof(PlayerAnimation))]
	public class Player : MonoBehaviour
	{
		/// <summary>
		/// Determines wether the player is facing right. Used to face the sprite in the movement direction.
		/// </summary>
		public bool IsFacingRight;

		/// <summary>
		/// The maximum walk speed.
		/// </summary>
		public float MaxWalkSpeed = 2f;

		/// <summary>
		/// The maximum run speed.
		/// </summary>
		public float MaxRunSpeed = 4f;

		/// <summary>
		/// The jump force.
		/// </summary>
		public float JumpForce = 500f;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is dead.
		/// </summary>
		/// <value><c>true</c> if this instance is dead; otherwise, <c>false</c>.</value>
		public bool IsDead  { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance has jumped. Used for double jumping.
		/// </summary>
		/// <value><c>true</c> if this instance has jumped; otherwise, <c>false</c>.</value>
		public bool HasJumped { get; set; }

		private float move;
		private float moveSpeed;

		/// <summary>
		/// Gets the current movement speed.
		/// </summary>
		/// <value>The move speed.</value>
		public float MoveSpeed { get { return moveSpeed; } }

		private float maxSpeed;
		private bool isRunning = false;
		private BottomCheck groundCheck;
		private Rigidbody2D rigidbody2d;
		private Jetpack jetpack;
		private PlayerAnimation _animation;
	
		void Awake ()
		{
			groundCheck = GetComponentInChildren<BottomCheck> ();

			if (!groundCheck) {
				Debug.LogError ("Player requires BottomCheck sript on child, disabling script");
				enabled = false;
			}

			rigidbody2d = GetComponent<Rigidbody2D> ();
			jetpack = GetComponentInChildren<Jetpack> ();
			_animation = GetComponent<PlayerAnimation> ();
		}

		void OnEnable ()
		{
			IsDead = false;
			isRunning = false;
			move = 0f;
			moveSpeed = 0f;
			rigidbody2d.velocity = Vector2.zero;

			Events.instance.AddListener<PlayerKilledEvent> (OnDead);
		}

		void OnDisable ()
		{
			Events.instance.RemoveListener<PlayerKilledEvent> (OnDead);
		}
		

		private void OnDead (GameEvent e)
		{
			IsDead = true;
		}
	
		// Update is called once per frame
		void FixedUpdate ()
		{
			if (IsDead || _animation.IsSpawning || IntroductorySpeech.instance.InProgress) {
				move = 0f;
				moveSpeed = 0f;
				return;
			}
	

			// Clamp Move.
			move = Input.GetAxis ("Horizontal");
			if (move != 0) {
				moveSpeed = (moveSpeed < maxSpeed) ? moveSpeed + Mathf.Abs (move) * 0.05f : maxSpeed;
			} else {
				moveSpeed = 0;
			}

			rigidbody2d.velocity = new Vector2 (moveSpeed * move, rigidbody2d.velocity.y);

			// Flip Sprite.
			if ((move > 0 && !IsFacingRight) || (move < 0 && IsFacingRight))
				Flip ();

		}

		void Update ()
		{
			HasJumped = false;

			if (_animation.IsSpawning || IntroductorySpeech.instance.InProgress) {
				IsDead = false;
				return;
			}

			if (IsDead)
				return;


			var isGrounded = groundCheck.IsGrounded;

			// Jump.
			if (isGrounded && Input.GetKeyDown (KeyCode.Space)) {
				rigidbody2d.AddForce (new Vector2 (0, JumpForce));
				HasJumped = true;
				if (jetpack) {
					jetpack.CanJet = false;
				}
			} else if (isGrounded || (jetpack && jetpack.UsingJet)) {
				HasJumped = false;
			} 

			// Run.
			if (isGrounded && Input.GetKey (KeyCode.LeftShift)) {
				maxSpeed = MaxRunSpeed;
				isRunning = true;
			} else {
				isRunning = false;
			}
		
			if (!isRunning && move != 0) {
				if (maxSpeed > MaxWalkSpeed) {
					maxSpeed -= 0.02f;
				} else {
					maxSpeed = MaxWalkSpeed;
				}
			}

		}

		private void Flip ()
		{
			IsFacingRight = !IsFacingRight;
			var theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}
}
