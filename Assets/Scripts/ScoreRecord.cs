using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreRecord : MonoBehaviour
{
	public static int score { get; private set; }

	private float lastEnemyKillTime;
	private float streakExpiryTime = 1f;
	private int streakCount;

	void Start()
	{
		Enemy.OnDeathStatic += OnEnemyKilled;
		FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
	}

	void OnEnemyKilled()
	{
		if (Time.time < lastEnemyKillTime + streakExpiryTime)
		{
			streakCount++;
		}
		else
		{
			streakCount = 0;
		}

		lastEnemyKillTime = Time.time;

		score += (10 + (int) Mathf.Pow(2, streakCount));
	}

	void OnPlayerDeath()
	{
		Enemy.OnDeathStatic -= OnEnemyKilled;
	}
}