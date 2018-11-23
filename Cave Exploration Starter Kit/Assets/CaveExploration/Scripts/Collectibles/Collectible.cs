using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Attach to collectible in game. Handles seeking player, and giving player a light boost on collection.
	/// </summary>
	[RequireComponent (typeof(Collider2D))]
	public class Collectible : MonoBehaviour
	{
		/// <summary>
		/// The light intensity added to player on collection.
		/// </summary>
		public float LightIntensityAdded = 1f;

		/// <summary>
		/// The light radius added to player on collection.
		/// </summary>
		public float LightRadiusAdded = 0.2f;

		/// <summary>
		/// If collectible within this distance to player, the collectible will move towards players location.
		/// </summary>
		public float SeekDistance = 1;

		/// <summary>
		/// The speed at which collectible moves towards player.
		/// </summary>
		public float SeekSpeed = 1f;

		private Transform player;

		void OnEnable ()
		{
			player = null;
			Events.instance.AddListener<PlayerSpawnedEvent> (OnPlayerSpawned);
		}

		void OnDisable ()
		{
			Events.instance.RemoveListener<PlayerSpawnedEvent> (OnPlayerSpawned);
		}

		void OnPlayerSpawned (PlayerSpawnedEvent e)
		{
			player = e.Player;
		}

		void Update ()
		{
			if (player == null || IntroductorySpeech.instance.InProgress)
				return;


			var distance = (transform.position - player.position).sqrMagnitude;

			if (distance < (SeekDistance * SeekDistance)) {

				var newPos = (player.position - transform.position) * SeekSpeed * Time.deltaTime;

				transform.Translate (newPos, Space.World);

			}
		}


		void OnCollisionEnter2D (Collision2D other)
		{
			if (other.gameObject.CompareTag ("Player")) {
				Events.instance.Raise (new CollectiblePickedUpEvent (LightIntensityAdded, LightRadiusAdded));
				ObjectManager.instance.RemoveObject (gameObject);
			}
		}
	}
}
