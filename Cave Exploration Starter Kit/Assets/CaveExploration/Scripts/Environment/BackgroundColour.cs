using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaveExploration
{
	/// <summary>
	/// Sets the sprites colour based on the current background colour of the texture pack.
	/// </summary>
	[RequireComponent (typeof(SpriteRenderer))]
	public class BackgroundColour : MonoBehaviour
	{
		private SpriteRenderer spriteRenderer;

		void Awake()
		{
			spriteRenderer = GetComponent<SpriteRenderer> ();
		}

		void OnEnable()
		{
			Events.instance.AddListener<LevelGeneratedSpeechRequired> (OnLevelGenerated);
			Events.instance.AddListener<LevelGeneratedEvent> (OnLevelGenerated);
		}

		void OnDisable()
		{
			Events.instance.RemoveListener<LevelGeneratedSpeechRequired> (OnLevelGenerated);
			Events.instance.RemoveListener<LevelGeneratedEvent> (OnLevelGenerated);
		}
		
		private void OnLevelGenerated (LevelGeneratedSpeechRequired e)
		{
			SetBackgroundColour ();
		}

		private void OnLevelGenerated (LevelGeneratedEvent e)
		{
			SetBackgroundColour ();
		}

		private void SetBackgroundColour()
		{
			var bckTexture = GridManager.instance.TexturePack.WallMiddle;


			spriteRenderer.sprite = bckTexture;


			/*
			var centre = bckTexture.bounds.center;

			var colours = bckTexture.texture.GetPixels ();

			float r = 0f, g = 0f, b = 0f;

			foreach (var c in colours) {
				r += c.r;
				g += c.g;
				b += c.b;
			}

			r /= colours.Length;
			g /= colours.Length;
			b /= colours.Length;

			var bckColour = new Color (r, g, b, 1f);

			print ("Setting colour: " + bckColour);
			spriteRenderer.color = bckColour;
			*/
		}

	}
}