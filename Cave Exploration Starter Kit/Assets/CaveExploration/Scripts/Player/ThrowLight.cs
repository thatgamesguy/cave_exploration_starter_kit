using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Handles the players ability to throw light.
	/// </summary>
	public class ThrowLight : MonoBehaviour
	{
		/// <summary>
		/// The number of lights held by the plyer at game start.
		/// </summary>
		public int Capacity = 3;

		/// <summary>
		/// The throwable prefab.
		/// </summary>
		public GameObject Throwable;

		/// <summary>
		/// The force that light is thrown.
		/// </summary>
		public float Force = 5f;

		/// <summary>
		/// Speech options to show when there are no more lights to be thrown.
		/// </summary>
		public string[] SpeechOnEmpty;

		private CharacterSpeech speech;
		private int currentThrowableCount;
		private GameObject currentThrowable;

		void Awake ()
		{
			if (!Throwable) {
				Debug.LogError ("No throwable set, disabling script");
				enabled = false;
			}

			var speechObj = GameObject.FindGameObjectWithTag ("Speech");
			
			if (speechObj) {
				speech = speechObj.GetComponent<CharacterSpeech> ();
			}
		}

		void OnEnable ()
		{
			currentThrowableCount = Capacity;
		}
	
		void Update ()
		{
		
			if (IntroductorySpeech.instance.InProgress)
				return;
		
			if (currentThrowable != null && !currentThrowable.activeSelf) {
				currentThrowable = null;
			}

			if (currentThrowable != null)
				return;
		
			if (Input.GetMouseButtonDown (0)) {

				if (currentThrowableCount-- <= 0) {
					if (speech && HasSpeechOptions ()) {
						speech.Speak (SpeechOnEmpty [Random.Range (0, SpeechOnEmpty.Length)]);
					}
					return;
				}

				var sp = Camera.main.WorldToScreenPoint (transform.position);
				var dir = (Input.mousePosition - sp).normalized;

				currentThrowable = ObjectManager.instance.GetObject (Throwable.name, transform.position);

				currentThrowable.GetComponent<Rigidbody2D> ().AddForce (dir * Force);
			}
	
		}

		private bool HasSpeechOptions ()
		{
			return SpeechOnEmpty != null && SpeechOnEmpty.Length > 0;
		}
	}
}
