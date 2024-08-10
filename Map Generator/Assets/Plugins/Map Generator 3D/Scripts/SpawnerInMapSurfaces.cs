using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CNB
{
    /// <summary>
    /// Class in charge of creating new "game objects" in the edge of the generated walls acording to params.
        /// </summary>
    [Serializable]
    public class SpawnerInMapSurfaces : MonoBehaviour
    {
        int iterationStep;
        float _scale;
        List<GameObject> objecstoToSpawn = new List<GameObject>();
        static List<Vector3> objecstoToSpawnPos = new List<Vector3>();
        static List<Vector3> objecstoToSpawnNormal = new List<Vector3>();

        //Variables set in the "MapSO.cs scriptable object asset"
        [HideInInspector]
        public bool rotPrefabInitial;
        [HideInInspector]
        public bool rotPrefab90InVerticalExe;
        [HideInInspector]
        public bool randomizeScale;
        [HideInInspector]
        public int poolSize;
        [HideInInspector]
        public int spawnDensity = 10;
        [HideInInspector]
        public bool addCollider;
        [HideInInspector]
        public bool distanceManaged;
        [HideInInspector]
        public List<GameObject> GOList = new List<GameObject>();
        [HideInInspector]
        public float offsetNormalToWall;
        [HideInInspector]
        public float offsetVertical;
        [HideInInspector]
        public float GOScale;
        [HideInInspector]
        public bool onTopOfTheW;
        [HideInInspector]
        public bool normalToWall = true;

        GameObject holder;
        MeshGeneratorCNB mesh;
        MapGeneratorCNB _mapGen;
        
        public static bool allSpawnPosAndNormalCollected = false;

        public void Generate(GameObject _map, VertexPath[] _creators, bool _spawnColliders, bool exeYVertival)
        {
            objecstoToSpawn.Clear();
            if (SpawnerInMapSurfaces.allSpawnPosAndNormalCollected==false)
            {
                objecstoToSpawnPos.Clear();
                objecstoToSpawnNormal.Clear();
            }

            if (holder == null)
            {
                holder = GameObject.FindGameObjectWithTag("SpawnManager");
            }

            mesh = _map.GetComponent<MeshGeneratorCNB>();

            if (_spawnColliders)
            {
                SpawnColliders(_creators, exeYVertival);
            }
        }

        void ApplySpawnDensityAndColliderPrefsAndScale()
        {
            iterationStep = spawnDensity;
            _scale = GOScale;
            
            if (iterationStep==0)
            {
                Debug.LogWarning("spawn density must be set in MapSO, defaults to 5");
                iterationStep = 5;
            }
            if (_scale == 0)
            {
                Debug.LogWarning("scale must be set in MapSO, defaults to .5f");
                _scale = .5f;
            }
        }

        void SpawnColliders(VertexPath[] _creators, bool exeYVertical)
        {
            ApplySpawnDensityAndColliderPrefsAndScale();
            if (allSpawnPosAndNormalCollected==false)
            {
                AddAllPossibleSpawnPosAndNormal(_creators, exeYVertical);
                allSpawnPosAndNormalCollected = true;
            }
            SpawnSurfaceObjects(spawnDensity, exeYVertical);
        }

        private void AddAllPossibleSpawnPosAndNormal(VertexPath[] _creators, bool exeYVertical)
        {
            if (_creators != null && objecstoToSpawn != null && holder != null)
            {
                for (int i = 0; i < _creators.Length; i++)
                {
                    VertexPath path = _creators[i];
                    for (int j = 0; j < path.NumPoints; j++)
                    {
                        Vector3 localNormal = transform.TransformPoint(-path.GetNormal(j));

                        Vector3 normal = exeYVertical? new Vector3(localNormal.x, -localNormal.z,0).normalized: new Vector3(localNormal.x, localNormal.y, 0).normalized;
                        objecstoToSpawnPos.Add(path.GetPoint(j));
                        objecstoToSpawnNormal.Add(normal);
                    }
                }
            }
        }

        private void SpawnSurfaceObjects(int spawnDensity, bool exeYVertical)
        {
            if (_mapGen==null)
            {
                _mapGen= _mapGen = FindObjectOfType<MapGeneratorCNB>();
            }
            float offSet = 0;
            if (GOList.Count > 0)
            {
                int objsToPoolCalculation = (int)(objecstoToSpawnPos.Count / (iterationStep==0?1:iterationStep));

                if (PoolManCNB.instance!=null)
                {
                    PoolManCNB.instance.FillPoolDictionary();
                }

                foreach (var item in GOList)
                {
                    if (item!=null)
                    {
                        PoolManCNB.instance.CreatePool(item, (int)(objsToPoolCalculation / GOList.Count), distanceManaged);
                    }
                }
                
                for (int i = 0; i < objecstoToSpawnPos.Count; i+= (int)(iterationStep% objecstoToSpawnPos.Count))
                {
                    int random = Random.Range(0, GOList.Count);
                    GameObject obj = GOList[random];
                    Vector3 instantiatePos = exeYVertical ? new Vector3(objecstoToSpawnPos[i].x, 0, -objecstoToSpawnPos[i].y) : objecstoToSpawnPos[i];
                    Vector3 normal = exeYVertical ? new Vector3(objecstoToSpawnNormal[i].x, 0, -objecstoToSpawnNormal[i].y) : objecstoToSpawnNormal[i];
                    GameObject newGO = PoolManCNB.instance.ReuseObject(obj, instantiatePos - normal * offsetNormalToWall, obj.transform.localRotation);

                    if (newGO != null)
                    {
                        newGO.transform.localScale = Vector3.one * _scale;
                        AddCollider(newGO, addCollider);
                        
                        if (!onTopOfTheW)
                        {
                            offSet = -DistanceToFloor(newGO, exeYVertical);
                        }
                        
                        Vector3 newPosZ = new Vector3(newGO.transform.localPosition.x, newGO.transform.localPosition.y, offSet + offsetVertical);
                        Vector3 newPosY = new Vector3(newGO.transform.localPosition.x, offSet + offsetVertical, newGO.transform.localPosition.z);
                        newGO.transform.localPosition = exeYVertical? newPosY:newPosZ;
                        newGO.transform.rotation = Quaternion.Euler(rotPrefabInitial == true ? 90 : 0, 0, 0);
                        
                        if (exeYVertical)
                        {
                            newGO.transform.Rotate(Vector3.right * -90);
                        }

                        Randomize(newGO, normal, exeYVertical);
                        objecstoToSpawn.Add(newGO);
                        offSet = 0;
                    }
                }
            }
        }

        MeshCollider coll;
        LODGroup lod;
        void AddCollider(GameObject obj, bool add)
        {
            coll = obj.GetComponent<MeshCollider>();
            lod = obj.GetComponent<LODGroup>();

            if (_mapGen == null)
            {
                _mapGen = FindObjectOfType<MapGeneratorCNB>();
            }
            
            if (lod!=null)
            {
                Transform child = obj.transform.GetChild(0);
                ResetPosInChildren(obj);
            }

            if (add)
            {
                if (lod == null)
                {
                    if (coll == null)
                    {
                        coll = obj.AddComponent<MeshCollider>();
                        _mapGen._spawnedObjsCollMapSurfaces.Add(coll);
                        coll.enabled = false;
                    }
                    else
                    {
                        _mapGen._spawnedObjsCollMapSurfaces.Add(coll);
                        coll.enabled = false;
                    }
                }
                else
                {
                    Transform child = obj.transform.GetChild(0);
                    if (child != null)
                    {
                        coll = child.GetComponent<MeshCollider>();
                        if (coll == null)
                        {
                            coll = child.gameObject.AddComponent<MeshCollider>();
                            _mapGen._spawnedObjsCollMapSurfaces.Add(coll);
                            coll.enabled = false;
                        }
                        else
                        {
                            _mapGen._spawnedObjsCollMapSurfaces.Add(coll);
                            coll.enabled = false;
                        }
                    }
                }
                int collLayer = LayerMask.NameToLayer("Collisions");
                obj.gameObject.layer = collLayer;
            }
        }
        
        void ResetPosInChildren(GameObject objToSpawn)//todo
        {
            int childCount = objToSpawn.transform.childCount;

            if (childCount > 0)
            {
                for (int j = 0; j < childCount; j++)
                {
                    Transform child = objToSpawn.transform.GetChild(j);
                    child.localPosition = Vector3.zero;
                    child.localRotation = Quaternion.identity;
                }
            }
        }

        void Randomize(GameObject objToRandomize, Vector3 normal, bool exeYVertical)
        {
            float randomScaleFactor = UnityEngine.Random.Range(.5f, 1);
            int randomRotAngle = UnityEngine.Random.Range(0, 360);
            Vector3 randomScale = objToRandomize.transform.localScale * randomScaleFactor;
            Vector3 lDirection = new Vector3(Mathf.Sin(Mathf.Deg2Rad * randomRotAngle), Mathf.Cos(Mathf.Deg2Rad * randomRotAngle),0);
            
            if (normalToWall)
            {
                if (rotPrefabInitial)
                {
                    if (exeYVertical)
                    {
                        objToRandomize.transform.Rotate(new Vector3(0, Vector3.SignedAngle(Vector3.forward, normal, Vector3.up), 0));
                    }
                    else
                    {
                        objToRandomize.transform.Rotate(new Vector3(0, Vector3.SignedAngle(Vector3.up, normal, Vector3.forward), 0));
                    }
                }
                else
                {
                    if (exeYVertical)
                    {
                        objToRandomize.transform.Rotate(new Vector3(0, 0, Vector3.SignedAngle(Vector3.forward, normal, Vector3.up)));
                    }
                    else
                    {
                        objToRandomize.transform.Rotate(new Vector3(0, 0, Vector3.SignedAngle(Vector3.up, normal, Vector3.forward)));
                    }
                }
            }
            else
            {
                if (rotPrefabInitial)
                {
                    objToRandomize.transform.Rotate(new Vector3(0, randomRotAngle, 0));
                }
                else
                {
                    objToRandomize.transform.Rotate(new Vector3(0, 0, randomRotAngle));
                }
            }

            if (rotPrefab90InVerticalExe)
            {
                if (rotPrefabInitial)
                {
                    objToRandomize.transform.Rotate(new Vector3(0, 90, 0));
                }
                else
                {
                    objToRandomize.transform.Rotate(new Vector3(0, 0, 90));
                }
            }
            if (randomizeScale)
            {
                objToRandomize.transform.localScale = randomScale;
            }
        }

        float DistanceToFloor(GameObject go, bool exeYVertical)
        {
            float dist = 0;
            RaycastHit hitInfo;
            Vector3 rayDir = exeYVertical?Vector3.down : Vector3.back;
            float offsetcast = .5f;
            Ray ray = new Ray(go.transform.position+rayDir*offsetcast, rayDir);
            Physics.Raycast(ray, out hitInfo, Mathf.Infinity);
            dist = hitInfo.distance;
            return dist + offsetcast;
        }
    }
}