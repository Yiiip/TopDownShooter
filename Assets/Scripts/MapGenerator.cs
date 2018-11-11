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
	
	[Range(0.0f, 100.0f)]
	public float obstaclePercent;

	public int seed;

	private List<Coord> mAllTileCoords = new List<Coord>();
	private Queue<Coord> mShuffledTileCoords = new Queue<Coord>();
	private Coord mMapCenter = new Coord();

	private void Start()
	{
		GenerateMap();
	}

	public void GenerateMap()
	{
		mMapCenter.x = (int) mapSize.x / 2;
		mMapCenter.y = (int) mapSize.y / 2;

		//初始化
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

		//生成地面
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
				newTile.localScale = Vector3.one * (1.0f - paddingPercent / 100f);
			}
		}
		//生成障碍物
		int obstacleCount = (int) (obstaclePercent / 100f * mapSize.x * mapSize.y);
		int currentObstacleCount = 0;
		bool[,] obstacleMap = new bool[(int) mapSize.x, (int) mapSize.y];
		for (int i = 0; i < obstacleCount; i++)
		{
			Coord randomCoord = GetRandomCoord();
			obstacleMap[randomCoord.x, randomCoord.y] = true; //先假设障碍已经放置，然后检测它是否会影响整个通路
			currentObstacleCount++;
			if (randomCoord != mMapCenter && IsMapFullyAccessible(obstacleMap, currentObstacleCount))
			{
				Vector3 obstaclePos = GetPositionByCoord(randomCoord.x, randomCoord.y) + Vector3.up * 0.5f;
				Transform newObstacle = GameObject.Instantiate(obstaclePrefab, obstaclePos, Quaternion.identity) as Transform;
				newObstacle.parent = mapHolderTrans;
			}
			else
			{
				obstacleMap[randomCoord.x, randomCoord.y] = false; //如果影响了通路的形成，就不要它了
				currentObstacleCount--;
			}
		}
	}

	private Vector3 GetPositionByCoord(int x, int y)
	{
		Vector3 pos = new Vector3(0.5f + x - mapSize.x / 2, 0, 0.5f + y - mapSize.y / 2);
		return pos;
	}

	public Coord GetRandomCoord()
	{
		Coord randomCoord = mShuffledTileCoords.Dequeue();
		mShuffledTileCoords.Enqueue(randomCoord);
		return randomCoord;
	}

	private bool IsMapFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
	{
		bool[,] mapFlags = new bool[(int) mapSize.x, (int) mapSize.y];
		Queue<Coord> queue = new Queue<Coord>();
		queue.Enqueue(mMapCenter);
		mapFlags[mMapCenter.x, mMapCenter.y] = true;

		int accessibleTileCount = 1;

		while (queue.Count > 0) //检测通路
		{
			Coord tile = queue.Dequeue();
			for (int horizontal = -1; horizontal <= 1; horizontal++)
			{
				for (int verticle = -1; verticle <= 1; verticle++)
				{
					int neighbourX = tile.x + horizontal;
					int neighbourY = tile.y + verticle;
					if (horizontal == 0 || verticle == 0) //只需要看上下左右
					{
						if (neighbourX >= 0 && neighbourX < (int) mapSize.x
							&& neighbourY >= 0 && neighbourY < (int) mapSize.y) //确保无越界
						{
							if (mapFlags[neighbourX, neighbourY] == false && obstacleMap[neighbourX, neighbourY] == false)
							{
								mapFlags[neighbourX, neighbourY] = true;
								queue.Enqueue(new Coord(neighbourX, neighbourY));
								accessibleTileCount++;
							}
						}
					}
				}
			}
		}
		int targetAcessibleTileCount = (int) (mapSize.x * mapSize.y - currentObstacleCount);
		return targetAcessibleTileCount == accessibleTileCount;
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
		public override bool Equals(object obj)
		{
			if (obj != null && this.GetType() == obj.GetType())
			{
				if (((Coord) obj).x == this.x && ((Coord) obj).y == this.y)
				{
					return true;
				}
			}
			return false;
		}
		public override int GetHashCode()
		{
			return this.x.GetHashCode();
		}
		public static bool operator == (Coord c1, Coord c2)
		{
			return (c1.x == c2.x && c1.y == c2.y);
		}
		public static bool operator != (Coord c1, Coord c2)
		{
			return !(c1 == c2);
		}
	}
}