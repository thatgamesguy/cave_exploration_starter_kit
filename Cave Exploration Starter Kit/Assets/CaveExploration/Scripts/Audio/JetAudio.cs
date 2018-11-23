using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Handles playing audio on jet pack use.
	/// </summary>
	[RequireComponent (typeof(AudioPlayer))]
	public class JetAudio : MonoBehaviour
	{
		/// <summary>
		/// The audio clip to play when the jetpack is in use.
		/// </summary>
		public AudioClip JetAudioClip;

		/// <summary>
		/// The volume to play the jet pack audio clip.
		/// </summary>
		public float Volume = 0.2f;

		private AudioPlayer audioPlayer;
		private Jetpack jetpack;

		void Awake ()
		{
			audioPlayer = GetComponent<AudioPlayer> ();
			jetpack = GetComponent<Jetpack> ();
		}
	
		// Update is called once per frame
		void FixedUpdate ()
		{
			if (jetpack.UsingJet) {
				audioPlayer.PlaySound (JetAudioClip, Volume);
			}
		}

	
	}
}
