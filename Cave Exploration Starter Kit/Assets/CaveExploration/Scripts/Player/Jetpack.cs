using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Responsible for controlling the players jetpack.
	/// </summary>
	[RequireComponent (typeof(ParticleSystem))]
	public class Jetpack : MonoBehaviour
	{
		/// <summary>
		/// The force applied when jet pack is active.
		/// </summary>
		public float JetForce = 30f;

		/// <summary>
		/// The maximum force applied to the player.
		/// </summary>
		public float MaxForce = 1.5f;

		/// <summary>
		/// The maximum jet fuel.
		/// </summary>
		public float JetFuel = 600f;

		/// <summary>
		/// The jet fuel waste per second.
		/// </summary>
		public float JetFuelWaste = 2f;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CaveExploration.Jetpack"/> is using the jetpack.
		/// </summary>
		/// <value><c>true</c> if using jetpack; otherwise, <c>false</c>.</value>
		public bool UsingJet { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance can use the jetpack.
		/// </summary>
		/// <value><c>true</c> if this instance can jet; otherwise, <c>false</c>.</value>
		public bool CanJet { get; set; }

		private static readonly float FUEL_RECHARGE_MULT = 1.1f;

		private float currentFuel;
		private Player playerController;
		private BottomCheck groundCheck;
		private Rigidbody2D playerRigidbody;
		private ParticleSystem jetParticle;
		private JetpackBar bar;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CaveExploration.Jetpack"/> is currently recharging the fuel.
		/// </summary>
		/// <returns><c>true</c>, if recharging was fueled, <c>false</c> otherwise.</returns>
		public bool FuelRecharging ()
		{
			return currentFuel < JetFuel;
		}

		void Awake ()
		{
			playerController = transform.parent.GetComponent<Player> ();
			playerRigidbody = transform.parent.GetComponent<Rigidbody2D> ();
			groundCheck = transform.parent.GetComponentInChildren<BottomCheck> ();
			jetParticle = GetComponent<ParticleSystem> ();
			currentFuel = JetFuel;

		}

		void OnEnable ()
		{
			CanJet = false;
			UsingJet = false;
			currentFuel = JetFuel;
			if (bar) {
				bar.Disable ();
			}
		}

		void Start ()
		{
			bar = GetComponentInChildren<JetpackBar> ();
			if (bar) {
				bar.Disable ();
			}
		}

		void FixedUpdate ()
		{
			if (bar) {
				var currentFuelNorm = currentFuel / (JetFuel * 5f);

				bar.UpdateLocalScaleY (currentFuelNorm);
				bar.UpdateColour (currentFuel / JetFuel);

				if (currentFuel >= JetFuel && !UsingJet)
					bar.Disable ();
				else if (currentFuel < JetFuel)
					bar.Enable ();
			}

			if (UsingJet) {		
				jetParticle.SetEmissionRate (30);
				if (currentFuel > 0) {
					currentFuel -= JetFuelWaste;
				} else {
					CanJet = false;
					UsingJet = false;
				}
				
			} else {
				if (groundCheck.IsGrounded) {
					if (currentFuel < JetFuel)
						currentFuel += JetFuelWaste * FUEL_RECHARGE_MULT;
					else
						currentFuel = JetFuel;
				}


				if (jetParticle.GetEmissionRate () > 0)
					jetParticle.SetEmissionRate (jetParticle.GetEmissionRate () - 2f);
				else
					jetParticle.SetEmissionRate (0);
			}
		}

		void Update ()
		{
			if (playerController.IsDead)
				return;

			var isGrounded = groundCheck.IsGrounded;

			if (isGrounded || playerController.HasJumped) {
				CanJet = false;
				UsingJet = false;
			}


			if (!isGrounded && (Input.GetKeyUp (KeyCode.Space) || playerRigidbody.velocity.y < 0f)) {
				UsingJet = false;
				CanJet = true;


			} 

			if (!isGrounded && CanJet && Input.GetKey (KeyCode.Space)) {
				if (!UsingJet) {
					UsingJet = true;
				}
		
				if (playerRigidbody.velocity.y <= MaxForce) {
					playerRigidbody.AddForce (new Vector2 (0, JetForce));
				}
			}

		
		
		}


	}
}
