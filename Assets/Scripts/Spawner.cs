using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[System.Serializable] public class Wave
	{
		public string tag;
		public int enemyCount;
		public float timeBtwSpawn;
	}

	public Wave[] waves;
	public Enemy enemy;
	public float timeBtwWave = 3;

	private int mCurrentWaveIndex = -1;
	private Wave mCurrentWave;
	private float nextSpawnTime;
	private int remainEnemiesToSpawn;
	private int remainEnemiesAlive;

	void Start()
	{
		NextWave();
	}

	void Update()
	{
		if (remainEnemiesToSpawn > 0 && Time.time > nextSpawnTime)
		{
			nextSpawnTime = Time.time + mCurrentWave.timeBtwSpawn;
			remainEnemiesToSpawn--;
			Enemy spawned = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
			spawned.OnDeath += OnEnemyDeath; //订阅事件
		}
	}

	void OnEnemyDeath()
	{
		remainEnemiesAlive--;
		Debug.Log("An enemy died.......");
		if (remainEnemiesAlive == 0)
		{
			StartCoroutine(NextWaveDelay());
		}
	}

	IEnumerator NextWaveDelay()
	{
		Debug.Log(timeBtwWave + "秒后开始 wave[" + (mCurrentWaveIndex + 1) + "]");
		yield return new WaitForSeconds(timeBtwWave);
		NextWave();
	}

	private void NextWave()
	{
		mCurrentWaveIndex++;
		if (mCurrentWaveIndex > waves.Length - 1)
		{
			Debug.LogWarning("wave[" + mCurrentWaveIndex + "]不存在，所有wave都已完结!");
			return;
		}

		mCurrentWave = waves[mCurrentWaveIndex];

		remainEnemiesToSpawn = mCurrentWave.enemyCount;
		remainEnemiesAlive = mCurrentWave.enemyCount;
	}
}