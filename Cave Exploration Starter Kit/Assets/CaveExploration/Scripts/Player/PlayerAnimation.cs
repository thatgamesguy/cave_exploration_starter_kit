using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Responsible for updating the players animation based on movement.
	/// </summary>
	[RequireComponent (typeof(Rigidbody2D))]
	[RequireComponent (typeof(Player))]
	[RequireComponent (typeof(Animator))]
	public class PlayerAnimation : MonoBehaviour
	{
		private int walkingHash = Animator.StringToHash ("walkSpeed");
		private int jetFastHash = Animator.StringToHash ("jetSpeed");
		private int jumpingHash = Animator.StringToHash ("jumping");

		private Animator _animator;
		private Player _player;
		private Jetpack _jetpack;
		private BottomCheck groundCheck;
		private bool isSpawning = true;

		/// <summary>
		/// Gets a value indicating whether this instance is spawning.
		/// </summary>
		/// <value><c>true</c> if this instance is spawning; otherwise, <c>false</c>.</value>
		public bool IsSpawning { get { return isSpawning; } }

		private bool eventRaised = false;

		/// <summary>
		/// Sets spawning as finished.
		/// </summary>
		public void FinishedSpawning ()
		{
			isSpawning = false;

			if (!eventRaised) {
				Events.instance.Raise (new PlayerSpawnedEvent (transform));
				eventRaised = true;
			}
		}

		void Awake ()
		{
			_player = GetComponent<Player> ();
			_animator = GetComponent<Animator> ();
			_jetpack = GetComponentInChildren<Jetpack> ();
			groundCheck = GetComponentInChildren<BottomCheck> ();
		}

		void OnEnable ()
		{
			ResetAnimation ();
			isSpawning = true;
			eventRaised = false;
		}

		private void ResetAnimation ()
		{
			_animator.SetFloat (walkingHash, 0);
			_animator.SetFloat (jetFastHash, 0);
			_animator.SetBool (jumpingHash, false);
			_animator.speed = 1;
		}
	
		void LateUpdate ()
		{
			if (isSpawning) {
				return;
			}

			ResetAnimation ();

			if (IsJumping ()) {
				HandleJumpAnimation ();
			} else if (IsOnGround ()) {
				HandleGroundedJetAnimation ();
			} else if (IsUsingJetPack ()) {
				HandleJetAnimation ();
			} 
		}

		private void HandleJetAnimation ()
		{
			_animator.SetFloat (jetFastHash, _player.MoveSpeed);

		}


		private void HandleGroundedJetAnimation ()
		{
			var moveSpeed = _player.MoveSpeed;
			
			if (moveSpeed > 0.1) {
				_animator.speed = moveSpeed * 1.2f;
			} else {
				_animator.speed = 1;
			}
			_animator.SetFloat (walkingHash, moveSpeed);
		}

		private void HandleJumpAnimation ()
		{
			_animator.SetBool (jumpingHash, true);
		}

		private bool IsUsingJetPack ()
		{
			return _jetpack != null && _jetpack.UsingJet;
		}

		private bool IsOnGround ()
		{
			return groundCheck.IsGrounded;
		}

		private bool IsJumping ()
		{
			return _player.HasJumped;
		}

	
	}
}
