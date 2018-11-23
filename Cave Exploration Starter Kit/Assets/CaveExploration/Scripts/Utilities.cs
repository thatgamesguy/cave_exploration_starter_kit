using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace CaveExploration
{
	/// <summary>
	/// Singleton. Provides centralised access to methods used by different classes.
	/// </summary>
	public class Utilities : MonoBehaviour
	{
		/// <summary>
		/// Sets whether the game is in debug mode. WHen in debug mode extra logs are shown and player can use cheat keys to skip levels.
		/// </summary>
		public bool IsDebug = false;

		/// <summary>
		/// Gets the width and height of the tiles.
		/// </summary>
		/// <value>The size of the tile.</value>
		public Vector2? TileSize { get; private set; }

		private Vector3 localScaleOfNodes = Vector3.one;
		public Vector3 nodeScale { get { return localScaleOfNodes; } }

		private static Utilities _instance;

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static Utilities instance {
			get {
				return _instance;
			}
		}

		void Awake ()
		{
			_instance = GetComponent<Utilities> ();

			// Comment this line out if the tiles you are using are the same size, or edit the values to match the average tile size if they differ in size.
			// (see 'Other things to note' in Read Me)
			//tileSize = new Vector2 (0.07f, 0.07f);

		}

		public void SetTileSize(TexturePack texturePack)
		{
			TileSize = texturePack.GetSpriteSize ();
			TileSize *= 0.97f; // Reduces chance that lines appear between sprites.
		}

		/// <summary>
		/// Returns world position of a specified node 
		/// </summary>
		public Vector2 GetNodePosition (Node node)
		{
			if (!TileSize.HasValue) {
				Debug.LogError ("Tile size not set");
			}
			
			if (node.Position.HasValue) {
				return node.Position.Value;
			}
					
			node.Position = new Vector2 (node.Coordinates.x * TileSize.Value.x + TileSize.Value.x / 2f, 
				node.Coordinates.y * TileSize.Value.y + TileSize.Value.y / 2f); 

			return node.Position.Value;
			
		}

		/// <summary>
		/// Returns grid coordinates for a node at 
		/// </summary>
		public Vector2? GetGridCoordinateForPosition (Vector2 position)
		{	
			if (!TileSize.HasValue) {
				Debug.LogError ("Tile size not set");
			}

			return new Vector2 (position.x / TileSize.Value.x, position.y / TileSize.Value.y);
		}

				
		/// <summary>
		/// Increments n towards target based on acceleration. 
		/// Useful in bespoke physics engine to increment a characters position towards their toarget.
		/// </summary>
		public float IncrementTowards (float n, float target, float acc)
		{
			if (n == target) {
				return n;	
			} else {
				float dir = Mathf.Sign (target - n); // must n be increased or decreased to get closer to target
				n += acc * Time.deltaTime * dir;
				return (dir == Mathf.Sign (target - n)) ? n : target; // if n has now passed target then return target, otherwise return n
			}
		}


		/// <summary>
		/// Implementation of unitys FindObjectWithTag with additional logging.
		/// </summary>
		public GameObject FindObjectWithTag (string scriptName, string tag)
		{
			var obj = GameObject.FindGameObjectWithTag (tag);

			if (!obj) {
				Debug.LogError (scriptName + ": no object with tag " + tag + " found");
			}

			return obj;
		}

		/// <summary>
		/// Implementation of unitys FindObjectOfType with additional logging.
		/// </summary>
		public T FindObjectOfType <T> (string scriptName) where T : Component
		{
			T obj = GameObject.FindObjectOfType<T> ();

			if (!obj) {
				Debug.LogError (scriptName + ": no object with type " + typeof(T) + " found");
			}

			return obj;
		}

		/// <summary>
		/// Implementation of unitys GetChildComponenet with additional logging.
		/// </summary>
		public T GetChildComponent <T> (string scriptName, Transform owner) where T : Component
		{
					
			T obj = (T)owner.GetComponent (typeof(T));

			if (!obj) {
				Debug.LogError (scriptName + ": no object with name " + typeof(T).Name + " found");
			}

			return obj; 
		}

		/// <summary>
		/// Implementation of unitys GetComponenet with additional logging.
		/// </summary>
		public T GetComponent<T> (string scriptName) where T: Component
		{
			T obj = (T)GetComponent (typeof(T));

			if (!obj) {
				Debug.LogError (scriptName + ": no object with name " + typeof(T).Name + " found");
			}

			return obj; 
		}

		public void UpdateNodeSortingOrder (string scriptName, GameObject node, int order)
		{
			SpriteRenderer spriteRenderer = GetChildComponent<SpriteRenderer> (scriptName, node.transform); 

			if (spriteRenderer) {
				spriteRenderer.sortingOrder = order;
			} 
		}

		/// <summary>
		/// Implementation of unitys Instantiate with additional logging. Instantiates object at specified position.
		/// Preferably use ObjectManager to initialise objects as this uses the object pool.
		/// </summary>
		public GameObject InstantiatePrefabAtPosition (string scriptName, GameObject prefab, Vector2 position)
		{
			GameObject obj = (GameObject)Instantiate (prefab, position, Quaternion.identity);

			if (!obj) {
				Debug.LogError (scriptName + ": no prefab with name " + prefab.name + " found");
			}

			return obj;
		} 

		/// <summary>
		/// Implementation of unitys Instantiate with additional logging.
		/// Preferably use ObjectManager to initialise objects as this uses the object pool.
		/// </summary>
		public GameObject InstantiatePrefab (string scriptName, GameObject prefab)
		{
			GameObject obj = (GameObject)Instantiate (prefab);

			if (!obj) {
				Debug.LogError (scriptName + ": no prefab with name " + prefab.name + " found");
			}

			return obj;
		}

		/// <summary>
		/// Returns prefab with specified name. Outputs error if prefab not found.
		/// </summary>
		public GameObject GetPrefab (string scriptName, string prefabName)
		{
			GameObject obj = (GameObject)Resources.Load (prefabName);

			if (!obj) {
				Debug.LogError (scriptName + ": no prefab with name " + prefabName + " found");
			}

			return obj;
		}

		/// <summary>
		/// Creates rays with angle difference.
		/// </summary>
		/// <returns>The rays.</returns>
		/// <param name="position">Position of rays.</param>
		/// <param name="heading">Heading of central ray.</param>
		/// <param name="angle">Angle between each ray.</param>
		/// <param name="number">Number of rays to create.</param>
		public List<Ray2D> CreateRays (Vector2 position, Vector2 heading, float angle, int number)
		{
			var rayList = new List <Ray2D> ();
			
			heading.Normalize ();

			angle /= number;
						
			int leftCount = 0;
			int rightCount = 0;
						
			for (int i = 0; i < number; i++) {
				Ray2D ray;
								
				if (i < (number / 2)) {
					Vector2 dir = Quaternion.AngleAxis (-angle * ++leftCount, Vector3.forward) * heading;
					ray = new Ray2D (position, dir);
									
				} else if (i > (number / 2)) {
					Vector2 dir = Quaternion.AngleAxis (angle * ++rightCount, Vector3.forward) * heading;
					ray = new Ray2D (position, dir);
						
				} else {
					ray = new Ray2D (position, heading);
									
				}

				rayList.Add (ray);

			}

			
			return rayList;
		}		

		/// <summary>
		/// Creates three rays with angle difference.
		/// </summary>
		/// <returns>The created rays.</returns>
		/// <param name="position">Position of rays.</param>
		/// <param name="heading">Heading of central ray.</param>
		/// <param name="angle">Angle between each ray.</param>
		public List<Ray2D> CreateRays (Vector2 position, Vector2 heading, float angle)
		{
			return CreateRays (position, heading, angle, 3);
		}

	}
}
