using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CaveExploration
{
	/// <summary>
	/// Spawns collectibles on round start.
	/// </summary>
	public class CollectibleSpawner : MonoBehaviour
	{
		/// <summary>
		/// The collectible prefab.
		/// </summary>
		public GameObject Collectible;

		/// <summary>
		/// The number of surrounding walls required to spawn collectible.
		/// </summary>
		public int CollectibleWallLimit = 4;

		/// <summary>
		/// The chance a collectible will spawn when a suitable location is found.
		/// </summary>
		[Range (0, 1)]
		public float
			SpawnChance = 0.1f;

		/// <summary>
		/// The maximum number of collectibles to be spawned in any one level.
		/// </summary>
		public int MaxCollectibles = 40;

		private int currentCollectibles = 0;
		private GameObject parentContainer;
			
		void Start ()
		{
			parentContainer = new GameObject ("Collectibles");
		}
	
		void OnEnable ()
		{
			Events.instance.AddListener<LevelGeneratedEvent> (OnLevelGenerated);
			Events.instance.AddListener<LevelGeneratedSpeechRequired> (OnLevelGenerated);
		}
		
		void OnDisable ()
		{
			Events.instance.RemoveListener<LevelGeneratedEvent> (OnLevelGenerated);
			Events.instance.RemoveListener<LevelGeneratedSpeechRequired> (OnLevelGenerated);
		}
		
		/// <summary>
		/// Handles the level generated event. Places collectibles.
		/// </summary>
		/// <param name="e">E.</param>
		public void OnLevelGenerated (GameEvent e)
		{
			if (Utilities.instance.IsDebug)
				Debug.Log ("Placing Collectibles");
				
			currentCollectibles = 0;
			PlaceInitialCollectibles ();
		}

		private void ShuffleList (List<Node> nodes)
		{
			for (int i = 0; i < nodes.Count; i++) {
				var temp = nodes [i];
				int randomIndex = Random.Range (i, nodes.Count);
				nodes [i] = nodes [randomIndex];
				nodes [randomIndex] = temp;
			}
		}

	
		private void PlaceInitialCollectibles ()
		{
			var nodes = GridManager.instance.GetBackgroundNodes ();

			ShuffleList (nodes);

			foreach (var node in nodes) {
				int wallCount = GridManager.instance.CountWallMooreNeighbours (node.Coordinates);

				if (wallCount > CollectibleWallLimit && Random.Range (0f, 1f) < SpawnChance) {
					
					if (++currentCollectibles > MaxCollectibles)
						return;
					
					var position = Utilities.instance.GetNodePosition (node);
					
					var collectible = ObjectManager.instance.GetObject (Collectible.name, position);
					
					collectible.transform.SetParent (parentContainer.transform);

				}

			}
		}
	}
}
