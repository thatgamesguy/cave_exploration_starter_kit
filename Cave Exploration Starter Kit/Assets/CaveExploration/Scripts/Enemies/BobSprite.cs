using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// "Bobs" sprite up and down.
	/// </summary>
	public class BobSprite : MonoBehaviour
	{
		/// <summary>
		/// Max up/down movement.
		/// </summary>
		public float MaxUpDown = 0.1f;

		/// <summary>
		/// Up/down speed.
		/// </summary>
		public float Speed = 1f;

		private bool _enabled = false;

		/// <summary>
		/// Sets a value indicating whether this <see cref="CaveExploration.BobSprite"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public bool Enabled {
			set {
				_enabled = value;
			}
		}

		private float angle = 0f;
		private float startY;
		private static readonly float TO_DEGREES = Mathf.PI / 180;


		void OnEnable ()
		{
			startY = transform.position.y;
		}

		void Update ()
		{
			if (!_enabled)
				return;

			angle += Speed * Time.deltaTime;
			if (angle > 270)
				angle -= 360;

			transform.position = new Vector3 (transform.position.x,
			                                  startY + MaxUpDown * (1 + Mathf.Sin (angle * TO_DEGREES)) / 2,
			                                  transform.position.z);

		}
	

	}
}
