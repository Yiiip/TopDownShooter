using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MapGenerator : MonoBehaviour
{
	public Transform tilePrefab;
	public Transform obstaclePrefab;
	public Transform navmeshFloor;
	public Transform navmeshMaskPrefab;
	public Vector2 maxMapSize;
	public float tileSize = 1.0f;

	[Range(0.0f, 100.0f)]
	public float paddingPercent;
	
	public int mapIndex;
	public Map[] maps;
	private Map mCurrentMap;

	private List<Coord> mAllTileCoords = new List<Coord>();
	private Queue<Coord> mShuffledTileCoords = new Queue<Coord>();

	private void Start()
	{
		GenerateMap();
	}

	public void GenerateMap()
	{
		mCurrentMap = maps[mapIndex];
		System.Random prng = new System.Random(mCurrentMap.seed); //PRNG:伪随机数生成器
		this.GetComponent<BoxCollider>().size = new Vector3(mCurrentMap.mapSize.x * tileSize, 0.05f, mCurrentMap.mapSize.y * tileSize);

		//初始化坐标
		mAllTileCoords.Clear();
		for (int x = 0; x < mCurrentMap.mapSize.x; x++)
		{
			for (int y = 0; y < mCurrentMap.mapSize.y; y++)
			{
				mAllTileCoords.Add(new Coord(x, y));
			}
		}
		mShuffledTileCoords.Clear();
		Coord[] shuffledArray = Utility.ShuffleArray(mAllTileCoords.ToArray(), mCurrentMap.seed);
		foreach (Coord i in shuffledArray)
		{
			mShuffledTileCoords.Enqueue(i);
		}

		//创建Map holder节点
		string mapHolderName = "Generated Map Tiles";
		Transform mapHolderTrans = this.transform.Find(mapHolderName);
		if (mapHolderTrans != null)
		{
			DestroyImmediate(mapHolderTrans.gameObject);
		}
		mapHolderTrans = new GameObject(mapHolderName).transform;
		mapHolderTrans.parent = this.transform;
		//生成地面
		Vector3 tileScale = Vector3.one * (1.0f - paddingPercent / 100f) * tileSize;
		for (int x = 0; x < mCurrentMap.mapSize.x; x++)
		{
			for (int y = 0; y < mCurrentMap.mapSize.y; y++)
			{
				//Middle Center的创建方式
				Vector3 tilePos = GetPositionByCoord(x, y);
				Transform newTile = GameObject.Instantiate(tilePrefab, tilePos, Quaternion.Euler(Vector3.right * 90.0f)) as Transform;
				newTile.name = "Tile [" + x + "," + y + "]";
				newTile.parent = mapHolderTrans;
				newTile.localScale = tileScale;
			}
		}
		//生成障碍物
		int obstacleCount = (int) (mCurrentMap.obstaclePercent * mCurrentMap.mapSize.x * mCurrentMap.mapSize.y);
		int currentObstacleCount = 0;
		bool[,] obstacleMap = new bool[(int) mCurrentMap.mapSize.x, (int) mCurrentMap.mapSize.y];
		for (int i = 0; i < obstacleCount; i++)
		{
			Coord randomCoord = GetRandomCoord();
			obstacleMap[randomCoord.x, randomCoord.y] = true; //先假设障碍已经放置，然后检测它是否会影响整个通路
			currentObstacleCount++;
			if (randomCoord != mCurrentMap.mapCenter && IsMapFullyAccessible(obstacleMap, currentObstacleCount))
			{
				float obstacleHeight = Mathf.Lerp(mCurrentMap.minObstacleHeight, mCurrentMap.maxObstacleHeight, (float) prng.NextDouble());
				Vector3 obstaclePos = GetPositionByCoord(randomCoord.x, randomCoord.y) + Vector3.up * 0.5f * obstacleHeight;
				Transform newObstacle = GameObject.Instantiate(obstaclePrefab, obstaclePos, Quaternion.identity) as Transform;
				newObstacle.name = "Obstacle [" + randomCoord.x + "," + randomCoord.y + "]";
				newObstacle.parent = mapHolderTrans;
				newObstacle.localScale = new Vector3(tileScale.x, obstacleHeight, tileScale.z);

				Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
				Material obstacleMat = new Material(obstacleRenderer.sharedMaterial);
				float colorPercent = (float) randomCoord.y / (float) mCurrentMap.mapSize.y;
				obstacleMat.color = Color.Lerp(mCurrentMap.foregroundColor, mCurrentMap.backgroundColor, colorPercent);
				obstacleRenderer.sharedMaterial = obstacleMat;
			}
			else
			{
				obstacleMap[randomCoord.x, randomCoord.y] = false; //如果影响了通路的形成，就不要它了
				currentObstacleCount--;
			}
		}

		//生成Navmesh mask
		Transform maskLeft = GameObject.Instantiate(navmeshMaskPrefab, (mCurrentMap.mapSize.x + maxMapSize.x) / 4f * tileSize * Vector3.left, Quaternion.identity) as Transform;
		maskLeft.name = "Navmesh Mask Left";
		maskLeft.parent = mapHolderTrans;
		maskLeft.localScale = new Vector3((maxMapSize.x - mCurrentMap.mapSize.x) / 2f, 1.0f, mCurrentMap.mapSize.y) * tileSize;

		Transform maskRight = GameObject.Instantiate(navmeshMaskPrefab, (mCurrentMap.mapSize.x + maxMapSize.x) / 4f * tileSize * Vector3.right, Quaternion.identity) as Transform;
		maskRight.name = "Navmesh Mask Right";
		maskRight.parent = mapHolderTrans;
		maskRight.localScale = new Vector3((maxMapSize.x - mCurrentMap.mapSize.x) / 2f, 1.0f, mCurrentMap.mapSize.y) * tileSize;

		Transform maskTop = GameObject.Instantiate(navmeshMaskPrefab, (mCurrentMap.mapSize.y + maxMapSize.y) / 4f * tileSize * Vector3.forward, Quaternion.identity) as Transform;
		maskTop.name = "Navmesh Mask Top";
		maskTop.parent = mapHolderTrans;
		maskTop.localScale = new Vector3(maxMapSize.x, 1.0f, (maxMapSize.y - mCurrentMap.mapSize.y) / 2f) * tileSize;

		Transform maskBottom = GameObject.Instantiate(navmeshMaskPrefab, (mCurrentMap.mapSize.y + maxMapSize.y) / 4f * tileSize * Vector3.back, Quaternion.identity) as Transform;
		maskBottom.name = "Navmesh Mask Bottom";
		maskBottom.parent = mapHolderTrans;
		maskBottom.localScale = new Vector3(maxMapSize.x, 1.0f, (maxMapSize.y - mCurrentMap.mapSize.y) / 2f) * tileSize;

		navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y, 0) * tileSize;
	}

	private Vector3 GetPositionByCoord(int x, int y)
	{
		Vector3 pos = new Vector3(0.5f + x - mCurrentMap.mapSize.x / 2f, 0, 0.5f + y - mCurrentMap.mapSize.y / 2f) * tileSize;
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
		bool[,] mapFlags = new bool[(int) mCurrentMap.mapSize.x, (int) mCurrentMap.mapSize.y];
		Queue<Coord> queue = new Queue<Coord>();
		queue.Enqueue(mCurrentMap.mapCenter);
		mapFlags[mCurrentMap.mapCenter.x, mCurrentMap.mapCenter.y] = true;

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
						if (neighbourX >= 0 && neighbourX < (int) mCurrentMap.mapSize.x
							&& neighbourY >= 0 && neighbourY < (int) mCurrentMap.mapSize.y) //确保无越界
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
		int targetAcessibleTileCount = (int) (mCurrentMap.mapSize.x * mCurrentMap.mapSize.y - currentObstacleCount);
		return targetAcessibleTileCount == accessibleTileCount;
	}

	[System.Serializable]
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

	[System.Serializable]
	public class Map
	{
		public Coord mapSize;
		public int seed;
		[Range(0.0f, 100.0f)]
		[SerializeField]
		private float mObstaclePercent;
		public float minObstacleHeight;
		public float maxObstacleHeight;
		public Color foregroundColor;
		public Color backgroundColor;

		public Coord mapCenter
		{
			get { return new Coord(mapSize.x / 2, mapSize.y / 2); }
		}

		public float obstaclePercent
		{
			get { return mObstaclePercent / 100.0f; }
			set { mObstaclePercent = Mathf.Clamp(value, 0.0f, 100.0f); }
		}
	}
}