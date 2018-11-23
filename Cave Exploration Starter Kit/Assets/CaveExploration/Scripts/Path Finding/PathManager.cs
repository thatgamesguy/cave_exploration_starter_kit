using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CaveExploration
{
	/// <summary>
	/// Creates a path between two nodes.
	/// </summary>
	public class PathManager : MonoBehaviour
	{
		/// <summary>
		/// Gets the shortest path using A*.
		/// </summary>
		/// <returns>A list of nodes between start and end that represents the shortest path.</returns>
		/// <param name="orig">Original cell.</param>
		/// <param name="dest">Destination cell.</param>
		/// <param name="wallMovementCost">Wall movement cost.</param>
		/// <param name="includeObstacles">If set to <c>true</c> include obstacles.</param>
		public List<Node> GetShortestPath (Node orig, Node dest, float wallMovementCost, bool includeObstacles)
		{
			wallMovementCost = 0f;
			List<Node> openSteps = new List<Node> ();
			List<Node> closedSteps = new List<Node> ();
			
			NodeList grid = GridManager.instance.Grid;
			
			//insert orig into openSteps
			InsertStep (new Node (orig.Coordinates, orig.NodeState, orig.IsOccupied), openSteps);

			do {
				Node currentStep = openSteps [0];
				
				closedSteps.Add (currentStep);
				
				openSteps.RemoveAt (0);
				
				if (Vector2.Equals (currentStep.Coordinates, dest.Coordinates)) {
					return ConstructPathFromNode (currentStep);
				}
								
				// Get the adjacent cell coordinates of the current step
				List<Node> adjNodes = grid.GetAdjacentNodes (currentStep.Coordinates, includeObstacles);
				
				foreach (var node in adjNodes) {
					//ShortestPathStep step = new ShortestPathStep(node.Position);
					
					// Check if the step isn't already in the closed set
					if (closedSteps.Contains (node)) {
						continue; // ignore it
					}
					
					// Compute the cost from the current step to that step
					float moveCost = CostToMove (currentStep, node, wallMovementCost);
										
					// Check if the step is already in the open list
					int index = openSteps.IndexOf (node);
					
					if (index == -1) { // Not on the open list, so add it
						
						// Set the current step as the parent
						node.Parent = currentStep;
						
						// The G score is equal to the parent G score plus the cost to move from the parent to it
						node.GScore = currentStep.GScore + moveCost;
									
						// Compute the H score, which is the estimated cost to move from that step
						// to the desired cell coordinate
						node.HScore = ComputeHScoreFromCoordinate (node.Coordinates, dest.Coordinates, grid); 
						
						// Adding it with the function which is preserving the list ordered by F score
						InsertStep (node, openSteps);
						
					} else {
						// To retrieve the old one, which has its scores already computed
						Node openNode = openSteps [index]; 
						
						// Check to see if the G score for that step is lower if we use the current step to get there
						if ((currentStep.GScore + moveCost) < openNode.GScore) {
							
							// The G score is equal to the parent G score plus the cost to move the parent to it
							openNode.GScore = currentStep.GScore + moveCost;
							
							// Because the G score has changed, the F score may have changed too.
							// So to keep the open list ordered we have to remove the step, and re-insert it with
							// the insert function, which is preserving the list ordered by F score.
							Node preservedStep = new Node (openNode.Coordinates, openNode.NodeState, 
																							openNode.IsOccupied);
							preservedStep.Parent = currentStep;
							preservedStep.GScore = currentStep.GScore + moveCost;
							preservedStep.HScore = ComputeHScoreFromCoordinate (preservedStep.Coordinates, 
																dest.Coordinates, grid);
							preservedStep.Position = openNode.Position;  
									
													                                             
							
							// Remove the step from the open list
							openSteps.RemoveAt (index); 
							
							// Re-insert the step to the open list
							InsertStep (preservedStep, openSteps);
						}
					} 
				} 
				
			} while (openSteps.Count > 0);

			return null;
		}

				
		private List<Node> ConstructPathFromNode (Node node)
		{
						
			List<Node> path = new List<Node> ();	
			while (node != null) {
				path.Add (node);
				node = node.Parent;
			}
			path.Reverse (); 
						
			return path;
		}

		private float ComputeHScoreFromCoordinate (Vector2 fromCoordinate, Vector2 toCoordinate, NodeList grid)
		{
			// Get the cell at the toCoordinate to calculate the hScore
			Node cell = grid.GetNodeFromGridCoordinate (toCoordinate);
			
			// It is 10 times more expensive to move through wall cells than floor cells.
			float multiplier = 1f;
						
			if (cell.IsOccupied) {
				multiplier = 200f;
			} else if (cell.IsObstacle) {
				multiplier = 10f;
			}

			return multiplier * (Mathf.Abs (toCoordinate.x - fromCoordinate.x) +
				Mathf.Abs (toCoordinate.y - fromCoordinate.y));
		}

		private float CostToMove (Node fromStep, Node toStep, float wallCost)
		{
			if (toStep.IsOccupied) {
				Debug.LogError ("Occupied");
				return 1000;
			}
					
			return toStep.IsObstacle ? wallCost : 1;
		}

		private void InsertStep (Node step, List<Node> list)
		{
			
			float stepFScore = step.GetFScore ();
			
			int count = list.Count;
			
			int i = 0;
			
			for (; i < count; i++) {
				if (stepFScore <= list [i].GetFScore ()) {
					break;
				}
			}
			
			list.Insert (i, step);
		}


	}
}
