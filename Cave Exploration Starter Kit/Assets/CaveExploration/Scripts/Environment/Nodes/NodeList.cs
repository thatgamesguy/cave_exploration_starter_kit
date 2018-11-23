using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CaveExploration
{
	/// <summary>
	/// Encapsulates 2D array of all active nodes in game.
	/// Provides helper methods such as Add and Contains that mimick there list counterparts.
	/// </summary>
	public class NodeList
	{

		private Node[,] nodes;
		private Vector2 size;

		/// <summary>
		/// Gets the number of nodes contained in this list.
		/// </summary>
		/// <value>The count.</value>
		public int Count {
			get {
				return nodes.Length;
			}
		}


		public NodeList (Rect gridSize)
		{
			nodes = new Node[(int)gridSize.width, (int)gridSize.height];
			size = new Vector2 (gridSize.width, gridSize.height);
		}

		/// <summary>
		/// Contains the specified node.
		/// </summary>
		/// <param name="node">Node.</param>
		public bool Contains (Node node)
		{
			Vector2 coord = node.Coordinates;
			if (IsValidCoordinate (coord)) {
				Node obj = nodes [(int)coord.x, (int)coord.y];
		
				return obj != null;
			} else {
				return false;
			}

		}

		/// <summary>
		/// Determines whether the specified coords are valid.
		/// </summary>
		/// <returns><c>true</c> if the specified coordinate are valid; otherwise, <c>false</c>.</returns>
		/// <param name="coord">Coordinate.</param>
		public bool IsValidCoordinate (Vector2 coord)
		{
			return !(coord.x < 0 ||
				coord.x >= size.x ||
				coord.y < 0 ||
				coord.y >= size.y);
		}

		/// <summary>
		/// Determines if the node at position is of the specified type.
		/// </summary>
		/// <returns><c>true</c>, if node at position is of specified type, <c>false</c> otherwise.</returns>
		/// <param name="coord">Coordinate.</param>
		/// <param name="nodeType">Node type.</param>
		public bool ContainsNodeTypeAtPosition (Vector2 coord, NodeType nodeType)
		{
			if (!IsValidCoordinate (coord)) {
				return false;
			}

			return nodes [(int)coord.x, (int)coord.y].NodeState == nodeType;
		}

		/// <summary>
		/// Add the specified node to array.
		/// </summary>
		/// <param name="node">Node.</param>
		public void Add (Node node)
		{
			Vector2 coord = node.Coordinates;

			if (IsValidCoordinate (coord)) {
				nodes [(int)coord.x, (int)coord.y] = node;
			}
		}

		/// <summary>
		/// Returns first Node.
		/// </summary>
		public Node First ()
		{
			if (this.nodes.Length > 0)
				return nodes [0, 0];

			return null;
		}

		
		/// <summary>
		/// Replace the specified origNode with newNode.
		/// </summary>
		/// <param name="origNode">Original node.</param>
		/// <param name="newNode">New node.</param>
		public void Replace (Node origNode, Node newNode)
		{
			Vector2 coord = origNode.Coordinates;

			if (IsValidCoordinate (coord)) {
				nodes [(int)coord.x, (int)coord.y] = newNode;
			}
		}

		/// <summary>
		/// Replace the node with origNodeCoord, with the node newNode.
		/// </summary>
		/// <param name="origNodeCoord">Original node coordinate.</param>
		/// <param name="newNode">New node.</param>
		public void Replace (Vector2 origNodeCoord, Node newNode)
		{
			if (IsValidCoordinate (origNodeCoord)) {
				nodes [(int)origNodeCoord.x, (int)origNodeCoord.y] = newNode;
			}
		}
				
		/// <summary>
		/// Returns a list of adjacent nodes with or withour obstacles.
		/// </summary>
		/// <returns>The adjacent nodes.</returns>
		/// <param name="cellCoordinate">Cell coordinate or original node.</param>
		/// <param name="includeObstacles">If set to <c>true</c> include obstacles.</param>
		public List<Node> GetAdjacentNodes (Vector2 cellCoordinate, bool includeObstacles)
		{
			if (includeObstacles) {
				return GetAdjacentNodes (cellCoordinate);
			} else {
				return GetAdjacentNodesMinusObstacles (cellCoordinate);
			}
		}

		/// <summary>
		/// Return adjacent nodes minus obstacles.
		/// </summary>
		/// <returns>The adjacent nodes minus obstacles.</returns>
		/// <param name="cellCoordinate">Cell coordinate or original node.</param>
		private List<Node> GetAdjacentNodesMinusObstacles (Vector2 cellCoordinate)
		{
			List<Node> cells = new List<Node> ();


			// Top
			Vector2 top = new Vector2 (cellCoordinate.x, cellCoordinate.y - 1);
			Node node = GetValidNode (top);
			if (node != null) {
				cells.Add (node);
			}

	

			// Left
			Vector2 left = new Vector2 (cellCoordinate.x - 1, cellCoordinate.y);
			node = GetValidNode (left);
			if (node != null) {
				cells.Add (node);
			}
			
			// Below
			Vector2 below = new Vector2 (cellCoordinate.x, cellCoordinate.y + 1);
			node = GetValidNode (below);
			if (node != null) {
				cells.Add (node);
			}

				
			// Right
			Vector2 right = new Vector2 (cellCoordinate.x + 1, cellCoordinate.y);
			node = GetValidNode (right);
			if (node != null) {
				cells.Add (node);
			} 
			
			return cells;
		}

		/// <summary>
		/// return the adjacent nodes including obstacles.
		/// </summary>
		/// <returns>The adjacent nodes.</returns>
		/// <param name="cellCoordinate">Cell coordinate of original node.</param>
		private List<Node> GetAdjacentNodes (Vector2 cellCoordinate)
		{
			List<Node> cells = new List<Node> ();
			
			// Top
			Vector2 top = new Vector2 (cellCoordinate.x, cellCoordinate.y - 1);
			if (IsValidCoordinate (top)) {
				cells.Add (nodes [(int)top.x, (int)top.y]);
			}

					
			// Left
			Vector2 left = new Vector2 (cellCoordinate.x - 1, cellCoordinate.y);
			if (IsValidCoordinate (left)) {
				cells.Add (nodes [(int)left.x, (int)left.y]);
			}
			
			// Bellow
			Vector2 bellow = new Vector2 (cellCoordinate.x, cellCoordinate.y + 1);
			if (IsValidCoordinate (bellow)) {
				cells.Add (nodes [(int)bellow.x, (int)bellow.y]);
			}
								
			
			// Right
			Vector2 right = new Vector2 (cellCoordinate.x + 1, cellCoordinate.y);
			if (IsValidCoordinate (right)) {
				cells.Add (nodes [(int)right.x, (int)right.y]);
			}
			
			return cells;
		}

		/// <summary>
		/// Returns a node at a specified (on-screen) position that is not an obstacle.
		/// </summary>
		/// <returns>The valid node.</returns>
		/// <param name="pos">Position of node.</param>
		private Node GetValidNode (Vector2 pos)
		{
			Node node = GetNodeFromGridCoordinate (pos);

			if (!node.IsObstacle && !node.IsOccupied) {
				return node;
			} else {
				return null;
			}
		}

		/// <summary>
		/// Returns a node for a specified on-screen position.
		/// </summary>
		/// <returns>The node from position.</returns>
		/// <param name="position">Position.</param>
		public Node GetNodeFromPosition (Vector2 position)
		{
			// First need to convert position into coordinates. If no node found then null is returned.
			Vector2? coord = Utilities.instance.GetGridCoordinateForPosition (position);

			if (coord.HasValue) {
				return GetNodeFromGridCoordinate (coord.Value);
			}

			return null;
		}

		/// <summary>
		/// Returns a node with a specified grid coordinate.
		/// </summary>
		/// <returns>The node from grid coordinate.</returns>
		/// <param name="coord">Coordinate.</param>
		public Node GetNodeFromGridCoordinate (Vector2 coord)
		{
			if (IsValidCoordinate (coord)) {
				return nodes [(int)coord.x, (int)coord.y];
			}

			return null;
		}

	}
}
