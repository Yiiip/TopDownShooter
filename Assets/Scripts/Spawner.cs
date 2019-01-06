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

	private MapGenerator mMapGenerator;
	private LivingEntity mPlayerEntity;
	private Transform mPlayerTransform;

	float timeBtwCampingChecks = 2f;
	float nextCampingCheckTime;
	float campThresholdDistance = 1.5f;
	Vector3 campPosOld;
	bool isCamping; //玩家是否停留不动

	bool isPlayerDie = false;

	public event System.Action<int> OnNewWave;

	void Start()
	{
		mPlayerEntity = FindObjectOfType<Player>();
		mPlayerEntity.OnDeath += OnPlayerDeath;
		mPlayerTransform = mPlayerEntity.transform;
		mMapGenerator = FindObjectOfType<MapGenerator>();

		nextCampingCheckTime = Time.time + timeBtwCampingChecks;
		campPosOld = mPlayerTransform.position;

		NextWave();
	}

	void Update()
	{
		if (!isPlayerDie)
		{
			if (Time.time > nextCampingCheckTime)
			{
				nextCampingCheckTime = Time.time + timeBtwCampingChecks;
				isCamping = (Vector3.Distance(mPlayerTransform.position, campPosOld) < campThresholdDistance);
				campPosOld = mPlayerTransform.position;
			}

			if (remainEnemiesToSpawn > 0 && Time.time > nextSpawnTime)
			{
				nextSpawnTime = Time.time + mCurrentWave.timeBtwSpawn;
				remainEnemiesToSpawn--;

				StartCoroutine(SpawnEnemy());
			}
		}
	}

	IEnumerator SpawnEnemy()
	{
		float delay = 1f;
		float tileFlashSpeed = 4f;

		Transform tile = mMapGenerator.GetRandomOpenTile();
		if (isCamping)
		{
			tile = mMapGenerator.GetTileByPosition(mPlayerTransform.position);
		}

		Material matTile = tile.GetComponent<Renderer>().material;
		Color initColor = matTile.color;
		Color flashColor = Color.red;
		float spawnTimer = 0.0f;

		while (spawnTimer < delay)
		{
			matTile.color = Color.Lerp(initColor, flashColor, Mathf.PingPong(tileFlashSpeed * spawnTimer, 1f));
			spawnTimer += Time.deltaTime;
			yield return null;
		}

		matTile.color = initColor;
		Enemy spawned = Instantiate(enemy, tile.position + Vector3.up, Quaternion.identity) as Enemy;
		spawned.OnDeath += OnEnemyDeath; //订阅事件
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

	void OnPlayerDeath()
	{
		isPlayerDie = true;
	}

	IEnumerator NextWaveDelay()
	{
		if (mCurrentWaveIndex + 1 > waves.Length - 1)
		{
			Debug.LogWarning("wave[" + mCurrentWaveIndex + "]结束，所有wave都已完结!");
			yield return null;
		}
		else
		{
			Debug.Log(timeBtwWave + "秒后将开始 wave[" + (mCurrentWaveIndex + 1) + "]");
			yield return new WaitForSeconds(timeBtwWave);
			NextWave();
		}
	}

	private void NextWave()
	{
		mCurrentWaveIndex++;

		if (mCurrentWaveIndex < waves.Length)
		{
			mCurrentWave = waves[mCurrentWaveIndex];

			remainEnemiesToSpawn = mCurrentWave.enemyCount;
			remainEnemiesAlive = mCurrentWave.enemyCount;

			if (OnNewWave != null)
			{
				OnNewWave(mCurrentWaveIndex);
			}
		}
	}
}