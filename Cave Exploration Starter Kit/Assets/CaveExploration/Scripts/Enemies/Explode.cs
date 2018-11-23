using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Spawns an explosion on mouse click at the objects position.
	/// </summary>
	[RequireComponent (typeof(Collider2D))]
	public class Explode : MonoBehaviour
	{
		public GameObject Explosion;

		void Awake ()
		{
			if (!Explosion) {
				Debug.Log ("Explosion not set, disabling script");
				enabled = false;
			}
		}

		void Update ()
		{
			if (Input.GetMouseButtonDown (0)) {
				Execute ();
			}
		}

		public void Execute ()
		{
			ObjectManager.instance.GetObject (Explosion.name, transform.position);
			ObjectManager.instance.RemoveObject (gameObject);
		}

	}
}
