using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Manages the game scene. Starts level, player, and collectible placement/creation. Handles player dead and level complete events.
	/// </summary>
	public class DirectorFullGame : MonoBehaviour
	{
		/// <summary>
		/// The seed used to generate the level. The same seed and generation settings will always generate the same level.
		/// </summary>
		public int Seed = 1;

		/// <summary>
		/// The player prefab.
		/// </summary>
		public GameObject PlayerPrefab;

		/// <summary>
		/// The end collectible prefab. 
		/// </summary>
		public GameObject EndCollectiblePrefab;

		void Start ()
		{
			CreateLevel (true);
		}

		void Update ()
		{
			if (Utilities.instance.IsDebug) {
				if (Input.GetKeyDown (KeyCode.N)) {
					Events.instance.Raise (new LevelCompleteEvent (Random.Range (0, 1000000)));
				}
			}
		}

		void OnEnable ()
		{
			Events.instance.AddListener<LevelGeneratedEvent> (OnLevelGenerated);
			Events.instance.AddListener<LevelGeneratedSpeechRequired> (OnLevelGenerated);
			Events.instance.AddListener<PlayerKilledEvent> (OnPlayerDead);
			Events.instance.AddListener<LevelCompleteEvent> (OnLevelComplete);
		}

		void OnDisable ()
		{
			Events.instance.RemoveListener<LevelGeneratedEvent> (OnLevelGenerated);
			Events.instance.RemoveListener<LevelGeneratedSpeechRequired> (OnLevelGenerated);
			Events.instance.RemoveListener<PlayerKilledEvent> (OnPlayerDead);
			Events.instance.RemoveListener<LevelCompleteEvent> (OnLevelComplete);
		}

		private void OnLevelGenerated (GameEvent e)
		{
			SpawnPlayer ();
			PlaceEndCollectible ();
		}

		private void OnPlayerDead (GameEvent e)
		{
			print ("creating level player dead");
			CreateLevel (false);
		}

		private void OnLevelComplete (LevelCompleteEvent e)
		{
			print ("creating level complete");
			Seed = e.NextSeed;
			CreateLevel (false);
		}

		private void CreateLevel (bool firstLevel)
		{
	
			GridManager.instance.GenerateWithSeed (Seed, firstLevel);
		}


		private void SpawnPlayer ()
		{
			if (Utilities.instance.IsDebug)
				Debug.Log ("Spawning Player");
				

			var startNode = GridManager.instance.StartNode;
			var pos = Utilities.instance.GetNodePosition (startNode);
			var player = ObjectManager.instance.GetObject (PlayerPrefab.name, pos);

			Events.instance.Raise (new PlayerPlacedInLevelEvent (player.transform));
		}

		private void PlaceEndCollectible ()
		{
			if (Utilities.instance.IsDebug)
				Debug.Log ("Spawning End Collectible");

			var endNode = GridManager.instance.EndNode;
			var pos = Utilities.instance.GetNodePosition (endNode) + new Vector2 (0, 0.017f);
			ObjectManager.instance.GetObject (EndCollectiblePrefab.name, pos);
		}

	}
}
