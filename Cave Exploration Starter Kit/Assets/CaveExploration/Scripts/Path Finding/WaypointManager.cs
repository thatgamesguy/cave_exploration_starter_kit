using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CaveExploration
{
	/// <summary>
	/// Delegate used by waypoint manager.
	/// </summary>
	public delegate void InitWaypointDel (List<Node> path);

	/// <summary>
	/// Holds list of waypoints and allows iteration over waypoints.
	/// </summary>
	public class WaypointManager : MonoBehaviour
	{
		/// <summary>
		///If not looped will end at last wapoint
		/// </summary>
		public bool IsLooped = false;

		/// <summary>
		/// When entity is within this distance to waypoint it will be registered as visited
		/// </summary>
		public float WaypointProximity = 0.1f;
	
		/// <summary>
		/// List of waypoint objects - initialised in InitiailiseWayPoints
		/// </summary>
		public List<Node> Waypoints = new List<Node> ();

		/// <summary>
		/// Gets or sets a value indicating whether this instance is complete.
		/// </summary>
		/// <value><c>true</c> if this instance is complete; otherwise, <c>false</c>.</value>
		public bool IsComplete { get; set; }
				
		private int currentWaypoint = 0;

		private const float WALL_WEIGHT = 3f;
				
		void Awake ()
		{
			IsComplete = true;
		}

		/// <summary>
		/// Initialises the internal waypoints from a list of nodes.
		/// </summary>
		/// <param name="path">Path.</param>
		public void InitialiseWaypointsFromNodes (List<Node> path)
		{

			if (Waypoints.Count > 0) {
				Waypoints.Clear ();
			}

			if (path != null && path.Count > 0) {
				Waypoints.AddRange (path); 
								
				IsComplete = false;
				currentWaypoint = 0;	
			} else {
				IsComplete = true;
			}

		}
	
		/// <summary>
		/// Determines whether this object has reached current waypoint.
		/// </summary>
		/// <returns><c>true</c> if this instance has reached the current waypoint; otherwise, <c>false</c>.</returns>
		/// <param name="characterPosition">Characters current position.</param>
		public bool HasReachedCurrentWaypoint (Vector2 characterPosition)
		{
			return (Vector2.Distance (GetCurrentWaypointPosition (), characterPosition) < WaypointProximity);
		}

		/// <summary>
		/// Gets the current waypoint.
		/// </summary>
		/// <returns>The current waypoint.</returns>
		public Node GetCurrentWaypoint ()
		{
			return Waypoints [currentWaypoint];
		}

		/// <summary>
		/// Gets wether the waypoint manager has been initialised.
		/// </summary>
		public bool Initialised ()
		{
			return (Waypoints != null && Waypoints.Count > 0);
		}

		/// <summary>
		/// Gets the next reactive position. The next position within the players line of sight (LOS) is returned.
		/// This iterates from the current waypoint to the next 5 waypoints and returns the last waypoint in the characters LOS.
		/// </summary>
		/// <returns>The next reactive position.</returns>
		/// <param name="currentPosition">Current position.</param>
		public Vector2 GetNextReactivePosition (Vector2 currentPosition)
		{
			//check reached current waypoint
			// if no return current waypoint position
			// else loop through each waypoint, find furthest waypoint with clear path and set that to current.
			
			bool reachedEnd = HasReachedCurrentWaypoint (currentPosition);
			
			if (reachedEnd && currentWaypoint == Waypoints.Count - 1) {
				if (IsLooped) {
					currentWaypoint = 0;
				} else {
					IsComplete = true;
				}
				
			}
			
			var lookAhead = 0;
			var maxLookAhead = 5;
			
			if (reachedEnd && !IsComplete) {
				var newWaypoint = currentWaypoint;
				for (int i = currentWaypoint; i < Waypoints.Count; i++) {
					var newPos = Utilities.instance.GetNodePosition (Waypoints [i]);
					if (!LayerInPath (newPos, LayerMask.NameToLayer ("Cave"))) {
						newWaypoint = i;
						
						if (lookAhead++ >= maxLookAhead) {
							break;
						}
					}
					
				}
				
				UpdateCurrentWaypoint (newWaypoint);
			}
			
			
			if (Utilities.instance.IsDebug) {
				DebugDrawPath ();
			}
			
			
			return  (GetCurrentWaypointPosition () - currentPosition).normalized; 
			
		}

		/// <summary>
		/// Gets the next position that character should move towards.
		/// </summary>
		/// <returns>The next position.</returns>
		/// <param name="currentPosition">Current position.</param>
		public Vector2 GetNextPosition (Vector2 currentPosition)
		{
			CheckHasReachedCurrentWaypointAndIncrement (currentPosition);			
			
			
			Vector2 newPos = (GetCurrentWaypointPosition () - currentPosition).normalized;
			
			if (Utilities.instance.IsDebug) {
				DebugDrawPath ();
			}
			
			return newPos; 
			
			
			
		}

		private void CheckHasReachedCurrentWaypointAndIncrement (Vector2 characterPosition)
		{
			if (HasReachedCurrentWaypoint (characterPosition)) {
				IncrementCurrentWaypoint ();
			}
		}

		private void IncrementCurrentWaypoint ()
		{

			if (currentWaypoint == Waypoints.Count - 1) {
				if (IsLooped) //seek to first waypoint
					currentWaypoint = 0;
				else
					IsComplete = true;
			} else {
				currentWaypoint++;
			}
		}

		private void UpdateCurrentWaypoint (int newWaypoint)
		{
					
			currentWaypoint = newWaypoint;
						
		}

		private Vector2 GetCurrentWaypointPosition ()
		{
			return  Utilities.instance.GetNodePosition (Waypoints [currentWaypoint]);
		}



		private void DebugDrawPath ()
		{
			// Draw path for debug purposes.
			for (int i = currentWaypoint; i < Waypoints.Count - 1; i++) {
				Debug.DrawLine (Utilities.instance.GetNodePosition (Waypoints [i]),
					                Utilities.instance.GetNodePosition (Waypoints [i + 1]));
			}
						
		}



		private bool LayerInPath (Vector3 pos, LayerMask mask)
		{
			var distance = (pos - transform.position).magnitude;
			
			Ray2D ray = new Ray2D (transform.position, -transform.right);
			
			if (Utilities.instance.IsDebug)
				Debug.DrawRay (ray.origin, ray.direction, Color.white);
			
			var hit = Physics2D.Raycast (ray.origin, ray.direction, distance, 1 << mask);
			
			return hit.collider != null;
		}


	}
}
