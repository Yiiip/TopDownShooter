using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		MapGenerator mg = target as MapGenerator;
		mg.GenerateMap();
	}
}