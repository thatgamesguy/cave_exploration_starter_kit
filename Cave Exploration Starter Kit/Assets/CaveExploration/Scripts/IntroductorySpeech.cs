using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Introductory speech. For example, can be used to introduce a level, the game, controls etc.
	/// </summary>
	public class IntroductorySpeech : MonoBehaviour
	{
		/// <summary>
		/// Speech options. These options are shown one at a time.
		/// </summary>
		public string[] Speech;

		/// <summary>
		/// Gets a value indicating whether this <see cref="CaveExploration.IntroductorySpeech"/> is in progress.
		/// </summary>
		/// <value><c>true</c> if in progress; otherwise, <c>false</c>.</value>
		public bool InProgress { get { return inProgress; } }
	
		private bool inProgress;
		private CharacterSpeech characterSpeech;
		
		private static IntroductorySpeech _instance;

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static IntroductorySpeech instance { get { return _instance; } }
		
		void Awake ()
		{
			_instance = this;
			characterSpeech = GetComponent<CharacterSpeech> ();
		}
		
		void OnEnable ()
		{
			Events.instance.AddListener<LevelGeneratedSpeechRequired> (OnLevelGenerated);
		}
		
		void OnDisable ()
		{
			Events.instance.RemoveListener<LevelGeneratedSpeechRequired> (OnLevelGenerated);
		}
		
		private void OnLevelGenerated (GameEvent e)
		{
			inProgress = true;
			
			if (Speech != null && Speech.Length > 0) {
				StartCoroutine (ShowText (Speech [0], 0));
			} else {
				FinishedSpeaking ();
			}
		}
		
		private IEnumerator ShowText (string text, int i)
		{
			characterSpeech.Speak (Speech [i]);
						
			yield return new WaitForSeconds (text.Length * 0.15f);
			
			if (i < Speech.Length - 1) {
				i++;
				StartCoroutine (ShowText (Speech [i], i));
			} else {
				FinishedSpeaking ();
			}
		}
		
		private void FinishedSpeaking ()
		{
			inProgress = false;
			Events.instance.Raise (new IntroductorySpeechFinishedEvent ());
		}
	}
}
