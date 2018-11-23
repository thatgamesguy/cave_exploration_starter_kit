using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	[RequireComponent (typeof(AudioPlayer))]
	public class PlayerAudio : MonoBehaviour
	{
		/// <summary>
		/// Audio to be played on player taking a step.
		/// </summary>
		public AudioClip[] StepClips;

		/// <summary>
		/// Audio to be played on player falling.
		/// </summary>
		public AudioClip FallingAudio;

		/// <summary>
		/// Audio to be played on player jumping.
		/// </summary>
		public AudioClip JumpAudio;

		/// <summary>
		/// Audio to be played on player hurt.
		/// </summary>
		public AudioClip HurtAudio;

		/// <summary>
		/// Audio to be played on player spawned.
		/// </summary>
		public AudioClip SpawnSound;

		private AudioPlayer audioPlayer;
		private Player player;
		private BottomCheck groundCheck;
		private Rigidbody2D _rigidbody2D;


		void Awake ()
		{
			audioPlayer = GetComponent<AudioPlayer> ();
			_rigidbody2D = GetComponent<Rigidbody2D> ();
			player = GetComponent<Player> ();
			groundCheck = GetComponentInChildren<BottomCheck> ();
		}

		/// <summary>
		/// Plays random clip on player step.
		/// </summary>
		public void PlayStepAudio ()
		{
			var stepSound = StepClips [Random.Range (0, StepClips.Length)];
			audioPlayer.PlaySound (stepSound, 0.8f);
		}


		/// <summary>
		/// Plays sound when player damaged.
		/// </summary>
		public void OnPlayerDamaged ()
		{
			audioPlayer.PlaySound (HurtAudio, 0.6f);
		}

		/// <summary>
		/// Plays spawn sound.
		/// </summary>
		public void PlaySpawnSound ()
		{
			audioPlayer.PlaySound (SpawnSound, Random.Range (0.6f, 1.2f));
		}
	
		void FixedUpdate ()
		{
			if (!groundCheck.IsGrounded && _rigidbody2D.velocity.y < -1f) {
				var volume = _rigidbody2D.velocity.y * -0.02f;
				var pitch = 1.5f + (_rigidbody2D.velocity.y * 0.05f);
				audioPlayer.PlaySound (FallingAudio, volume, pitch);
			} else if (player.HasJumped) {
				audioPlayer.PlaySound (JumpAudio, 0.6f);
			}
		}



	}
}
