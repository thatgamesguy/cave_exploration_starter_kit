using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Attach to camera. Used to follow player once spawned.
	/// </summary>
	public class FollowPlayer : MonoBehaviour
	{
		/// <summary>
		/// The displacement from the player.
		/// </summary>
		public Vector3 Displacement;

		private Transform player;

		void OnEnable ()
		{
			Events.instance.AddListener<PlayerPlacedInLevelEvent> (OnPlayerSpawned);
		}

		void OnDisable ()
		{
			Events.instance.RemoveListener<PlayerPlacedInLevelEvent> (OnPlayerSpawned);
		}

		private void OnPlayerSpawned (PlayerPlacedInLevelEvent e)
		{
			this.player = e.Player;
		}

		void LateUpdate ()
		{
			if (player) {
				transform.position = new Vector3 (player.transform.position.x + Displacement.x, 
			                                 		player.transform.position.y + Displacement.y, 
			                                  		player.transform.position.z + Displacement.z);
			}
		}
	}
}
