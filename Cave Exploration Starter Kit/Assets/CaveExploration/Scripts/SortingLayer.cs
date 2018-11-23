using UnityEngine;
using System.Collections;

/// <summary>
/// Changes a mesh renderers sorting layer.
/// </summary>
[RequireComponent (typeof(MeshRenderer))]
public class SortingLayer : MonoBehaviour
{
	/// <summary>
	/// The sorting order.
	/// </summary>
	public int SortingOrder = 0;
	
	void Awake ()
	{
		GetComponent<MeshRenderer> ().sortingOrder = SortingOrder;
	}
}
