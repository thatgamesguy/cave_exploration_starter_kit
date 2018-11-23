using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Attach to an explosion. Kills an enemy in explosion radius.
	/// </summary>
	[RequireComponent (typeof(AudioPlayer))]
	public class Explosion : MonoBehaviour
	{
		/// <summary>
		/// The force used to knockback objects in radius.
		/// </summary>
		public float ExplosionForce = 25f;

		/// <summary>
		/// The audio clip to play on explosion.
		/// </summary>
		public AudioClip ExplosionClip;

		private AudioPlayer _audio;

		void Awake ()
		{
			_audio = GetComponent<AudioPlayer> ();
		}

		/// <summary>
		/// Plays the explosion sound.
		/// </summary>
		public void PlayExplosionSound ()
		{
			_audio.PlaySound (ExplosionClip, 1f);
		}

		/// <summary>
		/// Returns the explosion to the object pool. Called by animation.
		/// </summary>
		public void ReturnToPool ()
		{
			ObjectManager.instance.RemoveObject (gameObject);
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.CompareTag ("Enemy")) {
				var enemy = other.GetComponent<EnemyHealth> ();
				
				if (enemy) {
					enemy.Kill (transform.position, ExplosionForce);
				}
			}
		}
	}
}
