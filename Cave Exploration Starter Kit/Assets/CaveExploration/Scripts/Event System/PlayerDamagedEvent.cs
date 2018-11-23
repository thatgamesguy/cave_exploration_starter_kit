using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Raised when the player takes damage.
	/// </summary>
	public class PlayerDamagedEvent : GameEvent
	{

		private int damageAmount;

		/// <summary>
		/// The amount to reduce the players health.
		/// </summary>
		/// <value>The damage amount.</value>
		public int DamageAmount { get { return damageAmount; } }

		private Vector2? forceDirection;
		private float force;

		/// <summary>
		/// Initializes a new instance of the <see cref="CaveExploration.PlayerDamagedEvent"/> class.
		/// </summary>
		/// <param name="damageAmount">Damage amount.</param>
		public PlayerDamagedEvent (int damageAmount) : this (damageAmount, null, 0f)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CaveExploration.PlayerDamagedEvent"/> class.
		/// </summary>
		/// <param name="damageAmount">Damage amount.</param>
		/// <param name="forceDirection">Force direction.</param>
		/// <param name="force">Force.</param>
		public PlayerDamagedEvent (int damageAmount, Vector2? forceDirection, float force)
		{
			this.damageAmount = damageAmount;
			this.forceDirection = forceDirection;
			this.force = force;
		}

		/// <summary>
		/// The force to apply as knockback when player recieves damage.
		/// </summary>
		/// <returns>The force.</returns>
		public Vector2? DamageForce ()
		{
			if (!forceDirection.HasValue)
				return null;

			return forceDirection.Value * force;
		}
	}
}
