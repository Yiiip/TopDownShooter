using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public Transform tilePrefab;
	public Transform obstaclePrefab;
	public Vector2 mapSize;

	[Range(0.0f, 100.0f)]
	public float paddingPercent;

	public int seed;

	private List<Coord> mAllTileCoords = new List<Coord>();
	private Queue<Coord> mShuffledTileCoords = new Queue<Coord>();

	private void Start()
	{
		GenerateMap();
	}

	public void GenerateMap()
	{
		mAllTileCoords.Clear();
		for (int x = 0; x < mapSize.x; x++)
		{
			for (int y = 0; y < mapSize.y; y++)
			{
				mAllTileCoords.Add(new Coord(x, y));
			}
		}
		mShuffledTileCoords.Clear();
		Coord[] shuffledArray = Utility.ShuffleArray(mAllTileCoords.ToArray(), seed);
		foreach (Coord i in shuffledArray)
		{
			mShuffledTileCoords.Enqueue(i);
		}

		string mapHolderName = "Generated Map Tiles";
		Transform mapHolderTrans = this.transform.Find(mapHolderName);
		if (mapHolderTrans != null)
		{
			DestroyImmediate(mapHolderTrans.gameObject);
		}
		mapHolderTrans = new GameObject(mapHolderName).transform;
		mapHolderTrans.parent = this.transform;
		for (int x = 0; x < mapSize.x; x++)
		{
			for (int y = 0; y < mapSize.y; y++)
			{
				//Middle Center的创建方式
				Vector3 tilePos = GetPositionByCoord(x, y);
				Transform newTile = GameObject.Instantiate(tilePrefab, tilePos, Quaternion.Euler(Vector3.right * 90.0f)) as Transform;
				newTile.parent = mapHolderTrans;
				newTile.localScale = Vector3.one * (1.0f - paddingPercent / 100.0f);
			}
		}

		int obstacleCount = 10; //障碍物数量
		for (int i = 0; i < obstacleCount; i++)
		{
			Coord randomCoord = GetRandomCoord();
			Vector3 obstaclePos = GetPositionByCoord(randomCoord.x, randomCoord.y) + Vector3.up * 0.5f;
			Transform newObstacle = GameObject.Instantiate(obstaclePrefab, obstaclePos, Quaternion.identity) as Transform;
			newObstacle.parent = mapHolderTrans;
		}
	}

	private Vector3 GetPositionByCoord(int x, int y)
	{
		Vector3 pos = new Vector3(0.5f + x - mapSize.x/2, 0, 0.5f + y - mapSize.y/2);
		return pos;
	}

	public Coord GetRandomCoord()
	{
		Coord randomCoord = mShuffledTileCoords.Dequeue();
		mShuffledTileCoords.Enqueue(randomCoord);
		return randomCoord;
	}

	public struct Coord
	{
		public int x;
		public int y;
		public Coord(int _x, int _y)
		{
			x = _x;
			y = _y;
		}
	}
}