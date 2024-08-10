using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Sets the noise floors acording to params 
/// </summary>
namespace CNB
{
	[CustomEditor(typeof(NoiseMapGenerator))]
	public class MapGeneratorEditor : Editor
	{

		public override void OnInspectorGUI()
		{
			NoiseMapGenerator mapGen = (NoiseMapGenerator)target;

			if (DrawDefaultInspector())
			{
				if (mapGen.autoUpdate)
				{
					mapGen.SetNoiseFloors();
				}
			}
			/*
			if (GUILayout.Button("Generate"))
			{
				mapGen.SetNoiseFloors();
			}*/
		}
	}
}