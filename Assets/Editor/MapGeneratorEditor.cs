using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		MapGenerator mg = target as MapGenerator;
		mg.mapIndex = Mathf.Clamp(mg.mapIndex, 0, mg.maps.Length - 1);
		foreach (MapGenerator.Map map in mg.maps)
		{
			map.mapSize.x = Mathf.Clamp(map.mapSize.x, 0, map.mapSize.x);
			map.mapSize.y = Mathf.Clamp(map.mapSize.y, 0, map.mapSize.y);
		}
		mg.GenerateMap();
	}
}