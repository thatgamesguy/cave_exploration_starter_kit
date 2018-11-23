using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Raised when the player is placed in the level.
	/// </summary>
	public class PlayerPlacedInLevelEvent : GameEvent
	{
		private Transform player;

		/// <summary>
		/// The players transform.
		/// </summary>
		/// <value>The player.</value>
		public Transform Player { get { return player; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="CaveExploration.PlayerSpawnedEvent"/> class.
		/// </summary>
		/// <param name="player">Player.</param>
		public PlayerPlacedInLevelEvent (Transform player)
		{
			this.player = player;
		}
	}

}
