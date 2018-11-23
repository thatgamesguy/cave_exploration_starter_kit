using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Handles playing audio clips, includes methods to handle volume, pitch, looped status, and playing songs forwards/backwards.
	/// </summary>
	[RequireComponent (typeof(AudioSource))]
	public class AudioPlayer : MonoBehaviour
	{
		private AudioSource _audioSource;

		void Awake ()
		{
			_audioSource = GetComponent<AudioSource> ();
			_audioSource.playOnAwake = false;
			_audioSource.spatialBlend = 0;
		}

		/// <summary>
		/// Determines whether this instance is playing an audio clip.
		/// </summary>
		/// <returns><c>true</c> if this instance is playing; otherwise, <c>false</c>.</returns>
		public bool IsPlaying ()
		{
			return _audioSource.isPlaying;
		}

		/// <summary>
		/// plays the sound clip at the specified volume.
		/// </summary>
		/// <param name="clip">Clip.</param>
		/// <param name="volume">Volume.</param>
		public void PlaySound (AudioClip clip, float volume)
		{
			PlaySound (clip, volume, Random.Range (0.9f, 1.2f));
		}

		/// <summary>
		/// plays the sound clip at the specified volume.
		/// </summary>
		/// <param name="clip">Clip.</param>
		/// <param name="volume">Volume.</param>
		/// <param name="looped">If set to <c>true</c> the clip is looped.</param>
		public void PlaySound (AudioClip clip, float volume, bool looped)
		{
			_audioSource.loop = looped;
			PlaySound (clip, volume, Random.Range (0.9f, 1.2f));
		}

		/// <summary>
		/// plays the sound clip at the specified volume and pitch.
		/// </summary>
		/// <param name="clip">Clip.</param>
		/// <param name="volume">Volume.</param>
		/// <param name="pitch">Pitch.</param>
		public void PlaySound (AudioClip clip, float volume, float pitch)
		{
			if (pitch >= 0) {
				PlaySoundForward (clip, volume, pitch);
			} else {
				PlaySoundReverse (clip, volume, pitch);
			}
		}

		private void PlaySoundForward (AudioClip clip, float volume, float pitch)
		{
			_audioSource.volume = volume;
			_audioSource.clip = clip;
			_audioSource.pitch = pitch;
			_audioSource.Play ();
		}

		private void PlaySoundReverse (AudioClip clip, float volume, float pitch)
		{
			_audioSource.volume = volume;
			_audioSource.clip = clip;
			_audioSource.pitch = pitch;
			_audioSource.timeSamples = _audioSource.clip.samples - 1;
			_audioSource.Play ();
		}

	
	}
}
