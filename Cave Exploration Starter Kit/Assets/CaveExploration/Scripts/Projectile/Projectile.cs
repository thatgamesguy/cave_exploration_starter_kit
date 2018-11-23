using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Attach to players light projectile. Handles seeking enemies, collision, 
	/// and returning projectile to pool.
	/// </summary>
	[RequireComponent (typeof(Rigidbody2D))]
	[RequireComponent (typeof(Explode))]
	public class Projectile : MonoBehaviour
	{
		/// <summary>
		/// Sets whether this projectile should seek an enemy.
		/// </summary>
		public bool SeekEnemies = false;

		/// <summary>
		/// THe speed to seek enemies.
		/// </summary>
		public float SeekSpeed = 2f;

		/// <summary>
		/// The maximum speed at which the projectile can rotate to move towards an enemy.
		/// </summary>
		public float RotationSpeed = 0.5f;

		/// <summary>
		/// The maximum time the projectile can be in the scene. It is added to the pool once this time has been reached.
		/// </summary>
		public float MaxTimeAlive = 5f;

		private float currentTimeAlive;
		private Transform target;
		private Rigidbody2D _rigidbody2D;
		private Explode explode;

		void Awake ()
		{
			_rigidbody2D = GetComponent<Rigidbody2D> ();
			explode = GetComponent<Explode> ();
		}

		void OnEnable ()
		{
			currentTimeAlive = 0f;
			target = null;
		}

		private void Explode ()
		{
			explode.Execute ();
		}

		void Update ()
		{
			currentTimeAlive += Time.deltaTime;

			if (currentTimeAlive >= MaxTimeAlive) {
				Explode ();
			}

			if (!SeekEnemies)
				return;

			if (target != null) {

				if (!target.gameObject.activeSelf) {
					target = null;
					return;
				}

				var heading = transform.position - target.position;
				float angle = Mathf.Atan2 (heading.y, heading.x) * Mathf.Rad2Deg;
				Quaternion q = Quaternion.AngleAxis (angle, Vector3.forward);
				transform.rotation = Quaternion.Slerp (transform.rotation, q, Time.deltaTime * RotationSpeed);

				_rigidbody2D.AddForce ((target.position - transform.position).normalized * (SeekSpeed * Time.deltaTime));
			}
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (!SeekEnemies)
				return;

			if (other.CompareTag ("Enemy") && EnemyInSight (other.transform.position)) {
				target = other.transform;
			}
		}

		void OnTriggerExit2D (Collider2D other)
		{
			if (!SeekEnemies)
				return;

			if (other.transform == target) {
				target = null;
			}
		}

		void OnCollisionEnter2D (Collision2D other)
		{
			if (other.gameObject.CompareTag ("Enemy")) {
				Explode ();
			}
		}

		private bool EnemyInSight (Vector3 enemyPos)
		{
			var heading = enemyPos - transform.position;
			var distance = heading.magnitude;
			var direction = heading / distance;
			
			Ray2D ray = new Ray2D (transform.position, direction);
			
			if (Utilities.instance.IsDebug)
				Debug.DrawRay (ray.origin, ray.direction, Color.white);
			
			var hit = Physics2D.Raycast (ray.origin, ray.direction, distance, 1 << LayerMask.NameToLayer ("Cave"));
			
			return hit.collider == null;
		}
	}
}
