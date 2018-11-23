using UnityEngine;
using System;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// The cell type.
	/// </summary>
	public enum NodeType
	{
		Invalid = -1,
		Wall,
		WallTopLeft,
		WallTopMiddle,
		WallTopRight,
		WallMiddleLeft,
		WallMiddle,
		WallMiddleRight,
		WallBottomLeft,
		WallBottomMiddle,
		WallBottomRight,
		Background,
		Entry,
		Exit,
		Collectible,
		Max
	}
	
	/// <summary>
	/// Logical representation of a block. Holds the node type (e.g. wall, floor etc),
	/// it's coordinates (a pointer to the node in a 2d array - not it's position on screen),
	/// it's position on screen and path finding variables.
	/// </summary>
	public class Node : IComparable
	{

		private NodeType nodeState;

		/// <summary>
		/// Gets or sets the state of the node.
		/// </summary>
		/// <value>The state of the node.</value>
		public NodeType NodeState {
			get {
				return nodeState;
			}
			set {
				nodeState = value;
			}
		}

		private Vector2 coordinates;

		/// <summary>
		/// Gets or sets the node coordinates (i.e. location within the 2d array as opposed to actual position in world).
		/// </summary>
		/// <value>The coordinates.</value>
		public Vector2 Coordinates {
			get {
				return coordinates;
			}
			set {
				coordinates = value;
			}
		}

		/// <summary>
		/// Gets or sets the nodes position in the scene.
		/// </summary>
		/// <value>The position.</value>
		public Vector2? Position { get; set; }
				
		private bool isOccupied;

		/// <summary>
		/// Gets or sets a value indicating whether this node is occupied by an enemy/player.
		/// </summary>
		/// <value><c>true</c> if this instance is occupied; otherwise, <c>false</c>.</value>
		public bool IsOccupied {
			get {
				return this.isOccupied;
			}
			set {
				isOccupied = value;
			}
		}

		/// <summary>
		/// The cost to move into this node used for path finding.
		/// </summary>
		/// <value>The G score.</value>
		public float GScore { get; set; }
				
		/// <summary>
		/// Estimated cost to mvoe from this node to end node.
		/// </summary>
		/// <value>The H score.</value>
		public float HScore { get; set; }
				
		/// <summary>
		/// Used when traversing a path.
		/// </summary>
		/// <value>The parent.</value>
		public Node Parent { get; set; }

		/// <summary>
		/// Gets a value indicating whether this instance is obstacle and cannot be traversed.
		/// </summary>
		/// <value><c>true</c> if this instance is obstacle; otherwise, <c>false</c>.</value>
		public bool IsObstacle { get { return nodeState == NodeType.Wall; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="CaveExploration.Node"/> class.
		/// </summary>
		public Node () : this (Vector2.zero)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CaveExploration.Node"/> class.
		/// </summary>
		/// <param name="coordinates">Coordinates.</param>
		public Node (Vector2 coordinates) : this (coordinates, NodeType.Invalid)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CaveExploration.Node"/> class.
		/// </summary>
		/// <param name="coordinates">Coordinates.</param>
		/// <param name="state">State.</param>
		public Node (Vector2 coordinates, NodeType state) : this (coordinates, state, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CaveExploration.Node"/> class.
		/// </summary>
		/// <param name="coordinates">Coordinates.</param>
		/// <param name="state">State.</param>
		/// <param name="isOccupied">If set to <c>true</c> is occupied by a player or enemy.</param>
		public Node (Vector2 coordinates, NodeType state, bool isOccupied)
		{
			Coordinates = coordinates;
			NodeState = state;
			IsOccupied = isOccupied;
			
			HScore = 0f;
			GScore = 1f;
			Parent = null;
			Position = null;
		}

		/// <summary>
		/// Total score. Returns GScore + HScore
		/// </summary>
		public float GetFScore ()
		{
			return GScore + HScore;
		}

		/// <summary>
		/// Used to compare the movement cost of two nodes for pathfinding.
		/// </summary>
		/// <returns>Relative position.</returns>
		/// <param name="obj">Object.</param>
		public int CompareTo (object obj)
		{
			Node other = (Node)obj;

			if (this.HScore < other.HScore)
				return -1;

			if (this.HScore > other.HScore)
				return 1;

			return 0;
	
		}

		public override bool Equals (object obj)
		{
			if (obj == null) {
				return false;
			}

			if (!(obj is Node)) {
				return false;
			}

			var other = (Node)obj;

			return other.coordinates == this.coordinates;

		}

		public override int GetHashCode ()
		{
			int result = 17;
			result = 31 * result + nodeState.GetHashCode ();
			result = 31 * result + coordinates.GetHashCode ();
			result = 31 * result + GScore.GetHashCode();
			result = 31 * result + HScore.GetHashCode();
			return result;
		}

	}
}
