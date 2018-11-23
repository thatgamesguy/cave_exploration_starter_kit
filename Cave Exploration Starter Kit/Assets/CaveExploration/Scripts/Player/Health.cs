using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// The players health.
	/// </summary>
	[RequireComponent (typeof(SpriteRenderer))]
	public class Health : MonoBehaviour
	{
		/// <summary>
		/// The players maximum health.
		/// </summary>
		public int MaxHealth = 5;

		/// <summary>
		/// Speech options to show when the player is hurt.
		/// </summary>
		public string[] HurtSpeech;

		/// <summary>
		/// Speech options to show when the player has low health.
		/// </summary>
		public string[] LowHealthSpeech;

		/// <summary>
		/// The low health threshold. Speech is shown when the players health is below the threshold.
		/// </summary>
		public int LowHealthThreshold = 2;

		/// <summary>
		/// The colour to change the players sprite to when damaged.
		/// </summary>
		public Color HurtColour = Color.red;

		/// <summary>
		/// The damage animation time.
		/// </summary>
		public float DamageAnimationTime = 0.5f;

		private PlayerAudio _audio;
		private int currentHealth;
		private CharacterSpeech speech;
		private Rigidbody2D _rigidbody2D;
		private SpriteRenderer _renderer;
		private Color spriteColour;
		private bool applyDamage = true;

		void Awake ()
		{
			_audio = GetComponent<PlayerAudio> ();
			var speechObj = GameObject.FindGameObjectWithTag ("Speech");

			if (speechObj) {
				speech = speechObj.GetComponent<CharacterSpeech> ();
			}

			_rigidbody2D = GetComponent<Rigidbody2D> ();
			_renderer = GetComponent<SpriteRenderer> ();
		}

		void Start ()
		{
			currentHealth = MaxHealth;
			spriteColour = _renderer.color;
		}

		void OnEnable ()
		{
			currentHealth = MaxHealth;
			applyDamage = true;
			//_renderer.color = spriteColour;
			Events.instance.AddListener<PlayerDamagedEvent> (OnDamage);
		}

		
		void OnDisable ()
		{
			Events.instance.RemoveListener<PlayerDamagedEvent> (OnDamage);
		}
	
		/// <summary>
		/// Handles the PlayerDamagedEvent. Applies damage, shows speech, and kills player (if health less than zero).
		/// </summary>
		/// <param name="e">E.</param>
		public void OnDamage (PlayerDamagedEvent e)
		{
			if (!applyDamage || currentHealth <= 0)
				return;

			currentHealth -= e.DamageAmount;

			if (currentHealth <= 0) {
				OnDead ();
				return;
			}

			_audio.OnPlayerDamaged ();
			StartCoroutine (Hurt ());

		
			if (currentHealth <= LowHealthThreshold) {
				Speak (LowHealthSpeech);
			} else {
				Speak (HurtSpeech);
			}


			var damageForce = e.DamageForce ();
			
			if (damageForce.HasValue) {
				_rigidbody2D.AddForce (damageForce.Value, ForceMode2D.Force);
			}

			applyDamage = false;


		}

		private IEnumerator Hurt ()
		{
			var damageTime = DamageAnimationTime;
	
			while (damageTime > 0) {
				_renderer.color = Color.Lerp (spriteColour, HurtColour, damageTime);
				damageTime -= 0.1f;
				yield return new WaitForSeconds (0.1f);
			}

			applyDamage = true;
			
		}

		private void Speak (string[] options)
		{
			if (speech && options != null && options.Length > 0) {
				speech.Speak (options [Random.Range (0, options.Length)]);
			}
		}


		private void OnDead ()
		{
			Events.instance.Raise (new PlayerKilledEvent ());
			applyDamage = false;
		}
	}
}
