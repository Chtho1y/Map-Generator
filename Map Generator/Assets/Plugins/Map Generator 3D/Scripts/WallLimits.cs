using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// creates a cage that include the map to hide in the lower part things that should not be visible and 
/// in the upper part allouw to choose between the use of a skybox or a map confitated with walls and top to apply materials 
/// </summary>
namespace CNB
{
	public class WallLimits : MonoBehaviour
	{
		[HideInInspector] 
		public bool generateWallsLimit = true;

		MapGeneratorCNB _mapGeneratorCNB;

		[HideInInspector]
		public GameObject noiseFloor;
		[HideInInspector]
		public MeshFilter _meshFilter;

		Mesh _limitWallsMesh;

		[HideInInspector]
		public float _limitWallPerimeter;
		[HideInInspector]
		public bool invertNormals = false;

		public void DrawMesh(bool up, bool invertNormals, int limitWallHeight)
		{
            if (!up)
            {
				limitWallHeight = -Mathf.Abs(limitWallHeight);

			}
			if (generateWallsLimit)
			{
				_meshFilter = GetComponent<MeshFilter>();
				_mapGeneratorCNB = GameObject.FindGameObjectWithTag("MapSO").GetComponent<MapGeneratorCNB>();

				List<Vector3> waterVertices;

				waterVertices = GameObject.FindGameObjectWithTag("Water").GetComponent<MeshFilter>().sharedMesh.vertices.ToList<Vector3>();
				Vector3 pos3 = waterVertices[2];
				waterVertices[2] = waterVertices[3];
				waterVertices[3] = pos3;
				waterVertices.Add(waterVertices[0]);

				List<Vector3> wallVertices = new List<Vector3>();
				List<int> wallTriangles = new List<int>();
				Vector3 offSet = Vector3.forward * limitWallHeight;

				for (int i = 0; i < waterVertices.Count-1; i++)
				{
					int startIndex = wallVertices.Count;
                    
					wallVertices.Add(waterVertices[i + 1]); // right
					wallVertices.Add(waterVertices[i]); // left
					wallVertices.Add(waterVertices[i + 1] + offSet); // top right
					wallVertices.Add(waterVertices[i] + offSet); // top left
                    
					if (!invertNormals)
                    {
						wallTriangles.Add(startIndex + 0);
						wallTriangles.Add(startIndex + 3);
						wallTriangles.Add(startIndex + 2);

						wallTriangles.Add(startIndex + 3);
						wallTriangles.Add(startIndex + 0);
						wallTriangles.Add(startIndex + 1);
                    }
                    else
                    {
						wallTriangles.Add(startIndex + 0);
						wallTriangles.Add(startIndex + 2);
						wallTriangles.Add(startIndex + 3);

						wallTriangles.Add(startIndex + 3);
						wallTriangles.Add(startIndex + 1);
						wallTriangles.Add(startIndex + 0);
					}
				}

				_limitWallsMesh = new Mesh();
				_limitWallsMesh.vertices = wallVertices.ToArray();
				_limitWallsMesh.triangles = wallTriangles.ToArray();
				_limitWallsMesh.RecalculateNormals();

				Vector2[] uvs = new Vector2[wallVertices.Count];

				float[] distanceToPreviousVertArr = new float[wallVertices.Count];
				float totalDistance = 0.0f;
				for (int i = 0; i < wallVertices.Count; i += 4)
				{
					Vector3 newPos = new Vector3(wallVertices[i].x * transform.parent.localScale.y, wallVertices[i].y * transform.parent.localScale.x, wallVertices[i].z);
					Vector3 newPos1 = new Vector3(wallVertices[i+1].x * transform.parent.localScale.y, wallVertices[i+1].y * transform.parent.localScale.x, wallVertices[i+1].z);
					float distanceToPreviousVert = Vector3.Distance(newPos1, newPos);
					distanceToPreviousVertArr[i % (wallVertices.Count)] = distanceToPreviousVert;
					totalDistance += distanceToPreviousVert;
				}
				_limitWallPerimeter = totalDistance;
				float distanceAccumulated = 0.0f;
				float distAcumPrev = 0.0f;
				for (int i = 0; i <= wallVertices.Count; i += 4)
				{
					float percentXorYPrev = Mathf.InverseLerp(0, totalDistance, distAcumPrev);
					float percentXorY = Mathf.InverseLerp(0, totalDistance, distanceAccumulated);

					uvs[(i) % wallVertices.Count] = new Vector2(percentXorYPrev, 0);
					uvs[(i + 1) % wallVertices.Count] = new Vector2(percentXorY, 0);
					uvs[(i + 2) % wallVertices.Count] = new Vector2(percentXorYPrev, limitWallHeight);
					uvs[(i + 3) % wallVertices.Count] = new Vector2(percentXorY, limitWallHeight);

					distAcumPrev = distanceAccumulated;
					distanceAccumulated += distanceToPreviousVertArr[i % (wallVertices.Count)];
				}

				_limitWallsMesh.uv = uvs;

				_meshFilter.sharedMesh = _limitWallsMesh;

				Vector3 newLimitWallsScaleUp = new Vector3(1,1,-1);
				Vector3 newLimitWallsScaleDown = Vector3.one;

				transform.localScale = up ? newLimitWallsScaleUp : newLimitWallsScaleDown;
				WaterLevel wl = transform.parent.GetComponent<WaterLevel>();
				Vector3 newLimitWallsPosUp = new Vector3(0, 0, 0);
				Vector3 newLimitWallsPosDown = new Vector3(0, 0, -limitWallHeight + noiseFloor.transform.localPosition.z + wl.waterLevelHeight);
				transform.localPosition = up? newLimitWallsPosUp: newLimitWallsPosDown;

				int collLayer = LayerMask.NameToLayer("Collisions");
				gameObject.layer = collLayer;

				MeshCollider col = GetComponent<MeshCollider>();

				if (col != null)
				{
					col.sharedMesh = _limitWallsMesh;
				}
			}
		}
	}
}
