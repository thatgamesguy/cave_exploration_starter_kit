using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Responsible for showing character speech (text) on screen.
	/// </summary>
	[RequireComponent (typeof(TextMesh))]
	public class CharacterSpeech : MonoBehaviour
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CaveExploration.CharacterSpeech"/> is currently speaking.
		/// </summary>
		/// <value><c>true</c> if speaking; otherwise, <c>false</c>.</value>
		public bool Speaking { get; set; }

		private TextMesh speech;

		void Awake ()
		{
			speech = GetComponent<TextMesh> ();
			HideSpeech ();
		}
	
		/// <summary>
		/// Shows the specified text at the location. The time the text is shown is based on the text length.
		/// </summary>
		/// <param name="text">Text.</param>
		public void Speak (string text)
		{
			StartCoroutine (ShowText (text));
		}

		private IEnumerator ShowText (string text)
		{
			Speaking = true;
			speech.text = text;
			SetAlpha (1);

			yield return new WaitForSeconds (text.Length * 0.15f);

			HideSpeech ();
			Speaking = false;
		}

		private void SetAlpha (float alpha)
		{
			var colour = speech.color;

			speech.color = new Color (colour.r, colour.b, colour.g, alpha);
		}



		private void HideSpeech ()
		{
			speech.text = " ";
			SetAlpha (0);
		}
	}
}
