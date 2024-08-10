using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CNB
{
    /// <summary>
    /// Class in charge of creating new "game objects" acording to params.
    /// </summary>
    [Serializable]
    public class SpawnerInMapCells : MonoBehaviour
    {
        //Variables set in the "MapSO.cs scriptable object asset"
        [HideInInspector]
        public int _minRadius = 1;
        
        [HideInInspector]
        public bool freeCells;
        [HideInInspector]
        public int iterationStep=10;
        [HideInInspector]
        public bool rotPrefab90;
        [HideInInspector]
        public bool addCollider;
        [HideInInspector]
        public float GOscale;
        [HideInInspector]
        public float offSetVertical;
        [HideInInspector]
        public bool normalToFloor;
        [HideInInspector]
        public bool distanceManaged;
        [HideInInspector]
        public List<GameObject> _GOs;
        //.......


        [HideInInspector]
        public Vector2 regionSize;
        int rejectionSamples = 10;
        MapGeneratorCNB _mapGen;
        [HideInInspector]
        public List<Vector2> points;
        public static bool maxPointsInitialized = false;
        List<GameObject> _objToSpawnList=new List<GameObject>();
        GameObject _holder;
       
        public void Init(int squaresize)
        {
            _minRadius = _minRadius * squaresize;
            _mapGen = FindObjectOfType<MapGeneratorCNB>();
            _holder = GameObject.FindGameObjectWithTag("SpawnManager");
            if (maxPointsInitialized == false)
            {
                if (_mapGen)
                {
                    ActualizeSize();
                    points = GeneratePoints(_minRadius, regionSize, rejectionSamples);
                    maxPointsInitialized = true;
                }
            }
        }

        public void Spawn(bool freeCells, bool exeYVertival)
        {
            _mapGen._freeCellSpawnedGOGlobalCount = 0;
            _objToSpawnList.Clear();
            List<Vector2> puntos = freeCells ? _mapGen.spawnablePointsFloor : _mapGen.spawnablePointsTop;
            int objsToPoolCalculation = (int)(puntos.Count/(iterationStep==0?1:iterationStep));
            bool hittedWater = false;
            Vector3 hitNormal = new Vector3();
            Vector3 newPosZ;
            Vector3 newPosY;
            Vector3 rayCastPos;

            PoolManCNB.instance.FillPoolDictionary();

            foreach (var item in _GOs)
            {
                if (item!=null)
                {
                     PoolManCNB.instance.CreatePool(item, (int)(objsToPoolCalculation / _GOs.Count), distanceManaged);
                }
            }
            Vector3 posExeYVertical;
            Vector3 posExeZVertical;
            if (puntos != null && _GOs.Count > 0)
            {
                for (int i = 0; i < puntos.Count; i+=(int)((iterationStep)%puntos.Count))
                {
                    posExeZVertical= new Vector3(puntos[i].x, puntos[i].y, 0);
                    posExeYVertical = new Vector3(puntos[i].x, 0, -puntos[i].y);
                    int index = UnityEngine.Random.Range(0, _GOs.Count);
                    rayCastPos = exeYVertival?posExeYVertical : posExeZVertical;
                    float offSet = -DistanceToFloor(rayCastPos, freeCells, ref hittedWater, ref hitNormal, exeYVertival);
                    if (_GOs[index] != null && !hittedWater)
                    {
                        GameObject newGO = PoolManCNB.instance.ReuseObject(_GOs[index], transform.localPosition, Quaternion.identity);

                        if (newGO != null)
                        {
                            _objToSpawnList.Add(newGO);
                            _mapGen._freeCellSpawnedGOGlobalCount++;
                            newGO.transform.rotation = Quaternion.Euler(rotPrefab90 == true ? 90 : 0, 0, 0);

                            if (exeYVertival)
                            {
                                newGO.transform.Rotate(Vector3.right * -90);
                            }

                            AddCollider(newGO, addCollider);

                            newGO.transform.localPosition = rayCastPos;
                            newPosZ = new Vector3(newGO.transform.localPosition.x, newGO.transform.localPosition.y, offSet +offSetVertical);
                            newPosY = new Vector3(newGO.transform.localPosition.x, offSet + offSetVertical , newGO.transform.localPosition.z);

                            if (normalToFloor)
                            {
                                if (rotPrefab90)
                                {
                                    newGO.transform.rotation = Quaternion.LookRotation(hitNormal);
                                    newGO.transform.Rotate(new Vector3(90, 0, 0));
                                }
                                else
                                {
                                    newGO.transform.rotation = Quaternion.LookRotation(hitNormal);
                                }
                            }

                            newGO.transform.localScale = Vector3.one * (GOscale == 0 ? 1f : GOscale);
                            Randomize(newGO);
                            newGO.transform.localPosition = exeYVertival ? newPosY : newPosZ;
                        }
                    }
                    hittedWater = false;
                }
            }
        }

        void AddCollider(GameObject obj, bool add)
        {
            MeshCollider coll = obj.GetComponent<MeshCollider>();
            MeshCollider[] childrenColl = obj.GetComponentsInChildren<MeshCollider>();

            int childCount = obj.transform.childCount;

            if (coll != null)
            {
                DestroyImmediate(coll);
            }

            if (childrenColl != null)
            {
                foreach (var item in childrenColl)
                {
                    DestroyImmediate(item);
                }
            }

            LODGroup lod = obj.GetComponent<LODGroup>();

            if (lod != null)
            {
                if (childCount>0)
                {
                    ResetPosInChildren(obj);
                }
            }

            if (add)
            {
                if (lod == null)
                {
                     coll = obj.AddComponent<MeshCollider>();
                     _mapGen._spawnedObjsCollMapCells.Add(coll);
                     coll.convex = true;
                     coll.enabled = false;
                }
                else
                {
                    if (childCount>0)
                    {
                        Transform child = obj.transform.GetChild(0);
                        if (child != null)
                        {
                            coll = child.gameObject.AddComponent<MeshCollider>();
                            coll.convex = true;
                            coll.enabled = false;
                            _mapGen._spawnedObjsCollMapCells.Add(coll);
                        }
                    }
                }
                int collLayer = LayerMask.NameToLayer("Collisions");
                obj.gameObject.layer = collLayer;
            }
        }

        void ActualizeSize()
        {
            regionSize = new Vector2(_mapGen._map._width-_mapGen._borderMap, _mapGen._map._height-_mapGen._borderMap) * _mapGen._map._squareSize;
        }

        List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30)
        {
            if (radius==0)
            {
                radius = 5;
            }

            float cellSize = radius / Mathf.Sqrt(2);

            int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
            List<Vector2> points = new List<Vector2>();
            List<Vector2> spawnPoints = new List<Vector2>();

            spawnPoints.Add(sampleRegionSize / 2);
            while (spawnPoints.Count > 0)
            {
                int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
                Vector2 spawnCentre = spawnPoints[spawnIndex];
                bool candidateAccepted = false;

                for (int i = 0; i < numSamplesBeforeRejection; i++)
                {
                    float angle = UnityEngine.Random.value * Mathf.PI * 2;
                    Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                    Vector2 candidate = spawnCentre + dir * UnityEngine.Random.Range(radius, 2 * radius);
                    if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid))
                    {
                        points.Add(candidate);
                        spawnPoints.Add(candidate);
                        grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                        candidateAccepted = true;
                        break;
                    }
                }
                if (!candidateAccepted)
                {
                    spawnPoints.RemoveAt(spawnIndex);
                }
            }
            return points;
        }

        bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
        {
            if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
            {
                int cellX = (int)(candidate.x / cellSize);
                int cellY = (int)(candidate.y / cellSize);
                int searchStartX = Mathf.Max(0, cellX - 2);
                int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
                int searchStartY = Mathf.Max(0, cellY - 2);
                int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

                for (int x = searchStartX; x <= searchEndX; x++)
                {
                    for (int y = searchStartY; y <= searchEndY; y++)
                    {
                        int pointIndex = grid[x, y] - 1;
                        if (pointIndex != -1)
                        {
                            float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
                            if (sqrDst < radius * radius)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }
        
        void ResetPosInChildren(GameObject GO) //todo
        {
            
            int childCount = GO.transform.childCount;

            if (childCount > 0)//helper to work with prefabs with children
            {
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = GO.transform.GetChild(i);
                    child.localPosition = Vector3.zero;
                    child.localRotation = Quaternion.identity;
                }
            }
        }

        void Randomize(GameObject objToRandomize)
        {
            float randomScaleFactor = UnityEngine.Random.Range(.5f, 1);
            Vector3 randomScale = objToRandomize.transform.localScale * randomScaleFactor;

            int randomRotAngle = UnityEngine.Random.Range(0, 360);
            if (rotPrefab90==true)
            {
                objToRandomize.transform.Rotate(new Vector3(0, randomRotAngle, 0));
            }
            else
            {
                objToRandomize.transform.Rotate(new Vector3(0, 0, randomRotAngle));
            }

            objToRandomize.transform.localScale = randomScale;
        }

        float DistanceToFloor(Vector3 pos, bool freeCellsBool, ref bool hitWater, ref Vector3 hitNormal, bool exeYVertival)
        {
            float dist = 0;
            int offsetCast = 100;
            (bool, RaycastHit) result = DistanceToFirstcontact(pos, offsetCast, exeYVertival);
            hitNormal = result.Item2.normal;
            
            if (result.Item1)
            {
                if (freeCellsBool)
                {
                    if (result.Item2.collider.tag == "Water")
                    {
                        hitWater = true;
                    }
                }
                dist = result.Item2.distance;
                return (dist - offsetCast);
            }
            return 0;
        }

        (bool,RaycastHit) DistanceToFirstcontact(Vector3 pos, int offSetTop, bool exeYVertical)
        {
            bool hit = false;
            RaycastHit hitInfoTop;
            Vector3 rayDir1 = exeYVertical ? Vector3.up : Vector3.forward;
            Vector3 newPos = pos + rayDir1 * offSetTop;
            Vector3 rayDir2 = exeYVertical ? Vector3.down : Vector3.back;
            Ray ray = new Ray(newPos, rayDir2);
            hit = Physics.Raycast(ray, out hitInfoTop, Mathf.Infinity);
            return (hit, hitInfoTop);
        }
    }
}