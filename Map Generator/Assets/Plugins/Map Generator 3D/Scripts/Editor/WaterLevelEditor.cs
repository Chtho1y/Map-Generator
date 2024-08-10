using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// sets the water level acording to params
/// </summary>
namespace CNB
{
	[CustomEditor(typeof(WaterLevel))]
	public class WaterLevelEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			WaterLevel waterLevel = (WaterLevel)target;

			if (DrawDefaultInspector())
			{
				if (waterLevel.autoUpdate)
				{
					waterLevel.SetWaterLevel();
				}
			}
			/*
			if (GUILayout.Button("Generate"))
			{
				waterLevel.SetWaterLevel();
			}*/
		}
	}
}