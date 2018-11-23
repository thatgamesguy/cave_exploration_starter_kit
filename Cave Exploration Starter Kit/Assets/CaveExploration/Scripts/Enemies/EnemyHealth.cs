using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Handles enemy health and death (including audio and animation).
	/// </summary>
	[RequireComponent (typeof(SpriteRenderer))]
	[RequireComponent  (typeof(Rigidbody2D))]
	[RequireComponent (typeof(AudioPlayer))]
	public class EnemyHealth : MonoBehaviour
	{
		/// <summary>
		/// The duration of the death animation.
		/// </summary>
		public float DeathDuration = 0.5f;

		/// <summary>
		/// Counts towards number of enemies killed.
		/// </summary>
		public bool CountTowardsEnemyCount = true;

		/// <summary>
		/// The sound played on death.
		/// </summary>
		public AudioClip SoundOnDeath;

		private SpriteRenderer _renderer;
		private Rigidbody2D _rigidbody2D;
		private float? startTime;
		private AudioPlayer _audio;

		void Awake ()
		{
			_renderer = GetComponent<SpriteRenderer> ();
			_rigidbody2D = GetComponent<Rigidbody2D> ();
			_audio = GetComponent<AudioPlayer> ();
		}

		void OnEnable ()
		{
			startTime = null;
			_renderer.color = Color.white;
		}

		void Update ()
		{
			if (startTime.HasValue) {
				float t = (Time.time - startTime.Value) / DeathDuration;
				_renderer.color = new Color (Mathf.SmoothStep (1f, 0f, t), 
				                             Mathf.SmoothStep (1f, 0f, t), 
				                             Mathf.SmoothStep (1f, 0f, t),
				                             1f);  

				if (_renderer.color.r == 0f) {
					ReturnToPool ();
				}
			}
		}

		/// <summary>
		/// Kill the specified enemy and applys knockback force.
		/// </summary>
		/// <param name="projectilePosition">Projectile position.</param>
		/// <param name="force">Force.</param>
		public void Kill (Vector3 projectilePosition, float force)
		{
			_audio.PlaySound (SoundOnDeath, 0.8f);
			startTime = Time.time;
			RepelFromPositionWithForce (projectilePosition, force);
		}

		private void ReturnToPool ()
		{
			if (CountTowardsEnemyCount) {
				Events.instance.Raise (new EnemyKilled ());
			}

			ObjectManager.instance.RemoveObject (gameObject);
		}

		private void RepelFromPositionWithForce (Vector3 position, float force)
		{
			var heading = transform.position - position;
			var distance = heading.magnitude;
			var direction = heading / distance;
			_rigidbody2D.AddForce (direction * force);

		}
	}
}