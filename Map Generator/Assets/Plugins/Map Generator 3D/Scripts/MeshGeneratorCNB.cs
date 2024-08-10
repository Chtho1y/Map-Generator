using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace CNB
{
    public class MeshGeneratorCNB : MonoBehaviour
    {
        /// <summary>
        /// Generates a new mesh when demanded from "MapGeneratorCNB.cs". Also creates one LineRenderer components for every outline of the map, and sets their points base on the Bezier curves and vertex data calculetes in respective clases.
        /// </summary>

        List<BezierPath> paths = new List<BezierPath>();
        List<VertexPath> vertexPaths = new List<VertexPath>();
        List<EdgeCollider2D> edgeColliders = new List<EdgeCollider2D>();
        [HideInInspector]
        public List<Material> _lineRendMat;
        [HideInInspector]
        public MapGeneratorCNB _mapGen;
        Mesh mesh;
        MeshCollider meshColl;
        //............................................

        [HideInInspector]
        public SquareGrid squareGrid;
        [HideInInspector]
        public MeshFilter cave;
        [HideInInspector]
        public List<Vector3> vertices = new List<Vector3>();
        [HideInInspector]
        public List<int> triangles = new List<int>();
        Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
        [HideInInspector]
        public List<List<int>> outlines = new List<List<int>>();
        HashSet<int> checkedVertices = new HashSet<int>();

        // spawn
        List<Vector2[]> allPoints = new List<Vector2[]>();
        Vector3[] allLocalNormals;
        
        [HideInInspector]
        public List<SpawnerInMapSurfaces> _spawnerColliders;
        
        public bool _spawnInCollidersBool = false;

        int indexTempAmount = 0;
        int squareCount = 0;

        public List<List<int>> outlinesBU;

        [Range(0f, 1f)]
        [HideInInspector]
        public float lineRendererVertexSpacing = .1f;

        [HideInInspector]
        public int _colliderSpawnersCount = 0;
        [HideInInspector]
        public int _colliderSpawnedGOGlobalCount = 0;

        MeshFilter _wallsMeshFilter;
        [HideInInspector]
        public bool _generateWalls;
        Vector3 _wallTransformApplied;
        [HideInInspector]
        public float _limitWallPerimeter;

        public void CleanMap()
        {
            edgeColliders = new List<EdgeCollider2D>(GetComponents<EdgeCollider2D>() != null ? GetComponents<EdgeCollider2D>() : null);
            foreach (var edgeCol in edgeColliders)
            {
                DestroyImmediate(edgeCol, true);
            }
            edgeColliders.Clear();
        }

        public void GenerateMesh(int[,] map, float squareSize)
        {
            indexTempAmount = 0;
            squareCount = 0;

            triangleDictionary.Clear();
            outlines.Clear();
            checkedVertices.Clear();

            squareGrid = new SquareGrid(map, squareSize);

            vertices.Clear();
            triangles.Clear();

            AssignVertexForMesh();

            mesh = new Mesh();
            cave.mesh = mesh;

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            Vector2[] uvs = new Vector2[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                float percentX = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, vertices[i].x);
                float percentY = Mathf.InverseLerp(-map.GetLength(1) / 2 * squareSize, map.GetLength(1) / 2 * squareSize, vertices[i].y);
                uvs[i] = new Vector2(percentX, percentY);
            }
            mesh.uv = uvs;
            meshColl = _mapGen._mapMesh.GetComponent<MeshCollider>();
            if(meshColl != null)
            {
                meshColl.sharedMesh = mesh;
            }

            if (_generateWalls)
            {
                CreateWallMesh(map, squareSize);
            }
            
            Generate2DColliders(vertices);//todo no es necesario.eliminar codigo
        }

        private void AssignVertexForMesh()
        {
            for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
                {
                    TriangulateSquare(squareGrid.squares[x, y], x, y);
                }
            }
        }

        public void Generate2DColliders(List<Vector3> vertices)
        {
            //mio...................................................................aa
            CleanMap();
            vertexPaths.Clear();
            paths.Clear();
            CalculateMeshOutlines(vertices);

            //mio.................................................................................
            foreach (List<int> outline in outlines)
            {
                EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
                edgeCollider.edgeRadius = .1f;
                edgeColliders.Add(edgeCollider);
                Vector2[] edgePoints = new Vector2[outline.Count];

                for (int i = 0; i < outline.Count; i++)
                {
                    edgePoints[i] = new Vector2(vertices[outline[i]].x, vertices[outline[i]].y);
                }

                allPoints.Add(edgePoints);
                edgeCollider.points = edgePoints;
                BezierPath path = new BezierPath(edgePoints);
                paths.Add(path);
                VertexPath vertexPath = new VertexPath(path, transform, lineRendererVertexSpacing);
                vertexPaths.Add(vertexPath);

                allLocalNormals = vertexPath.localNormals;
                //.......................................................................
            }
            SpawnerInMapSurfaces.allSpawnPosAndNormalCollected = false;
            SpawnInColliders(_spawnInCollidersBool);
        }
        //
        public void SpawnInColliders(bool genColl)
        {
            _colliderSpawnersCount = 0;
            _colliderSpawnedGOGlobalCount = 0;
            _mapGen._spawnedObjsCollMapSurfaces.Clear();

            if (genColl)
            {
                if (_spawnerColliders != null && _spawnerColliders.Count > 0)
                {
                    foreach (var p in _spawnerColliders)
                    {
                        if (p != null && vertexPaths != null && vertexPaths.Count > 0)
                        {
                            _colliderSpawnersCount++;
                            p.Generate(gameObject, vertexPaths.ToArray(), genColl, _mapGen._exeYVertival);
                        }
                    }
                }
            }
        }
        
        void TriangulateSquare(Square square, int indexX, int indexY)
        {

            bool firstSqareInRow = indexY == 0;
            bool lastSquareInRow = indexY == squareGrid.squares.GetLength(1) - 1;

            Square squareAtPrecedentIndex = lastSquareInRow || firstSqareInRow ? squareGrid.squares[indexX, indexY] : squareGrid.squares[indexX, indexY - 1]; ;
            Square firstSquareToReduceMeshRes = !firstSqareInRow ? squareGrid.squares[indexX, indexY - indexTempAmount] : squareGrid.squares[indexX, indexY];

            if (squareAtPrecedentIndex.configuration == 15 && square.configuration != 15 || lastSquareInRow)
            {
                MeshFromPoints(squareAtPrecedentIndex.topLeft, squareAtPrecedentIndex.topRight, firstSquareToReduceMeshRes.bottomRight, firstSquareToReduceMeshRes.bottomLeft);
                checkedVertices.Add(squareAtPrecedentIndex.topLeft.vertexIndex);
                checkedVertices.Add(squareAtPrecedentIndex.topRight.vertexIndex);
                checkedVertices.Add(firstSquareToReduceMeshRes.bottomRight.vertexIndex);
                checkedVertices.Add(firstSquareToReduceMeshRes.bottomLeft.vertexIndex);
                indexTempAmount = lastSquareInRow ? -1 : 0;
            }

            switch (square.configuration)
            {
                case 0:
                    break;

                // 1 points:
                case 1:
                    MeshFromPoints(square.centreLeft, square.centreBottom, square.bottomLeft);
                    break;
                case 2:
                    MeshFromPoints(square.bottomRight, square.centreBottom, square.centreRight);
                    break;
                case 4:
                    MeshFromPoints(square.topRight, square.centreRight, square.centreTop);
                    break;
                case 8:
                    MeshFromPoints(square.topLeft, square.centreTop, square.centreLeft);
                    break;

                // 2 points:
                case 3:
                    MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                    break;
                case 6:
                    MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
                    break;
                case 9:
                    MeshFromPoints(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
                    checkedVertices.Add(square.topLeft.vertexIndex);
                    checkedVertices.Add(square.bottomLeft.vertexIndex);
                    break;
                case 12:
                    MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreLeft);
                    break;
                case 5:
                    MeshFromPoints(square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
                    break;
                case 10:
                    MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
                    break;

                // 3 point:
                case 7:
                    MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                    break;
                case 11:
                    MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
                    break;
                case 13:
                    MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
                    break;
                case 14:
                    MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
                    break;

                // 4 point:
                case 15:
                    //MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                    checkedVertices.Add(square.topLeft.vertexIndex);
                    checkedVertices.Add(square.topRight.vertexIndex);
                    checkedVertices.Add(square.bottomRight.vertexIndex);
                    checkedVertices.Add(square.bottomLeft.vertexIndex);
                    indexTempAmount++;
                    break;
            }
            squareCount++;
        }

        void MeshFromPoints(params Node[] points)
        {
            AssignVertices(points);

            if (points.Length >= 3)
                CreateTriangle(points[0], points[1], points[2]);
            if (points.Length >= 4)
                CreateTriangle(points[0], points[2], points[3]);
            if (points.Length >= 5)
                CreateTriangle(points[0], points[3], points[4]);
            if (points.Length >= 6)
                CreateTriangle(points[0], points[4], points[5]);

        }

        void AssignVertices(Node[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].vertexIndex == -1)
                {
                    points[i].vertexIndex = vertices.Count;
                    vertices.Add(points[i].position);
                }
            }
        }

        void CreateTriangle(Node a, Node b, Node c)
        {
            triangles.Add(a.vertexIndex);
            triangles.Add(b.vertexIndex);
            triangles.Add(c.vertexIndex);

            Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
            AddTriangleToDictionary(triangle.vertexIndexA, triangle);
            AddTriangleToDictionary(triangle.vertexIndexB, triangle);
            AddTriangleToDictionary(triangle.vertexIndexC, triangle);
        }

        void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
        {
            if (triangleDictionary.ContainsKey(vertexIndexKey))
            {
                triangleDictionary[vertexIndexKey].Add(triangle);
            }
            else
            {
                List<Triangle> triangleList = new List<Triangle>();
                triangleList.Add(triangle);
                triangleDictionary.Add(vertexIndexKey, triangleList);
            }
        }

        void CalculateMeshOutlines(List<Vector3> vertices)
        {

            for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
            {
                if (!checkedVertices.Contains(vertexIndex))
                {
                    int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                    if (newOutlineVertex != -1)
                    {
                        checkedVertices.Add(vertexIndex);

                        List<int> newOutline = new List<int>();
                        newOutline.Add(vertexIndex);
                        outlines.Add(newOutline);
                        FollowOutline(newOutlineVertex, outlines.Count - 1);
                        outlines[outlines.Count - 1].Add(vertexIndex);
                    }
                }
            }
        }

        void FollowOutline(int vertexIndex, int outlineIndex)
        {
            outlines[outlineIndex].Add(vertexIndex);
            checkedVertices.Add(vertexIndex);
            int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

            if (nextVertexIndex != -1)
            {
                FollowOutline(nextVertexIndex, outlineIndex);
            }
        }

        int GetConnectedOutlineVertex(int vertexIndex)
        {
            List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

            for (int i = 0; i < trianglesContainingVertex.Count; i++)
            {
                Triangle triangle = trianglesContainingVertex[i];

                for (int j = 0; j < 3; j++)
                {
                    int vertexB = triangle[j];
                    if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB))
                    {
                        if (IsOutlineEdge(vertexIndex, vertexB))
                        {
                            return vertexB;
                        }
                    }
                }
            }

            return -1;
        }

        bool IsOutlineEdge(int vertexA, int vertexB)
        {
            List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];
            int sharedTriangleCount = 0;

            for (int i = 0; i < trianglesContainingVertexA.Count; i++)
            {
                if (trianglesContainingVertexA[i].Contains(vertexB))
                {
                    sharedTriangleCount++;
                    if (sharedTriangleCount > 1)
                    {
                        break;
                    }
                }
            }
            return sharedTriangleCount == 1;
        }
        //.........................................UPD
        void CreateWallMesh(int[,] map, float squareSize)
        {
            _wallTransformApplied = Vector3.forward * _mapGen._map._wallsHeight;

            CalculateMeshOutlines(vertices);

            List<Vector3> wallVertices = new List<Vector3>();
            List<int> wallTriangles = new List<int>();
            Mesh wallMesh = new Mesh();

            foreach (List<int> outline in outlines)
            {
                for (int i = 0; i < outline.Count - 1; i++)
                {
                    int startIndex = wallVertices.Count;
                    wallVertices.Add(vertices[outline[i + 1]]); // right
                    wallVertices.Add(vertices[outline[i]]); // left
                    wallVertices.Add(vertices[outline[i + 1]] + _wallTransformApplied); // bottom right
                    wallVertices.Add(vertices[outline[i]] + _wallTransformApplied); // bottom left

                    wallTriangles.Add(startIndex + 0);
                    wallTriangles.Add(startIndex + 2);
                    wallTriangles.Add(startIndex + 3);

                    wallTriangles.Add(startIndex + 3);
                    wallTriangles.Add(startIndex + 1);
                    wallTriangles.Add(startIndex + 0);
                }
            }
            wallMesh.vertices = wallVertices.ToArray();
            wallMesh.triangles = wallTriangles.ToArray();
            wallMesh.RecalculateNormals();
            Vector2[] uvs = new Vector2[wallVertices.Count];

            float[] distanceToPreviousVertArr = new float[wallVertices.Count];
            float totalDistance = 0.0f;
            for (int i = 0; i < wallVertices.Count; i+=4)
            {
                float distanceToPreviousVert = Vector3.Distance(wallVertices[i+1], wallVertices[i]);
                distanceToPreviousVertArr[i% (wallVertices.Count)] = distanceToPreviousVert;
                totalDistance += distanceToPreviousVert;
            }
            _limitWallPerimeter = totalDistance;
            float distanceAccumulated = 0.0f;
            float distAcumPrev = 0.0f;
            for (int i = 0; i <= wallVertices.Count && wallVertices.Count>0; i+=4)
            {
                float percentXorYPrev = Mathf.InverseLerp(0, totalDistance, distAcumPrev);
                float percentXorY = Mathf.InverseLerp(0, totalDistance, distanceAccumulated);

                uvs[(i ) % wallVertices.Count] = new Vector2(percentXorY, 0);
                uvs[(i+ 1) % wallVertices.Count] = new Vector2(percentXorYPrev, 0);
                uvs[(i+ 2) % wallVertices.Count] = new Vector2(percentXorY, _mapGen._map._wallsHeight);
                uvs[(i+ 3) % wallVertices.Count] = new Vector2(percentXorYPrev, _mapGen._map._wallsHeight);

                distAcumPrev = distanceAccumulated;
                distanceAccumulated += distanceToPreviousVertArr[i % (wallVertices.Count)];
            }

            wallMesh.uv = uvs;
            GameObject wall = GameObject.FindGameObjectWithTag("Walls");
            MeshCollider wallMeshColl = wall.GetComponent<MeshCollider>();
            if (wall!=null)
            {
                _wallsMeshFilter = wall.GetComponent<MeshFilter>();
                _wallsMeshFilter.sharedMesh = wallMesh;
                if (wallMeshColl!=null)
                {
                    wallMeshColl.sharedMesh = wallMesh;
                }
            }
        }
        
        //........................................................



        struct Triangle
        {
            public int vertexIndexA;
            public int vertexIndexB;
            public int vertexIndexC;
            int[] vertices;

            public Triangle(int a, int b, int c)
            {
                vertexIndexA = a;
                vertexIndexB = b;
                vertexIndexC = c;

                vertices = new int[3];
                vertices[0] = a;
                vertices[1] = b;
                vertices[2] = c;
            }

            public int this[int i]
            {
                get
                {
                    return vertices[i];
                }
            }


            public bool Contains(int vertexIndex)
            {
                return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
            }
        }

        public class SquareGrid
        {
            public Square[,] squares;

            public SquareGrid(int[,] map, float squareSize)
            {
                int nodeCountX = map.GetLength(0);
                int nodeCountY = map.GetLength(1);
                float mapWidth = nodeCountX * squareSize;
                float mapHeight = nodeCountY * squareSize;

                ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

                for (int x = 0; x < nodeCountX; x++)
                {
                    for (int y = 0; y < nodeCountY; y++)
                    {
                        Vector2 pos = new Vector2(-mapWidth / 2 + x * squareSize + squareSize / 2, -mapHeight / 2 + y * squareSize + squareSize / 2);
                        controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                    }
                }

                squares = new Square[nodeCountX - 1, nodeCountY - 1];
                for (int x = 0; x < nodeCountX - 1; x++)
                {
                    for (int y = 0; y < nodeCountY - 1; y++)
                    {
                        squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                    }
                }

            }
        }

        public class Square
        {

            public ControlNode topLeft, topRight, bottomRight, bottomLeft;
            public Node centreTop, centreRight, centreBottom, centreLeft;
            public int configuration;

            public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)
            {
                topLeft = _topLeft;
                topRight = _topRight;
                bottomRight = _bottomRight;
                bottomLeft = _bottomLeft;

                centreTop = topLeft.right;
                centreRight = bottomRight.above;
                centreBottom = bottomLeft.right;
                centreLeft = bottomLeft.above;

                if (topLeft.active)
                    configuration += 8;
                if (topRight.active)
                    configuration += 4;
                if (bottomRight.active)
                    configuration += 2;
                if (bottomLeft.active)
                    configuration += 1;
            }

        }

        public class Node
        {
            public Vector2 position;
            public int vertexIndex = -1;

            public Node(Vector2 _pos)
            {
                position = _pos;
            }
        }

        public class ControlNode : Node
        {

            public bool active;
            public Node above, right;

            public ControlNode(Vector2 _pos, bool _active, float squareSize) : base(_pos)
            {
                active = _active;
                above = new Node(position + Vector2.up * squareSize / 2f);
                right = new Node(position + Vector2.right * squareSize / 2f);
            }

        }
    }
}