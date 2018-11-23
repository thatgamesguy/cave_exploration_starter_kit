using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	public class EnemiesPlaced : GameEvent
	{

		private int enemyCount;
		public int EnemyCount { get { return enemyCount; } }

		public EnemiesPlaced (int enemyCount)
		{
			this.enemyCount = enemyCount;
		}
	}
}
