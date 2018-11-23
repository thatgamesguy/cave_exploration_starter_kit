using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Enemy AI state.
	/// </summary>
	public enum AIState
	{
		Idle,
		PlayerInSight
	}

	/// <summary>
	/// Abstract base class for all enemy AI.
	/// </summary>
	public abstract class EnemyAI : MonoBehaviour
	{
		protected AIState state;

		protected Transform player;

		protected virtual void OnEnable()
		{
			player = null;
			Events.instance.AddListener<PlayerSpawnedEvent> (OnPlayerSpawned);
		}

		protected virtual void OnDisble()
		{
			Events.instance.RemoveListener<PlayerSpawnedEvent> (OnPlayerSpawned);
		}

		private void OnPlayerSpawned(PlayerSpawnedEvent e)
		{
			player = e.Player;
		}
	
		/// <summary>
		/// Update this instance using a simple state machine.
		/// </summary>
		public virtual void Update ()
		{
			if (player == null || IntroductorySpeech.instance.InProgress) {
				return;
			}
	
			DecideState ();

			switch (state) {
			case AIState.Idle:
				Idle ();
				break;
			case AIState.PlayerInSight:
				AttackPlayer ();
				break;
			}
		}

		protected abstract void Idle ();
		protected abstract void AttackPlayer ();
		protected abstract void DecideState ();

		protected bool PlayerNearby ()
		{
			if (player == null) {
				return false;
			}
			
			var heading = player.position - transform.position;
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
