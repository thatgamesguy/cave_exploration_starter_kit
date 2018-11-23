using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Raised when the player completes a level.
	/// </summary>
	public class LevelCompleteEvent : GameEvent
	{
		private int nextSeed;

		/// <summary>
		/// Gets the seed to be used when generating the next level.
		/// </summary>
		/// <value>The next seed.</value>
		public int NextSeed { get { return nextSeed; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="CaveExploration.LevelCompleteEvent"/> class.
		/// </summary>
		/// <param name="nextSeed">Next seed.</param>
		public LevelCompleteEvent (int nextSeed)
		{
			this.nextSeed = nextSeed;
		}
	}
}
