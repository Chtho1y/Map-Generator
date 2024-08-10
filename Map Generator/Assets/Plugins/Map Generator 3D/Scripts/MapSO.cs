using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CNB
{
    /// <summary>
    /// 
    /// The main philosophy of the map generator 3d is to restrict most of the user interface to this asset and the rest to be like a black box that,
    /// the most design profiles will not want to open because they feel that they have the necessary parameters to design and edit the most complex maps at a very high level of detail,
    /// and only programmers will modify. In this way errors produced by touching parts of the code without taking into account other parts that may be affected are avoided.
    /// Given this class is the core UI of our Map Generator 3d, comments are included in all layout variables.
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "New Map", menuName = "ScriptableObjects/MapSO", order = 0)]
    [System.Serializable]
    public class MapSO : ScriptableObject
    {
        //Makes the modification of variables in this "asset" show the effects in the map in real time.
        public event System.Action OnValuesUpdated;
        //Makes the modification of variables in this "asset" show the effects in the map in real time.
        public bool autoUpdate = true;

        public bool findSpawnPos = true; //FindSpawnablePos() is expensive only call if needed to spawn props

        //Sets the thickness of the walls surrounding the map.
        [Range(1, 10)]
        public int _borderMap = 1;
        //Sets the scriptable object that defines a few main main materials to set the look of the map.
        public MapMatSO _mapMatSO;

        public MapScriptO _map;

        //Class to set all the variables that define the generated map.
        [System.Serializable]
        public class MapScriptO
        {
            [Range(20, 1000)]
            public int width = 20;
            [Range(20, 1000)]
            public int height = 20;
            [Range(-3, -100)]
            public int wallsHeight = -5;
            //This parameter sets the percentage of the wall tiles represented by 1 in the binary matrix created as the starting point with the width anf height before defined.
            [Range(0, 70)]
            public int randomFillPercent = 45;

            //Sets the scale of the map
            [Range(1, 4)]
            public int squareSize = 1;

            //All open areas must reachable, if not a tunnel is created to make the needed connections. This parameter sets the width of the tunnels.
            [Range(1, 5)]
            public int tunnelRadio = 1;

            //diferent seed, diferent map.
            [Range(0, 1000)]
            public int seed = 0;

            //low values ​​generate more abrupt and angular shapes on the map surfaces and high values ​​generate more rounded shapes.
            [Range(0, 6)]
            public int smoothMapIterations = 5;

            [Range(30, 100)]
            public int lessOrganic = 50;

            //spawners
            public List<FreeCellSpawnerScriptO> _freeCellSpawners;
            public List<ColliderSpawnerScriptO> _colliderSpawners;
        }

        [System.Serializable]
        public class FreeCellSpawnerScriptO
        {
            public bool freeCells = true;
            [Range(1, 300)]
            public int spawnDendityIterationStep = 10;
            public bool rotatePrefab90Horizontal = false;
            public bool addCollider = false;
            public float GOScale = 1.0f;
            public float offsetVertical = 0;
            public bool normalToFloor = false;
            public bool distanceManaged;
            public List<GameObject> GOs;
        }

        [System.Serializable]
        public class ColliderSpawnerScriptO
        {
            public bool onTopOfTheWall = false;
            [Range(1, 300)]
            public int spawnDensityIterationStep = 10;
            public bool rotatePrefab90Horizontal = false;
            public bool addColliders = false;
            public float GOscale = 1f;
            public float offsetVertical = 0;
            public bool normalToWall = false;
            public bool distanceManaged;
            public List<GameObject> GOList = new List<GameObject>();

            //Only collider Spawner params
            public bool randomizeScale = false;
            public bool rotatePrefab90Vertical = false;
            public float offsetNormalToWall = 0;

        }

#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            if (autoUpdate)
            {
                UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
            }
        }

        public void NotifyOfUpdatedValues()
        {
            UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
            if (OnValuesUpdated != null)
            {
                OnValuesUpdated();
            }
        }
#endif
    }
}