using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CaveExploration
{
	/// <summary>
	/// Manages adding and retrieving objects from the object pool.
	/// </summary>
	[RequireComponent (typeof(ObjectPool))]
	public class ObjectManager : MonoBehaviour
	{

		protected List<GameObject> objects = new List<GameObject> ();

		private static readonly string SCRIPT_NAME = typeof(ObjectManager).Name;

		private static ObjectManager _instance;

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static ObjectManager instance {
			get {
						
				if (!_instance) {
					_instance = Utilities.instance.GetComponent<ObjectManager> (SCRIPT_NAME);
					
					_instance.pool = Utilities.instance.GetChildComponent<ObjectPool> (SCRIPT_NAME, _instance.transform);
				}

				return _instance;
			}
		}

		private ObjectPool pool;

		/// <summary>
		/// Gets the object from the object pool. Moves to position with rotation and enables object.
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="prefabName">Prefab name.</param>
		/// <param name="position">Position.</param>
		/// <param name="rotation">Rotation.</param>
		public GameObject GetObject (string prefabName, Vector2 position, Quaternion rotation)
		{
			var obj = pool.GetObjectForType (prefabName, false);
			
			if (obj) {
				obj.transform.position = position;
				obj.transform.rotation = rotation;
				obj.SetActive (true);
				
				objects.Add (obj);
			} 
			
			return obj;
		}

		/// <summary>
		/// Gets the object from the object pool. Moves to position and enables object.
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="prefabName">Prefab name.</param>
		/// <param name="position">Position.</param>
		public GameObject GetObject (string prefabName, Vector2 position)
		{
			var obj = pool.GetObjectForType (prefabName, false);

			if (obj) {
				obj.transform.position = position;

				obj.SetActive (true);

				objects.Add (obj);
			} 

			return obj;
		}

		/// <summary>
		/// Gets the object from the object pool. Moves to position and enables object. 
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="prefabName">Prefab name.</param>
		/// <param name="position">Position.</param>
		/// <param name="onlyPooledObjects">If set to <c>true</c> only pooled objects will be returned. If no object is found in pool then null is returned.</param>
		public GameObject GetObject (string prefabName, Vector2 position, bool onlyPooledObjects)
		{

			var obj = pool.GetObjectForType (prefabName, onlyPooledObjects);
			
			if (obj) {
				obj.transform.position = position;
				
				obj.SetActive (true);
				
				objects.Add (obj);
			} 
			
			return obj;
		}

		/// <summary>
		/// Removes the object from the scene and adds it to the pool.
		/// </summary>
		/// <param name="obj">Object.</param>
		public void RemoveObject (GameObject obj)
		{
			pool.PoolObject (obj);
					
			objects.Remove (obj);

		}

		/// <summary>
		/// Removes all spawned objects and clears spawn list.
		/// </summary>
		public void RemoveObjects ()
		{
			for (int i = 0; i < objects.Count; i++) {
				pool.PoolObject (objects [i]);
		
			}
			
			objects.Clear ();
		}

	}
}
