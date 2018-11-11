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
		mg.mapSize.x = Mathf.Clamp((int) mg.mapSize.x, 0, (int) mg.mapSize.x);
		mg.mapSize.y = Mathf.Clamp((int) mg.mapSize.y, 0, (int) mg.mapSize.y);
		mg.GenerateMap();
	}
}