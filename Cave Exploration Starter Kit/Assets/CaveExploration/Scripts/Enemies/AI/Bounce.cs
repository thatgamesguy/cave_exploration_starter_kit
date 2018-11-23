using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// AI for the "bounce" enemy. Handles animation, audio and AI states.
	/// </summary>
	[RequireComponent (typeof(Animator))]
	[RequireComponent (typeof(AudioPlayer))]
	public class Bounce : EnemyAI
	{
		/// <summary>
		/// The audio clip to play when enemy activates.
		/// </summary>
		public AudioClip PlayerNearbyAudio;

		private Animator _animator;
		private int attackingHash = Animator.StringToHash ("attacking");
		private AudioPlayer _audio;
		private bool playerNearby = false;

		void Awake ()
		{
			_animator = GetComponent<Animator> ();
			_audio = GetComponent<AudioPlayer> ();
		}

		protected override void OnEnable ()
		{
			base.OnEnable ();
			playerNearby = false;
			_animator.SetBool (attackingHash, false);
		}

		/// <summary>
		/// Plays the audio clip when enemy activates.
		/// </summary>
		public void PlayNearbyAudio ()
		{
			_audio.PlaySound (PlayerNearbyAudio, 1f, Random.Range (0.6f, 1.6f));
		}
	
		protected override void DecideState ()
		{
			state = (playerNearby) ? AIState.PlayerInSight : AIState.Idle;
		}

		protected override void Idle ()
		{
			_animator.SetBool (attackingHash, false);
		}

		protected override void AttackPlayer ()
		{
			_animator.SetBool (attackingHash, true);
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.gameObject.CompareTag ("Player")) {
				playerNearby = true;
			}
		}

		void OnTriggerExit2D (Collider2D other)
		{
			if (other.gameObject.CompareTag ("Player")) {
				playerNearby = false;
			}
		}
	}
}
