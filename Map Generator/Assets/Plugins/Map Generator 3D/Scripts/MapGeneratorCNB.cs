using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;

namespace CNB
{
    /// <summary>
    /// Generates a new  "map" and a new "mesh" from the data in "_mapSOLoadFrom" 
    /// </summary>
    [ExecuteAlways]
    public class MapGeneratorCNB : MonoBehaviour
    {

        [Serializable]
        public class Map
        {
            public int _width;
            public int _height;
            public int _wallsHeight;
            public int _squareSize;

            public string _seed;

            public int _tunnelRadio;

            public int _smoothMapIterations;

            public int _caseFourChance;

            public int _randomFillPercent;

            public Material _floorMat;
            public Material _meshMat;
            public Material _wallsMat;
            public Material _waterMat;
            public Material _limitsMatUp;
            public Material _limitsTopUp;
            public Material _limitsMatDown;
            public Material _limitsTopDown;

            public int _meshMaterialTileX;
            public int _floorMaterialTileX;
            public int _wallsMaterialTileX;
            public int _waterMaterialTileX;
            public int _mapLimitsWallsUpTileX;
            public int _mapLimitsTopUpTileX;
            public int _mapLimitsWallsDownTileX;
            public int _mapLimitsTopDownTileX;
        }

        public bool _exeYVertival;//if false exe Z vertical

        [HideInInspector]
        public Map _map;

        [HideInInspector]
        public int _borderMap;

        [HideInInspector]
        public GameObject _mapMesh;
        [HideInInspector]
        public List<GameObject> _mapNoiseFloors=new List<GameObject>();
        [HideInInspector]
        public GameObject _mapWalls;
        [HideInInspector]
        public GameObject _water;
        [HideInInspector]
        public GameObject _spawnedHolder;
        [HideInInspector]
        public GameObject _mapBounds;
        [HideInInspector]
        public GameObject _mapLimitsUp;
        [HideInInspector]
        public GameObject _mapLimitsDown;
        
        List<HashSet<Coord>> _freepos;
        
        public int[,] _mapArray;

        //SO
        public MapSO _mapSOLoadFrom;

        [HideInInspector]
        public MeshGeneratorCNB _meshGen;

        public bool _spawnFreeCellBool = false;

        List<SpawnerInMapCells> _spawnersFC = new List<SpawnerInMapCells>();

        List<string> _tagsNeeded = new List<string>();
        bool _tagsInitialized = false;
        List<string> _layersNeeded = new List<string>();
        bool _layersInitialized = false;

        [HideInInspector]
        public int _freeCellSpawnersCount = 0;
        [HideInInspector]
        public int _freeCellSpawnedGOGlobalCount = 0;

        [HideInInspector]
        public List<Vector2> spawnablePointsFloor = new List<Vector2>();
        [HideInInspector]
        public List<Vector2> spawnablePointsTop = new List<Vector2>();

        List<Vector2> copyPoints = new List<Vector2>();

        HashSet<Coord> passageCreatedCoords = new HashSet<Coord>();
        HashSet<Coord> allregionCoordsWithPasagesCreated = new HashSet<Coord>();

        float textureScaleYMeshAnfFloor = 1;
        float wallsPerimeter = 1;
        float limitWallsPerimeter = 1;

        [HideInInspector]
        public bool _enableSpawnedObjColliders = false;

        public bool _mapWallLimitsUp = true;
        [Range(20, 100)]
        public int _mapWallLimitsUpHeight=5;

        public bool _mapWallLimitsDown = true;
        [Range(20, 100)]
        public int _mapWallLimitsDownHeight=5;

        [HideInInspector]
        public List<MeshCollider> _spawnedObjsCollMapCells = new List<MeshCollider>();
        [HideInInspector]
        public List<MeshCollider> _spawnedObjsCollMapSurfaces = new List<MeshCollider>();

        public void LoadMap()
        {
            CheckTags();
            
            if (GameObject.FindGameObjectWithTag("SpawnManager") == null)
            {
                GameObject SpawnManager = new GameObject("SpawnManager");
                SpawnManager.AddComponent<PoolManCNB>();
                SpawnManager.transform.tag = "SpawnManager";
                _spawnedHolder = SpawnManager;
            }
            else
            {
                _spawnedHolder = GameObject.FindGameObjectWithTag("SpawnManager");
            }
            
            SpawnerInMapCells[] spawnersMC = GetComponents<SpawnerInMapCells>();
            foreach (var item in spawnersMC)
            {
                DestroyImmediate(item,true);
            }

            SpawnerInMapSurfaces[] spawnersSF = GetComponents<SpawnerInMapSurfaces>();
            foreach (var item in spawnersSF)
            {
                DestroyImmediate(item, true);
            }

            if (_mapSOLoadFrom != null && _meshGen != null)
            {
                MapSO.MapScriptO map = _mapSOLoadFrom._map;
                _borderMap = _mapSOLoadFrom._borderMap;
                _spawnersFC.Clear();
                _meshGen._spawnerColliders.Clear();
                
                for (int i = 0; i < _spawnersFC.Count; i++)
                {
                    DestroyImmediate(_spawnersFC[i], true);
                }
                
                _map = new Map();
                
                MapSO.MapScriptO _mapSOScriptO = _mapSOLoadFrom._map;
                //map
                _map._width = _mapSOScriptO.width;
                _map._height = _mapSOScriptO.height;
                _map._wallsHeight = _mapSOScriptO.wallsHeight;
                _map._squareSize = _mapSOScriptO.squareSize;
                _map._seed = _mapSOScriptO.seed.ToString();
                _map._tunnelRadio = _mapSOScriptO.tunnelRadio;
                _map._smoothMapIterations = _mapSOScriptO.smoothMapIterations;
                _map._caseFourChance = _mapSOScriptO.lessOrganic;
                if (_mapSOLoadFrom._mapMatSO == null)
                {
                    Debug.LogWarning("MapMatSO scriptable object must be created and asigned in MapSO");
                }
                _map._floorMat = _mapSOLoadFrom._mapMatSO._floorMaterial;
                _map._floorMaterialTileX = _mapSOLoadFrom._mapMatSO._floorMaterialTileX;

                _map._meshMat = _mapSOLoadFrom._mapMatSO._meshMaterial;
                _map._meshMaterialTileX = _mapSOLoadFrom._mapMatSO._meshMaterialTileX;

                _map._wallsMat = _mapSOLoadFrom._mapMatSO._wallsMaterial;
                _map._wallsMaterialTileX = _mapSOLoadFrom._mapMatSO._wallsMaterialTileX;

                _map._waterMat = _mapSOLoadFrom._mapMatSO._waterMaterial;
                _map._waterMaterialTileX = _mapSOLoadFrom._mapMatSO._waterMaterialTileX;

                _map._limitsMatUp = _mapSOLoadFrom._mapMatSO._mapLimitsWallsUp;
                _map._mapLimitsWallsUpTileX = _mapSOLoadFrom._mapMatSO._mapLimitsWallsUpTileX;

                _map._limitsTopUp = _mapSOLoadFrom._mapMatSO._mapLimitsTopUp;
                _map._mapLimitsTopUpTileX = _mapSOLoadFrom._mapMatSO._mapLimitsTopUpTileX;

                _map._limitsMatDown = _mapSOLoadFrom._mapMatSO._mapLimitsWallsDown;
                _map._mapLimitsWallsDownTileX = _mapSOLoadFrom._mapMatSO._mapLimitsWallsDownTileX;

                _map._limitsTopDown = _mapSOLoadFrom._mapMatSO._mapLimitsTopDown;
                _map._mapLimitsTopDownTileX = _mapSOLoadFrom._mapMatSO._mapLimitsTopDownTileX;

                _map._randomFillPercent = _mapSOScriptO.randomFillPercent;

                //spawners
                SpawnerInMapCells.maxPointsInitialized = false;
                for (int j = 0; j < _mapSOScriptO._freeCellSpawners.Count; j++)
                {
                    SpawnerInMapCells newSP = gameObject.AddComponent<SpawnerInMapCells>();
                    _spawnersFC.Add(newSP);
                    newSP.freeCells = _mapSOScriptO._freeCellSpawners[j].freeCells;
                    newSP.iterationStep = _mapSOScriptO._freeCellSpawners[j].spawnDendityIterationStep;
                    newSP.rotPrefab90 = _mapSOScriptO._freeCellSpawners[j].rotatePrefab90Horizontal;
                    newSP.addCollider = _mapSOScriptO._freeCellSpawners[j].addCollider;
                    newSP.GOscale = _mapSOScriptO._freeCellSpawners[j].GOScale;
                    newSP.offSetVertical = _mapSOScriptO._freeCellSpawners[j].offsetVertical;
                    newSP.normalToFloor = _mapSOScriptO._freeCellSpawners[j].normalToFloor;
                    newSP.distanceManaged = _mapSOScriptO._freeCellSpawners[j].distanceManaged;


                    newSP._GOs = new List<GameObject>();
                    for (int k = 0; k < _mapSOScriptO._freeCellSpawners[j].GOs.Count; k++)
                    {
                        newSP._GOs.Add(_mapSOScriptO._freeCellSpawners[j].GOs[k]);
                    }
                }

                SpawnerInMapSurfaces.allSpawnPosAndNormalCollected = false;
                for (int l = 0; l < _mapSOScriptO._colliderSpawners.Count; l++)
                {
                    SpawnerInMapSurfaces newCS = gameObject.AddComponent<SpawnerInMapSurfaces>();
                   
                    _meshGen._spawnerColliders.Add(newCS);
                    _meshGen._spawnerColliders[l].spawnDensity = _mapSOScriptO._colliderSpawners[l].spawnDensityIterationStep;
                    _meshGen._spawnerColliders[l].rotPrefabInitial = _mapSOScriptO._colliderSpawners[l].rotatePrefab90Horizontal;
                    _meshGen._spawnerColliders[l].rotPrefab90InVerticalExe = _mapSOScriptO._colliderSpawners[l].rotatePrefab90Vertical;
                    _meshGen._spawnerColliders[l].randomizeScale = _mapSOScriptO._colliderSpawners[l].randomizeScale;
                    _meshGen._spawnerColliders[l].addCollider = _mapSOScriptO._colliderSpawners[l].addColliders;
                    _meshGen._spawnerColliders[l].offsetNormalToWall = _mapSOScriptO._colliderSpawners[l].offsetNormalToWall;
                    _meshGen._spawnerColliders[l].offsetVertical = _mapSOScriptO._colliderSpawners[l].offsetVertical;
                    _meshGen._spawnerColliders[l].GOScale = _mapSOScriptO._colliderSpawners[l].GOscale;
                    _meshGen._spawnerColliders[l].onTopOfTheW = _mapSOScriptO._colliderSpawners[l].onTopOfTheWall;
                    _meshGen._spawnerColliders[l].normalToWall = _mapSOScriptO._colliderSpawners[l].normalToWall;
                    _meshGen._spawnerColliders[l].distanceManaged = _mapSOScriptO._colliderSpawners[l].distanceManaged;


                    _meshGen._spawnerColliders[l].GOList = new List<GameObject>();
                    for (int m = 0; m < _mapSOScriptO._colliderSpawners[l].GOList.Count; m++)
                    {
                        if (_mapSOScriptO._colliderSpawners[l].GOList[m] != null)
                        {
                            _meshGen._spawnerColliders[l].GOList.Add(_mapSOScriptO._colliderSpawners[l].GOList[m]);
                        }
                    }
                }
            }
            
            GenerateMap();
        }

        private void Awake()
        {
            PrefabInstanceStatus isPrefab = PrefabUtility.GetPrefabInstanceStatus(this);
            if ((isPrefab==PrefabInstanceStatus.NotAPrefab)==false)
            {
                PrefabUtility.UnpackPrefabInstance(this.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }

            _tagsNeeded.Clear();
            _layersNeeded.Clear();

            _tagsNeeded.Add("DistanceManaged");
            _tagsNeeded.Add("SpawnManager");
            _tagsNeeded.Add("Line Renderer");
            _tagsNeeded.Add("Map");
            _tagsNeeded.Add("MapSO");
            _tagsNeeded.Add("MapBounds");
            _tagsNeeded.Add("Walls");
            _tagsNeeded.Add("NoiseFloor");
            _tagsNeeded.Add("Water");

            _layersNeeded.Add("Collisions");

            if (!_tagsInitialized)
            {
                Initializetags();
                _tagsInitialized = true;
            }

            if (!_layersInitialized)
            {
                InitializeLayers();
                _layersInitialized = true;
            }
        }

        private void InitializeLayers()
        {
#if UNITY_EDITOR
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            bool foundLayer = false;
            foreach (var layer in _layersNeeded)
            {
                for (int i = 0; i < layersProp.arraySize; i++)
                {
                    SerializedProperty t = layersProp.GetArrayElementAtIndex(i);
                    if (t.stringValue.Equals(layer)) { foundLayer = true; break; }
                }

                if (!foundLayer)
                {
                    SerializedProperty slot = null;
                    for (int i = 8; i <= 31; i++)
                    {
                        SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
                        if (sp != null && string.IsNullOrEmpty(sp.stringValue))
                        {
                            slot = sp;
                            break;
                        }
                    }

                    if (slot != null)
                    {
                        slot.stringValue = layer;
                    }
                    else
                    {
                        Debug.LogError("Could not find an open Layer Slot for: " + name);
                    }

                }
                foundLayer = true;
            }

            tagManager.ApplyModifiedProperties();

            if (GameObject.FindGameObjectWithTag("MapSO")!=null&&foundLayer)
            {
                GameObject.FindGameObjectWithTag("MapSO").layer =LayerMask.NameToLayer("Collisions");
            }
#endif
        }
        
            private void Initializetags()
        {
#if UNITY_EDITOR
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            bool foundTag = false;
            foreach (var tag in _tagsNeeded)
            {
                for (int i = 0; i < tagsProp.arraySize; i++)
                {
                    foundTag = false;    
                    SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                    if (t.stringValue.Equals(tag)) 
                    { 
                        foundTag = true; 
                        break; 
                    }
                }

                if (!foundTag)
                {
                    tagsProp.InsertArrayElementAtIndex(0);
                    SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                    n.stringValue = tag;
                }
            }

            tagManager.ApplyModifiedProperties();

#endif
        }

        void CheckTags()
        {
            if (this.gameObject.CompareTag("MapSO") == false)
            {
                this.gameObject.tag = "MapSO";
            }
            if (_mapMesh!=null&&_mapMesh.CompareTag("Map")==false)
            {
                _mapMesh.tag = "Map";
            }
            if (_mapNoiseFloors != null && _mapNoiseFloors.Count>0) 
            {
                foreach (var item in _mapNoiseFloors)
                {
                    if (item!=null&&item.CompareTag("NoiseFloor") == false)
                    {
                        item.tag = "NoiseFloor";
                    }
                }
            }
            if (_mapWalls != null && _mapWalls.CompareTag("Walls") == false)
            {
                _mapWalls.tag = "Walls";
            }
            if (_water != null && _water.CompareTag("Water") == false)
            {
                _water.tag = "Water";
            }
            if (_spawnedHolder != null && _spawnedHolder.CompareTag("SpawnManager") == false)
            {
                _spawnedHolder.tag = "SpawnManager";
            }
            if (_mapBounds != null && _mapBounds.CompareTag("MapBounds") == false)
            {
                _mapBounds.tag = "MapBounds";
            }
            if (_mapMesh != null && _mapMesh.CompareTag("Map") == false)
            {
                _mapMesh.tag = "Map";
            }
        }

        void OnValuesUpdated()
        {
            if (_mapSOLoadFrom != null && this != null) //importannte check. Si no al salvar la escena this ==null
            {
                LoadMap();
            }
        }

        void OnValidate()
        {
            if (_mapSOLoadFrom!=null)
            {
                _mapSOLoadFrom.OnValuesUpdated -= OnValuesUpdated;

                if (_mapSOLoadFrom != null)
                {
                    _mapSOLoadFrom.OnValuesUpdated += OnValuesUpdated;
                }
            }
        }

        void SetMapBoundsAndFloorLayers()
        {
            BoxCollider col = _mapBounds.GetComponent<BoxCollider>();
            if (col)
            {
                _mapBounds.GetComponent<BoxCollider>().size = new Vector3(_map._width + _borderMap * 2 -1, _map._height + _borderMap * 2 -1, 400) * _map._squareSize; 

            }

            if (_mapNoiseFloors[0].GetComponent<NoiseMapGenerator>() != null)
            {
                _mapNoiseFloors[0].GetComponent<NoiseMapGenerator>().SetNoiseFloors();
            }

            if (GameObject.FindGameObjectWithTag("Water")!=null)
            {
                GameObject.FindGameObjectWithTag("Water").GetComponent<WaterLevel>().SetWaterLevel();
            }
            
            
        }

        public void GenerateMap()
        {
            Quaternion rot;
            if (_exeYVertival)
            {
                rot = Quaternion.Euler(Vector3.right * -90);
            }
            else
            {
                rot = Quaternion.Euler(Vector3.zero);
            }
            transform.rotation = rot;

            spawnablePointsFloor.Clear();
            spawnablePointsTop.Clear();
            copyPoints.Clear();
            passageCreatedCoords.Clear();
            allregionCoordsWithPasagesCreated.Clear();
            if (PoolManCNB.instance!=null)
            {
                PoolManCNB.instance.DesactivatePoolGOs();
            }

            SetMapBoundsAndFloorLayers();
            _mapArray = new int[_map._width, _map._height];
            RandomFillMap();

            for (int i = 0; i < _map._smoothMapIterations; i++)
            {
                SmoothMap(_map._caseFourChance);
            }

            ProcessMap();

            int[,] borderedMap = new int[_map._width + _borderMap * 2, _map._height + _borderMap * 2];

            for (int x = 0; x < borderedMap.GetLength(0); x++)
            {
                for (int y = 0; y < borderedMap.GetLength(1); y++)
                {
                    if (x >= _borderMap && x < _map._width + _borderMap && y >= _borderMap && y < _map._height + _borderMap)
                    {
                        borderedMap[x, y] = _mapArray[x - _borderMap, y - _borderMap];
                    }
                    else
                    {
                        borderedMap[x, y] = 1;
                    }
                }
            }

            _meshGen.GenerateMesh(borderedMap, _map._squareSize);

            textureScaleYMeshAnfFloor = (float)_map._width / _map._height;

            if (_mapSOLoadFrom.findSpawnPos)
            {
                FindSpawnablePos();//freecall spawner method only once per map

                SpawnFreeCell(_spawnFreeCellBool);
            }

            if (_mapMesh != null)
            {
                Renderer rend = _mapMesh.GetComponent<Renderer>();
                if (_map._meshMat != null)
                {
                    rend.sharedMaterial = _map._meshMat;
                    int tileX = _map._meshMaterialTileX; 
                    rend.sharedMaterial.mainTextureScale = new Vector2(tileX, tileX / textureScaleYMeshAnfFloor);
                }
            }

            if (_mapNoiseFloors.Count > 0)
            {
                foreach (var item in _mapNoiseFloors)
                {
                    if (item != null)
                    {
                        Renderer rend = item.GetComponent<Renderer>();
                        if (_map._floorMat != null)
                        {
                            rend.sharedMaterial = _map._floorMat;
                            int tileX = _map._floorMaterialTileX;
                            rend.sharedMaterial.mainTextureScale = new Vector2(tileX, tileX / textureScaleYMeshAnfFloor);
                        }
                    }
                }
            }

            if (_mapWalls != null)
            {
                Renderer rend = _mapWalls.GetComponent<Renderer>();
                rend.sharedMaterial = _map._wallsMat;
                if (_mapWalls.GetComponent<MeshFilter>().sharedMesh != null)
                {
                    if (_map._wallsMat != null)
                    {
                        int tileX = _map._wallsMaterialTileX;
                        wallsPerimeter = _meshGen._limitWallPerimeter;
                        rend.sharedMaterial.mainTextureScale = new Vector2(1, 1 / wallsPerimeter) * tileX;
                    }
                }
            }

            if (_water != null)
            {
                Renderer rend = _water.GetComponent<Renderer>();
                if (_map._waterMat != null)
                {
                    rend.sharedMaterial = _map._waterMat;
                    int tileX = _map._waterMaterialTileX;
                    rend.sharedMaterial.mainTextureScale = new Vector2(tileX, tileX / textureScaleYMeshAnfFloor);
                }
            }

            if (_mapLimitsUp != null)
            {
                if (_mapWallLimitsUp)
                {
                    _mapLimitsUp.SetActive(true);
                    Renderer rend = _mapLimitsUp.GetComponent<Renderer>();
                    rend.sharedMaterial = _map._limitsMatUp;
                    if (_map._limitsMatUp != null)
                    {
                        if (_mapLimitsUp.GetComponent<MeshFilter>().sharedMesh != null)
                        {
                            int tileX = _map._mapLimitsWallsUpTileX;
                            limitWallsPerimeter = _mapLimitsUp.GetComponent<WallLimits>()._limitWallPerimeter;
                            rend.sharedMaterial.mainTextureScale = new Vector2(1, 1 / limitWallsPerimeter) * tileX;
                        }

                        Transform top = _mapLimitsUp.transform.GetChild(0);
                        if (top && _map._limitsTopUp)
                        {
                            Renderer rendTop = top.GetComponent<Renderer>();
                            rendTop.sharedMaterial = _map._limitsTopUp;
                            int tileX = _map._mapLimitsTopUpTileX;
                            rendTop.sharedMaterial.mainTextureScale = new Vector2(tileX, tileX / textureScaleYMeshAnfFloor);
                            Vector3 newPos = new Vector3(0, 0, _mapWallLimitsUpHeight);
                            top.transform.localPosition = newPos;
                        }
                    }
                }
                else
                {
                    _mapLimitsUp.SetActive(false);
                }
            }
            if (_mapLimitsDown != null)
            {
                if (_mapWallLimitsDown)
                {
                    _mapLimitsDown.SetActive(true);
                    Renderer rend = _mapLimitsDown.GetComponent<Renderer>();
                    rend.sharedMaterial = _map._limitsMatDown;
                    if (_map._limitsMatDown != null)
                    {
                        if (_mapLimitsDown.GetComponent<MeshFilter>().sharedMesh != null)
                        {
                            int tileX = _map._mapLimitsWallsDownTileX;
                            limitWallsPerimeter = _mapLimitsDown.GetComponent<WallLimits>()._limitWallPerimeter;
                            rend.sharedMaterial.mainTextureScale = new Vector2(1, 1 / limitWallsPerimeter) * tileX;
                        }

                        Transform top = _mapLimitsDown.transform.GetChild(0);
                        if (top && _map._limitsTopDown)
                        {
                            Renderer rendTop = top.GetComponent<Renderer>();
                            rendTop.sharedMaterial = _map._limitsTopDown;
                            int tileX = _map._mapLimitsTopDownTileX;
                            rendTop.sharedMaterial.mainTextureScale = new Vector2(tileX, tileX / textureScaleYMeshAnfFloor);
                            Vector3 newPos = new Vector3(0, 0, 0);
                            top.transform.localPosition = newPos;
                        }
                    }
                }
                else
                {
                    _mapLimitsDown.SetActive(false);
                }
            }
        }

        public void EnableSpawnedObjColliders(GameObject spawnedHolder, bool enable)
        {
            foreach (var item in _spawnedObjsCollMapCells)
            {
                if (item==null)
                {
                    DestroyImmediate(item);
                }
                else
                {
                    item.enabled = enable;
                }
            }

            foreach (var item in _spawnedObjsCollMapSurfaces)
            {
                if (item == null)
                {
                    DestroyImmediate(item);
                }
                else
                {
                    item.enabled = enable;
                }
            }
        }

        public void SpawnFreeCell(bool _spawnFreeCellBool)
        {
            _freeCellSpawnersCount = 0;
            _spawnedObjsCollMapCells.Clear();

            if (_spawnFreeCellBool)
            {
                if (_spawnersFC != null)
                {
                    foreach (var test in _spawnersFC)
                    {
                        SpawnerInMapCells t = test;
                        if (t != null)
                        {
                            t.Init(_map._squareSize);
                            t.Spawn(t.freeCells, _exeYVertival);
                            _freeCellSpawnersCount++;
                        }
                    }
                }
            }
        }

        void ProcessMap()
        {
            List<HashSet<Coord>> wallRegions = GetRegions(1);
            int wallThresholdSize = 50;

            foreach (HashSet<Coord> wallRegion in wallRegions)
            {
                if (wallRegion.Count < wallThresholdSize)
                {
                    foreach (Coord tile in wallRegion)
                    {
                        _mapArray[tile.tileX, tile.tileY] = 0;
                    }
                }
            }

            List<HashSet<Coord>> roomRegions = GetRegions(0);
            int roomThresholdSize = 20;
            List<Room> survivingRooms = new List<Room>();

            foreach (HashSet<Coord> roomRegion in roomRegions)
            {
                if (roomRegion.Count < roomThresholdSize)
                {
                    foreach (Coord tile in roomRegion)
                    {
                        _mapArray[tile.tileX, tile.tileY] = 1;
                    }
                }
                else
                {
                    survivingRooms.Add(new Room(roomRegion, _mapArray));
                }
            }
            survivingRooms.Sort();
            if (survivingRooms != null && survivingRooms.Count > 0)
            {
                survivingRooms[0].isMainRoom = true;
                survivingRooms[0].isAccessibleFromMainRoom = true;
            }

            ConnectClosestRooms(survivingRooms);

            _freepos = GetFreePositions();
        }

        List<HashSet<Coord>> GetFreePositions()
        {
            List<HashSet<Coord>>freePositions = GetRegions(0);
            freePositions.Add(passageCreatedCoords);
            return freePositions;
        }

        void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
        {

            List<Room> roomListA = new List<Room>();
            List<Room> roomListB = new List<Room>();

            if (forceAccessibilityFromMainRoom)
            {
                foreach (Room room in allRooms)
                {
                    if (room.isAccessibleFromMainRoom)
                    {
                        roomListB.Add(room);
                    }
                    else
                    {
                        roomListA.Add(room);
                    }
                }
            }
            else
            {
                roomListA = allRooms;
                roomListB = allRooms;
            }

            int bestDistance = 0;
            Coord bestTileA = new Coord();
            Coord bestTileB = new Coord();
            Room bestRoomA = new Room();
            Room bestRoomB = new Room();
            bool possibleConnectionFound = false;

            foreach (Room roomA in roomListA)
            {
                if (!forceAccessibilityFromMainRoom)
                {
                    possibleConnectionFound = false;
                    if (roomA.connectedRooms.Count > 0)
                    {
                        continue;
                    }
                }

                foreach (Room roomB in roomListB)
                {
                    if (roomA == roomB || roomA.IsConnected(roomB))
                    {
                        continue;
                    }

                    for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                    {
                        for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                        {
                            Coord tileA = roomA.edgeTiles[tileIndexA];
                            Coord tileB = roomB.edgeTiles[tileIndexB];
                            int distanceBetweenRooms = (int)((tileA.tileX - tileB.tileX)*(tileA.tileX - tileB.tileX) + (tileA.tileY - tileB.tileY)* (tileA.tileY - tileB.tileY));

                            if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                            {
                                bestDistance = distanceBetweenRooms;
                                possibleConnectionFound = true;
                                bestTileA = tileA;
                                bestTileB = tileB;
                                bestRoomA = roomA;
                                bestRoomB = roomB;
                            }
                        }
                    }
                }
                if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
                {
                    CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
                }
            }

            if (possibleConnectionFound && forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
                ConnectClosestRooms(allRooms, true);
            }

            if (!forceAccessibilityFromMainRoom)
            {
                ConnectClosestRooms(allRooms, true);
            }
        }

        void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
        {
            Room.ConnectRooms(roomA, roomB);
            //Debug.DrawLine (CoordToWorldPoint (tileA), CoordToWorldPoint (tileB), Color.green, 100);

            List<Coord> line = GetLine(tileA, tileB);
            foreach (Coord c in line)
            {
                DrawCircle(c, _map._tunnelRadio);
                passageCreatedCoords.Add(c);
            }
        }

        void DrawCircle(Coord c, int r)
        {
            for (int x = -r; x <= r; x++)
            {
                for (int y = -r; y <= r; y++)
                {
                    if (x * x + y * y <= r * r)
                    {
                        int drawX = c.tileX + x;
                        int drawY = c.tileY + y;
                        if (IsInMapRange(drawX, drawY))
                        {
                            _mapArray[drawX, drawY] = 0;
                        }
                    }
                }
            }
        }

        List<Coord> GetLine(Coord from, Coord to)
        {
            List<Coord> line = new List<Coord>();

            int x = from.tileX;
            int y = from.tileY;

            int dx = to.tileX - from.tileX;
            int dy = to.tileY - from.tileY;

            bool inverted = false;
            int step = Math.Sign(dx);
            int gradientStep = Math.Sign(dy);

            int longest = Mathf.Abs(dx);
            int shortest = Mathf.Abs(dy);

            if (longest < shortest)
            {
                inverted = true;
                longest = Mathf.Abs(dy);
                shortest = Mathf.Abs(dx);

                step = Math.Sign(dy);
                gradientStep = Math.Sign(dx);
            }

            int gradientAccumulation = longest / 2;
            for (int i = 0; i < longest; i++)
            {
                line.Add(new Coord(x, y));

                if (inverted)
                {
                    y += step;
                }
                else
                {
                    x += step;
                }

                gradientAccumulation += shortest;
                if (gradientAccumulation >= longest)
                {
                    if (inverted)
                    {
                        x += gradientStep;
                    }
                    else
                    {
                        y += gradientStep;
                    }
                    gradientAccumulation -= longest;
                }
            }

            return line;
        }

        Vector2 CoordToWorldPoint(Coord tile)
        {
            return new Vector2(-_map._width / 2 + .5f + tile.tileX, -_map._height / 2 + .5f + tile.tileY) * _map._squareSize;
        }

        //...........
        public Coord WorldPointToCoord(Vector2 worldPoint)
        {
            Coord coord = new Coord(Mathf.RoundToInt(worldPoint.x + _map._width / 2 - .5f), Mathf.RoundToInt(worldPoint.y + _map._height / 2 - .5f));
            return coord;
        }

        public bool spawnablePos(Coord coord)
        {
            if (_freepos != null)
            {
                foreach (HashSet<Coord> list in _freepos)
                {
                    if (list.Contains(coord))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //..............................test
        void FindSpawnablePos()
        {
            spawnablePointsFloor.Clear();
            spawnablePointsTop.Clear();
            copyPoints.Clear();
            if (_spawnersFC.Count>0)
            {
                SpawnerInMapCells sp = _spawnersFC[0];

                if (sp != null)
                {
                    sp.Init(_map._squareSize);
                    List<Vector2> AllPos = sp.points;
                    copyPoints = new List<Vector2>();
                    foreach (var point in AllPos)
                    {
                        copyPoints.Add(point - sp.regionSize / 2);
                    }
                    AllPos.Clear();
                    foreach (var point in copyPoints)
                    {
                        if (spawnablePos(WorldPointToCoord(point / _map._squareSize)))
                        {
                            spawnablePointsFloor.Add(point);
                        }
                        else
                        {
                            spawnablePointsTop.Add(point);
                        }
                    }
                }
            }
        }
        //.............
        List<HashSet<Coord>> GetRegions(int tileType)
        {
            
            List<HashSet<Coord>> regions = new List<HashSet<Coord>>();
            int[,] mapFlags = new int[_map._width, _map._height];
            HashSet<Coord> newRegion;

            for (int x = 0; x < _map._width; x++)
            {
                for (int y = 0; y < _map._height; y++)
                {
                    if (mapFlags[x, y] == 0 && _mapArray[x, y] == tileType)
                    {
                        newRegion = GetRegionTiles(x, y);
                        regions.Add(newRegion);

                        foreach (Coord tile in newRegion)
                        {
                            mapFlags[tile.tileX, tile.tileY] = 1;
                        }
                    }
                }
            }

            return regions;
        }

        HashSet<Coord> GetRegionTiles(int startX, int startY)
        {
            List<Coord> tiles = new List<Coord>();
            int[,] mapFlags = new int[_map._width, _map._height];
            int tileType = _mapArray[startX, startY];

            Queue<Coord> queue = new Queue<Coord>();
            queue.Enqueue(new Coord(startX, startY));
            mapFlags[startX, startY] = 1;

            Coord dequedCoord;
            Coord enquedCoord;
            while (queue.Count > 0)
            {
                dequedCoord = queue.Dequeue();
                tiles.Add(dequedCoord);

                for (int x = dequedCoord.tileX - 1; x <= dequedCoord.tileX + 1; x++)
                {
                    for (int y = dequedCoord.tileY - 1; y <= dequedCoord.tileY + 1; y++)
                    {
                        if (IsInMapRange(x, y) && (y == dequedCoord.tileY || x == dequedCoord.tileX))
                        {
                            if (mapFlags[x, y] == 0 && _mapArray[x, y] == tileType)
                            {
                                mapFlags[x, y] = 1;
                                enquedCoord.tileX = x;
                                enquedCoord.tileY = y;
                                queue.Enqueue(enquedCoord);
                            }
                        }
                    }
                }
            }
            return tiles.ToHashSet<Coord>();
        }

        bool IsInMapRange(int x, int y)
        {
            return x >= 0 && x < _map._width && y >= 0 && y < _map._height;
        }


        void RandomFillMap()
        {
            System.Random pseudoRandom = new System.Random(_map._seed.GetHashCode());

            for (int x = 0; x < _map._width; x++)
            {
                for (int y = 0; y < _map._height; y++)
                {
                    if (x == 0 || x == _map._width - 1 || y == 0 || y == _map._height - 1)
                    {
                        _mapArray[x, y] = 1;
                    }
                    else
                    {
                        _mapArray[x, y] = pseudoRandom.Next(0, 100) < _map._randomFillPercent ? 1 : 0;
                    }
                }
            }
        }

        void SmoothMap(int caseFourChance)
        {
            System.Random pseudoRandom = new System.Random(_map._seed.GetHashCode());
            
            for (int x = 0; x < _map._width; x++)
            {
                for (int y = 0; y < _map._height; y++)
                {
                    int neighbourWallTiles = GetSurroundingWallCount(x, y);

                    if (neighbourWallTiles > 4)
                        _mapArray[x, y] = 1;
                    else if (neighbourWallTiles < 4)
                        _mapArray[x, y] = 0;
                    else
                    {
                        if (pseudoRandom.Next(0, 100)>caseFourChance)
                        {
                            _mapArray[x, y] = 1;
                        }
                        else
                        {
                            _mapArray[x, y] = 0;

                        }
                    }
                }
            }
        }

        int GetSurroundingWallCount(int gridX, int gridY)
        {
            int wallCount = 0;
            for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
            {
                for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
                {
                    if (IsInMapRange(neighbourX, neighbourY))
                    {
                        if (neighbourX != gridX || neighbourY != gridY)
                        {
                            wallCount += _mapArray[neighbourX, neighbourY];
                        }
                    }
                    else
                    {
                        wallCount++;
                    }
                }
            }

            return wallCount;
        }

        public struct Coord
        {
            public int tileX;
            public int tileY;

            public Coord(int x, int y)
            {
                tileX = x;
                tileY = y;
            }
        }


        class Room : IComparable<Room>
        {
            public HashSet<Coord> tiles;
            public List<Coord> edgeTiles;
            public List<Room> connectedRooms;
            public int roomSize;
            public bool isAccessibleFromMainRoom;
            public bool isMainRoom;

            public Room()
            {
            }

            public Room(HashSet<Coord> roomTiles, int[,] map)
            {
                tiles = roomTiles;
                roomSize = tiles.Count;
                connectedRooms = new List<Room>();

                edgeTiles = new List<Coord>();
                foreach (Coord tile in tiles)
                {
                    for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                    {
                        for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                        {
                            if (x == tile.tileX || y == tile.tileY)
                            {
                                if (map[x, y] == 1)
                                {
                                    edgeTiles.Add(tile);
                                }
                            }
                        }
                    }
                }
            }

            public void SetAccessibleFromMainRoom()
            {
                if (!isAccessibleFromMainRoom)
                {
                    isAccessibleFromMainRoom = true;
                    foreach (Room connectedRoom in connectedRooms)
                    {
                        connectedRoom.SetAccessibleFromMainRoom();
                    }
                }
            }

            public static void ConnectRooms(Room roomA, Room roomB)
            {
                if (roomA.isAccessibleFromMainRoom)
                {
                    roomB.SetAccessibleFromMainRoom();
                }
                else if (roomB.isAccessibleFromMainRoom)
                {
                    roomA.SetAccessibleFromMainRoom();
                }
                roomA.connectedRooms.Add(roomB);
                roomB.connectedRooms.Add(roomA);
            }

            public bool IsConnected(Room otherRoom)
            {
                return connectedRooms.Contains(otherRoom);
            }

            public int CompareTo(Room otherRoom)
            {
                return otherRoom.roomSize.CompareTo(roomSize);
            }
        }

    }
}