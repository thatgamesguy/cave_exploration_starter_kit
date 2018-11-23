using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CaveExploration
{
	/// <summary>
	/// Responsible for spawning enemies at level start.
	/// </summary>
	public class EnemySpawner : MonoBehaviour
	{
		public enum EnemyPlacementType
		{
			RandomBackgroundTile,
			RandomFloorTile,
			RandomTileWithRectSpaceAround,
			RandomTileWithCircleSpaceAround
		}

		/// <summary>
		/// The size of the space required around the enemy when using RandomTileWithRectSpaceAround.
		/// </summary>
		public Vector2 RectWidthHeght = new Vector2 (0.7f, 0.5f);

		/// <summary>
		/// The size of the space required around the enemy when using RandomTileWithCircleSpaceAround.
		/// </summary>
		public float CircleRadius = 0.25f;

		/// <summary>
		/// Apply random rotation to enemy when spawned.
		/// </summary>
		public bool RandomRotation = false;

		/// <summary>
		/// The enemy prefab to spawn.
		/// </summary>
		public GameObject Enemy;

		/// <summary>
		/// The maximum number of enemies of this type to spawn.
		/// </summary>
		public int MaxEnemies = 10;

		/// <summary>
		/// The enemy placement type. Please see Read Me for more information on placement types.
		/// </summary>
		public EnemyPlacementType PlacementType = EnemyPlacementType.RandomBackgroundTile;

		private GameObject parentContainer;
		private List<Vector2> usedCoords;
		private int enemiesPlaced = 0;

		void Awake ()
		{
			if (!Enemy) {
				Debug.LogError ("No enemy prefab set, disabling script");
				enabled = false;
			}

			parentContainer = new GameObject ("Enemies");
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
		/// Handles the level generated event. Places the enemies based on placement type.
		/// </summary>
		/// <param name="e">E.</param>
		public void OnLevelGenerated (GameEvent e)
		{
			if (Utilities.instance.IsDebug)
				Debug.Log ("Placing Enemies");

			enemiesPlaced = 0;
			
			switch (PlacementType) {
			case EnemyPlacementType.RandomBackgroundTile:
				RandomTilePlacement ();
				break;
			case EnemyPlacementType.RandomFloorTile:
				OnFloorPlacement ();
				break;
			case EnemyPlacementType.RandomTileWithRectSpaceAround:
				RandomTileWithRectSpaceAroundPlacement ();
				break;
			case EnemyPlacementType.RandomTileWithCircleSpaceAround:
				RandomTileWithCircleSpaceAroundPlacement ();
				break;
			}

			Events.instance.Raise (new EnemiesPlaced (enemiesPlaced));
		}

		private void SetRandomRotation (GameObject enemy)
		{
			var rot = enemy.transform.rotation;
			var randomRot = Random.rotation;

			enemy.transform.rotation = new Quaternion (rot.x, rot.y, randomRot.z, rot.w);
		}

		private void RandomTilePlacement ()
		{
		
			usedCoords = new List<Vector2> ();
		
			for (int i = 0; i < MaxEnemies; i++) {
				var node = GridManager.instance.GetRandomBackgroundNode ();
			
				if (usedCoords.Contains (node.Coordinates)) {
					i--; 
					continue;
				}
			
				usedCoords.Add (node.Coordinates);
			
				var pos = Utilities.instance.GetNodePosition (node);
			
				var enemy = ObjectManager.instance.GetObject (Enemy.name, pos, false);

				if (RandomRotation) {
					SetRandomRotation (enemy);
				}
			
				enemy.transform.SetParent (parentContainer.transform);

				enemiesPlaced++;
			}
		}

		private void OnFloorPlacement ()
		{
			usedCoords = new List<Vector2> ();
			
			for (int i = 0; i < MaxEnemies; i++) {
				var node = GridManager.instance.GetRandomFloorNode ();

				if (node == null)
					return;

				usedCoords.Add (node.Coordinates);
				
				var pos = Utilities.instance.GetNodePosition (node);
	
				var enemy = ObjectManager.instance.GetObject (Enemy.name, pos, false);

				if (RandomRotation) {
					SetRandomRotation (enemy);
				}

				enemy.transform.SetParent (parentContainer.transform, true);
				
				enemiesPlaced++;
			}
		}

		private void RandomTileWithRectSpaceAroundPlacement ()
		{
			var nodes = GridManager.instance.GetBackgroundNodes ();

			foreach (var node in nodes) {
				var pos = Utilities.instance.GetNodePosition (node);

				var topLeft = new Vector2 (pos.x - RectWidthHeght.x * 0.5f, pos.y + RectWidthHeght.y * 0.5f);
				var bottomRight = new Vector2 (pos.x + RectWidthHeght.x * 0.5f, pos.y - RectWidthHeght.y * 0.5f);
				
				if (Physics2D.OverlapArea (topLeft, bottomRight, 1 << LayerMask.NameToLayer ("Cave") | 1 << LayerMask.NameToLayer ("Enemy"))) {
					continue;
				}

				var enemy = ObjectManager.instance.GetObject (Enemy.name, pos, false);

				if (RandomRotation) {
					SetRandomRotation (enemy);
				}
				
				enemy.transform.SetParent (parentContainer.transform);
				
				enemiesPlaced++;

				if (enemiesPlaced == MaxEnemies) {
					return;
				}
			}
	
		}

		private void RandomTileWithCircleSpaceAroundPlacement ()
		{
			var nodes = GridManager.instance.GetBackgroundNodes ();
			
			foreach (var node in nodes) {
				var pos = Utilities.instance.GetNodePosition (node);

				if (Physics2D.OverlapCircle (pos, CircleRadius, 1 << LayerMask.NameToLayer ("Cave") | 1 << LayerMask.NameToLayer ("Enemy"))) {
					continue;
				}
				
				var enemy = ObjectManager.instance.GetObject (Enemy.name, pos, false);

				if (RandomRotation) {
					SetRandomRotation (enemy);
				}
				
				enemy.transform.SetParent (parentContainer.transform);
				
				enemiesPlaced++;
				
				if (enemiesPlaced == MaxEnemies) {
					return;
				}
			}

		}

	}
}
