using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Added to the exit block for the current level.
	/// When the character enters the blocks collider LevelComplete is set to true.
	/// This is quieried in the Director Class.
	/// </summary>
	public class LevelEnd : MonoBehaviour
	{
		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.CompareTag ("Player")) {
				Events.instance.Raise (new LevelCompleteEvent (Random.Range (0, 1000000)));
			}
		}
		
	}
}
