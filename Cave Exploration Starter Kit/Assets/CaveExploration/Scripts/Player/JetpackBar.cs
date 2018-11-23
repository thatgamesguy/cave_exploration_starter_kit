using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Visual representation of the jetpack fuel.
	/// </summary>
	[RequireComponent (typeof(SpriteRenderer))]
	public class JetpackBar : MonoBehaviour
	{
		/// <summary>
		/// The colour when jetpack fuel is full.
		/// </summary>
		public Color ColourWhenFullFuel;

		/// <summary>
		/// The colour when jetpack fuel is empty.
		/// </summary>
		public Color ColourWhenEmptyFuel;

		private SpriteRenderer _renderer;
		private Vector3 scale;
		private Vector2 initialScale;
		private Color initialColour;

		void Awake ()
		{
			_renderer = GetComponent<SpriteRenderer> ();
			scale = transform.localScale;
			initialScale = scale;
			initialColour = _renderer.color;
		}

		void OnEnable ()
		{
			_renderer.color = initialColour;
			transform.localScale = initialScale;
		}

		/// <summary>
		/// Disable the jetpack fuel renderer.
		/// </summary>
		public void Disable ()
		{
			_renderer.enabled = false;
		}

		/// <summary>
		/// Enables the jetpack fuel renderer.
		/// </summary>
		public void Enable ()
		{
			_renderer.enabled = true;

		}

		/// <summary>
		/// Updates the bars colour based on the current fuel amount.
		/// </summary>
		/// <param name="currentFuel">Current fuel.</param>
		public void UpdateColour (float currentFuel)
		{
			_renderer.color = Color.Lerp (ColourWhenEmptyFuel, ColourWhenFullFuel, currentFuel);
		}

		/// <summary>
		/// Updates the bars y scale based on current fuel amount.
		/// </summary>
		/// <param name="scaleY">Scale y.</param>
		public void UpdateLocalScaleY (float scaleY)
		{
			if (float.IsNaN (scaleY))
				scaleY = 0f;

			transform.localScale = new Vector3 (scale.x, scaleY, scale.z);
		}
	}
}
