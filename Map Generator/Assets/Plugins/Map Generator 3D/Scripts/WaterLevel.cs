using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// exposes a param to set the water height and a bool to avoid spawning objects in the water or not. 
/// </summary>
namespace CNB
{
    public class WaterLevel : MonoBehaviour
    {
        public bool isWater = true;
        [HideInInspector]
        public bool autoUpdate = true;
        public float waterLevelHeight = 0;
        [HideInInspector]
        public WallLimits wallLimitsUp;
        [HideInInspector]
        public WallLimits wallLimitsDown;
        public GameObject waterLevelPrefab;


        MapGeneratorCNB _mapGeneratorCNB;

        public void SetWaterLevel()
        {
                if (isWater)
                {
                    this.tag = "Water";
                }
                
                _mapGeneratorCNB = this.gameObject.GetComponentInParent<MapGeneratorCNB>();
            if (_mapGeneratorCNB != null)
            {
                Mesh waterPlaneMesh = GetComponent<MeshFilter>().sharedMesh;
                MeshRenderer waterRend = GetComponent<MeshRenderer>();
                if (waterLevelPrefab!=null)
                {
                    waterRend.enabled = false;
                }
                
                int mapWidth = _mapGeneratorCNB._map._width + _mapGeneratorCNB._borderMap * 2;
                int mapHeight = _mapGeneratorCNB._map._height + _mapGeneratorCNB._borderMap * 2;
                Vector3 newWaterScale = new Vector3((mapWidth-1) / waterPlaneMesh.bounds.size.x, (mapHeight-1) / waterPlaneMesh.bounds.size.y, -1);
                transform.localScale = newWaterScale;
                Vector3 newWaterPos = new Vector3(0, 0, waterLevelHeight);
                transform.localPosition = newWaterPos;
                int collLayer = LayerMask.NameToLayer("Collisions");
                gameObject.layer = collLayer;
                MeshCollider col = GetComponent<MeshCollider>();
                if (col)
                {
                    col.sharedMesh = waterPlaneMesh;
                }
            }

            wallLimitsUp.DrawMesh(true,wallLimitsUp.invertNormals, _mapGeneratorCNB._mapWallLimitsUpHeight);
            wallLimitsDown.DrawMesh(false, wallLimitsDown.invertNormals, _mapGeneratorCNB._mapWallLimitsDownHeight);
        }
    }
}
