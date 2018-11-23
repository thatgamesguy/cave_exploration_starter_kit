using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CaveExploration
{

	/// <summary>
	/// Represents a group of neighbouring floor nodes i.e. a cavern in the environment.
	/// </summary>
	public class NodeCluster
	{
				
		private List<Node> nodes;

		/// <summary>
		/// Gets or sets the nodes contained by this cluster.
		/// </summary>
		/// <value>The nodes.</value>
		public List<Node> Nodes {
			get {
				return nodes;
			}
			set {
				nodes = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CaveExploration.NodeCluster"/> class.
		/// </summary>
		public NodeCluster ()
		{
			nodes = new List<Node> ();
						
		}
	}
}
