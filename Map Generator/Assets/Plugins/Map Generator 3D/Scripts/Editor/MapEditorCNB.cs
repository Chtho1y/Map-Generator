using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CNB
{
    /// <summary>
    /// Editor class with buttons to spawn and clear spawned objects from both spawners in free cells and in map surface easily after trying different param configurations. 
    /// Turn spawned objects with colliders on/off and throws the order to load a map.
    /// </summary>
    [CustomEditor(typeof(MapGeneratorCNB))]
    public class MapEditorCNB : Editor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawProperties();
            serializedObject.ApplyModifiedProperties();
            SceneView.RepaintAll();
        }

        private void DrawProperties()
        {
            var map = target as MapGeneratorCNB;

            EditorGUILayout.Space();

            if (map != null&& map._mapSOLoadFrom!=null)
            {
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Spawn sprites in colliders"))
                {
                    map.GetComponent<MeshGeneratorCNB>().SpawnInColliders(true);
                    EditorUtility.SetDirty(target);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }
                if (GUILayout.Button("Spawn GO in free Cells"))
                {
                    map.SpawnFreeCell(true);
                    EditorUtility.SetDirty(target);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Clear Spawned Objects"))
                {
                    PoolManCNB.instance.DesactivatePoolGOs();
                    EditorUtility.SetDirty(target);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }

                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Load Map From SO"))
                {
                    map.LoadMap();
                    EditorUtility.SetDirty(target);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Switch Spawned Objects Colliders ON/OFF"))
                {
                    map._enableSpawnedObjColliders = !(map._enableSpawnedObjColliders);
                    map.EnableSpawnedObjColliders(map._spawnedHolder,map._enableSpawnedObjColliders);
                    EditorUtility.SetDirty(target);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }
            }
            EditorGUILayout.Space();
            DrawDefaultInspector();
        }
    }
}