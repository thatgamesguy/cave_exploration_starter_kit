using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Handles changing of background image based on currently used texture pack.
	/// </summary>
	[RequireComponent (typeof(SpriteRenderer))]
	public class CameraBackground : MonoBehaviour
	{
		private SpriteRenderer _renderer;

		void Awake ()
		{
			_renderer = GetComponent<SpriteRenderer> ();
		}

		void OnEnable ()
		{
			Events.instance.AddListener<LevelGeneratedEvent> (OnLevelGenerated);
		}

		void OnDisable ()
		{
			Events.instance.RemoveListener<LevelGeneratedEvent> (OnLevelGenerated);
		}

		private void OnLevelGenerated (GameEvent e)
		{
			var texture = GridManager.instance.TexturePack.Background;

		
			_renderer.sprite = texture;

			Debug.LogError ("Here");

		}

	}
}
