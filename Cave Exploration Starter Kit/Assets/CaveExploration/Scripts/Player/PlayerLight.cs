using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Responsible for updating the players light.
	/// </summary>
	[RequireComponent (typeof(PlayerLight))]
	[RequireComponent (typeof(AudioPlayer))]
	public class PlayerLight : MonoBehaviour
	{
		/// <summary>
		/// The light intensity decrease per fixed update.
		/// </summary>
		public float LightDecreaseOverTime = 0.001f;

		/// <summary>
		/// The radius decrease per fixed update.
		/// </summary>
		public float RadiusDecreaseOverTime = 0.0002f;

		/// <summary>
		/// The audio clip to play when light collectible picked up.
		/// </summary>
		public AudioClip PickUpAudio;

		/// <summary>
		/// Sets whether the level should be reset when light reaches zero.
		/// </summary>
		public bool RestartLevelOnEmptyLight = false;

		/// <summary>
		/// Speech options when light reaches threshold.
		/// </summary>
		public string[] LightLowSpeech;

		/// <summary>
		/// The low light threshold (LowLightSpeech is shown when the light reaches this threshold).
		/// </summary>
		public float LowLightThreshold = 2f;

		/// <summary>
		/// The light colour when player has been hit.
		/// </summary>
		public Color LightDecreaseColour = Color.black;

		/// <summary>
		/// The time to change the light colour.
		/// </summary>
		public float DamageAnimationTime = 0.5f;

		/// <summary>
		/// Speech options to show then the light decreases.
		/// </summary>
		public string[] LightDecreaseSpeech;

		private CharacterSpeech speech;
		private float initialLightIntensity;
		private float initialRange;
		private Light _light;
		private bool spoken;
		private AudioPlayer _audio;

		void Awake ()
		{
			_light = GetComponent<Light> ();
			_audio = GetComponent<AudioPlayer> ();

			if (HasSpeechOptions (LightLowSpeech) || HasSpeechOptions (LightDecreaseSpeech)) {
				var speechObj = GameObject.FindGameObjectWithTag ("Speech");
			
				if (speechObj) {
					speech = speechObj.GetComponent<CharacterSpeech> ();
				}
			}

			initialLightIntensity = _light.intensity;
			initialRange = _light.range;
		}


		private bool HasSpeechOptions (string[] speechOp)
		{
			return speechOp != null && speechOp.Length > 0;
		}

		void OnEnable ()
		{
			//_light.color = initialColour;
			_light.intensity = initialLightIntensity;
			_light.range = initialRange;


			Events.instance.AddListener<CollectiblePickedUpEvent> (OnCollectiblePickUp);
			Events.instance.AddListener<LightDecreaseEvent> (OnLightDecrease);
		}
	
		void OnDisable ()
		{
			Events.instance.RemoveListener<CollectiblePickedUpEvent> (OnCollectiblePickUp);
			Events.instance.RemoveListener<LightDecreaseEvent> (OnLightDecrease);
		}

		private void OnCollectiblePickUp (CollectiblePickedUpEvent e)
		{
			_audio.PlaySound (PickUpAudio, 0.5f);
			_light.intensity += e.LightAmount;
			_light.range += e.RadiusAmount;
		}

		private void OnLightDecrease (LightDecreaseEvent e)
		{
			_audio.PlaySound (PickUpAudio, 0.5f, -1.1f);
			StartCoroutine (Hurt ());

			if (speech && HasSpeechOptions (LightDecreaseSpeech)) {
				speech.Speak (LightDecreaseSpeech [Random.Range (0, LightDecreaseSpeech.Length)]);
			}

			if (e.LightAmount.HasValue) {
				_light.intensity -= e.LightAmount.Value;
			}

			if (e.RadiusAmount.HasValue) {
				_light.range -= e.RadiusAmount.Value;
			}

			CheckLightEmpty ();
		}

		private IEnumerator Hurt ()
		{
			var currentColour = _light.color;
			
			while (DamageAnimationTime > 0) {
				_light.color = Color.Lerp (currentColour, LightDecreaseColour, DamageAnimationTime);
				DamageAnimationTime -= 0.1f;
				yield return new WaitForSeconds (0.1f);
			}
			
		}


		void FixedUpdate ()
		{
			_light.intensity -= LightDecreaseOverTime;
			_light.range -= RadiusDecreaseOverTime;

			if (_light.intensity < LowLightThreshold && !spoken && speech && HasSpeechOptions (LightLowSpeech)) {
				spoken = true;
				speech.Speak (LightLowSpeech [Random.Range (0, LightLowSpeech.Length)]);
			}

			CheckLightEmpty ();
		}

		private void CheckLightEmpty ()
		{
			if (!RestartLevelOnEmptyLight)
				return;

			if (_light.intensity <= 0.1f || _light.range <= 0.1f) {
				Events.instance.Raise (new PlayerKilledEvent ());
			}
		}
	}
}
