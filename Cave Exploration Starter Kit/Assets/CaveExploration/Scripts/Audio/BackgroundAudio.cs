using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Plays a background audio clip on start.
	/// </summary>
	[RequireComponent (typeof(AudioPlayer))]
	public class BackgroundAudio : MonoBehaviour
	{
		/// <summary>
		/// The background audio tracks. A random track (with no bias) is selected and played.
		/// </summary>
		public AudioClip[] BackgroundAudioTracks;

		/// <summary>
		/// The volume to play background audio.
		/// </summary>
		public float Volume = 0.5f;

		private AudioPlayer _audio;


		void Awake ()
		{
			_audio = GetComponent<AudioPlayer> ();

			if (BackgroundAudioTracks == null || BackgroundAudioTracks.Length == 0) {
				this.enabled = false;
			}
		}

		void Start ()
		{
			_audio.PlaySound (BackgroundAudioTracks [Random.Range (0, BackgroundAudioTracks.Length)], Volume, true);
		}


	}
}
