using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Managers the example grid generator scene.
	/// </summary>
	public class DirectorGridGenerator : MonoBehaviour
	{
		/// <summary>
		/// The seed used to generate the level. The same seed and generation settings will always generate the same level.
		/// </summary>
		public int Seed = 1;

		void Start ()
		{
			Events.instance.Raise (new LevelCompleteEvent (Seed));
		}

		void OnEnable ()
		{
			Events.instance.AddListener<LevelCompleteEvent> (OnLevelComplete);
		}

		void OnDisable ()
		{
			Events.instance.RemoveListener<LevelCompleteEvent> (OnLevelComplete);
		}

		private void OnLevelComplete (LevelCompleteEvent e)
		{
			Seed = e.NextSeed;
			CreateLevel (false);
		}

		private void CreateLevel (bool firstLevel)
		{
			GridManager.instance.GenerateWithSeed (Seed, firstLevel);
		}
	
		void Update ()
		{

			if (Input.GetKeyDown (KeyCode.N)) {
				Events.instance.Raise (new LevelCompleteEvent (Random.Range (0, 1000000)));
			}

		}
	}
}
