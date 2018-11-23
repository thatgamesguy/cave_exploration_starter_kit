using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Shows character speech on enemy killed.
	/// </summary>
	[RequireComponent (typeof(CharacterSpeech))]
	public class EnemyKillCountSpeech : MonoBehaviour
	{
		/// <summary>
		/// Speech options on enemy killed.
		/// </summary>
		public string[] EnemyKilledSpeech = {"Got em!", "", "1 Down", "How many are left?", "", "Good Shot"};
		
		private CharacterSpeech characterSpeech;

		void Awake ()
		{
			characterSpeech = GetComponent<CharacterSpeech> ();
		}

		void OnEnable ()
		{
			Events.instance.AddListener<EnemyKilled> (OnEnemyKilled);
		}

		void OnDisable ()
		{
			Events.instance.RemoveListener<EnemyKilled> (OnEnemyKilled);
		}

		void OnEnemyKilled (EnemyKilled e)
		{
			characterSpeech.Speak (EnemyKilledSpeech [Random.Range (0, EnemyKilledSpeech.Length)]);
		}

	}
}
