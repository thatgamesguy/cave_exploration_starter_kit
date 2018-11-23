using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Disables a colliders trigger (i.e. enables collisions with a collider) when the collider exits the specified trigger. 
	/// Useful for shooting a projectile to ensure it does not collide with the enemy/player that shot the projectile.
	/// </summary>
	[RequireComponent (typeof(Collider2D))]
	public class EnableColliderOnTriggerExit : MonoBehaviour
	{
		/// <summary>
		/// Enable this is the object contains multiple colliders.
		/// </summary>
		public bool MultipleColliders = true;

		/// <summary>
		/// The tag of the gameobject that will be shooting projectile.
		/// </summary>
		public string Tag = "Player";

		private Collider2D _collider;
		private bool set = false;

		void Awake ()
		{
			if (MultipleColliders) {
				var colliders = GetComponents<Collider2D> ();

				foreach (var c in colliders) {
					if (c.sharedMaterial != null) {
						_collider = c;
					}
				}
			} else {
				_collider = GetComponent<Collider2D> ();
			}
		}

		void OnEnable ()
		{
			_collider.isTrigger = true;
			set = false;
		}
	
		void OnTriggerExit2D (Collider2D other)
		{
			if (set)
				return;

			if (other.CompareTag (Tag)) {
				_collider.isTrigger = false;
				set = true;
			}
		}
	}
}
