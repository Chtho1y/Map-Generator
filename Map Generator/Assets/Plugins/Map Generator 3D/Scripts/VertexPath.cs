using System.Collections.Generic;
using UnityEngine;

namespace CNB
{
    /// <summary>
    /// Process the Bezier curves data created in "bezierPath.cs" to set data required of the points in the edge of the walls (position and normal) 
    /// </summary>
    public class VertexPath
    {
        public readonly Vector3[] localPoints;
        public readonly Vector3[] localTangents;
        public readonly Vector3[] localNormals;

        public readonly float[] times;
        public readonly float length;
        public readonly float[] cumulativeLengthAtEachVertex;
        public readonly Bounds bounds;
        public readonly Vector3 up;

        const int accuracy = 10;
        const float minVertexSpacing = .01f;

        Transform transform;

        public VertexPath(BezierPath bezierPath, Transform transform, float vertexSpacing) :
            this(bezierPath, SplitBezierPathEvenly(bezierPath, Mathf.Max(vertexSpacing, minVertexSpacing), accuracy), transform)
        { }

        VertexPath(BezierPath bezierPath, PathSplitData pathSplitData, Transform transform)
        {
            this.transform = transform;
            int numVerts = pathSplitData.vertices.Count;
            length = pathSplitData.cumulativeLength[numVerts - 1];

            localPoints = new Vector3[numVerts];
            localNormals = new Vector3[numVerts];
            localTangents = new Vector3[numVerts];
            cumulativeLengthAtEachVertex = new float[numVerts];
            times = new float[numVerts];
            bounds = new Bounds((pathSplitData.minMax.Min + pathSplitData.minMax.Max) / 2, pathSplitData.minMax.Max - pathSplitData.minMax.Min);

            up = bounds.size.z > bounds.size.y ? Vector3.up : -Vector3.forward;
            Vector3 lastRotationAxis = up;

            for (int i = 0; i < localPoints.Length; i++)
            {
                localPoints[i] = pathSplitData.vertices[i];
                localTangents[i] = pathSplitData.tangents[i];
                cumulativeLengthAtEachVertex[i] = pathSplitData.cumulativeLength[i];
                times[i] = cumulativeLengthAtEachVertex[i] / length;

                localNormals[i] = Vector3.Cross(localTangents[i], up) * -1;

            }
        }

        public int NumPoints
        {
            get
            {
                return localPoints.Length;
            }
        }

        public Vector3 GetTangent(int index)
        {
            return MathHelper.TransformDirection(localTangents[index], transform);
        }

        public Vector3 GetNormal(int index)
        {
            return MathHelper.TransformDirection(localNormals[index], transform);
        }

        public Vector3 GetPoint(int index)
        {
            return MathHelper.TransformPoint(localPoints[index], transform);
        }


        static PathSplitData SplitBezierPathEvenly(BezierPath bezierPath, float spacing, float accuracy)
        {
            PathSplitData splitData = new PathSplitData();

            splitData.vertices.Add(bezierPath[0]);
            splitData.tangents.Add(MathHelper.EvaluateCurveDerivative(bezierPath.GetPointsInSegment(0), 0).normalized);
            splitData.cumulativeLength.Add(0);
            splitData.anchorVertexMap.Add(0);
            splitData.minMax.AddValue(bezierPath[0]);

            Vector3 prevPointOnPath = bezierPath[0];
            Vector3 lastAddedPoint = bezierPath[0];

            float currentPathLength = 0;
            float dstSinceLastVertex = 0;

            for (int segmentIndex = 0; segmentIndex < bezierPath.NumSegments; segmentIndex++)
            {
                Vector3[] segmentPoints = bezierPath.GetPointsInSegment(segmentIndex);
                float estimatedSegmentLength = MathHelper.EstimateCurveLength(segmentPoints[0], segmentPoints[1], segmentPoints[2], segmentPoints[3]);
                int divisions = Mathf.CeilToInt(estimatedSegmentLength * accuracy);
                float increment = 1f / divisions;

                for (float t = increment; t <= 1; t += increment)
                {
                    bool isLastPointOnPath = t + increment > 1 && segmentIndex == bezierPath.NumSegments - 1;
                    if (isLastPointOnPath)
                    {
                        t = 1;
                    }
                    Vector3 pointOnPath = MathHelper.EvaluateCurve(segmentPoints, t);
                    dstSinceLastVertex += (pointOnPath - prevPointOnPath).magnitude;

                    if (dstSinceLastVertex > spacing)
                    {
                        float overshootDst = dstSinceLastVertex - spacing;
                        pointOnPath += (prevPointOnPath - pointOnPath).normalized * overshootDst;
                        t -= increment;
                    }

                    if (dstSinceLastVertex >= spacing || isLastPointOnPath)
                    {
                        currentPathLength += (lastAddedPoint - pointOnPath).magnitude;
                        splitData.cumulativeLength.Add(currentPathLength);
                        splitData.vertices.Add(pointOnPath);
                        splitData.tangents.Add(MathHelper.EvaluateCurveDerivative(segmentPoints, t).normalized);
                        splitData.minMax.AddValue(pointOnPath);
                        dstSinceLastVertex = 0;
                        lastAddedPoint = pointOnPath;
                    }
                    prevPointOnPath = pointOnPath;
                }
                splitData.anchorVertexMap.Add(splitData.vertices.Count - 1);
            }
            return splitData;
        }


        class PathSplitData
        {
            public List<Vector3> vertices = new List<Vector3>();
            public List<Vector3> tangents = new List<Vector3>();
            public List<float> cumulativeLength = new List<float>();
            public List<int> anchorVertexMap = new List<int>();
            public MinMax3D minMax = new MinMax3D();
        }

        public class MinMax3D
        {

            public Vector3 Min { get; private set; }
            public Vector3 Max { get; private set; }

            public MinMax3D()
            {
                Min = Vector3.one * float.MaxValue;
                Max = Vector3.one * float.MinValue;
            }

            public void AddValue(Vector3 v)
            {
                Min = new Vector3(Mathf.Min(Min.x, v.x), Mathf.Min(Min.y, v.y), Mathf.Min(Min.z, v.z));
                Max = new Vector3(Mathf.Max(Max.x, v.x), Mathf.Max(Max.y, v.y), Mathf.Max(Max.z, v.z));
            }
        }

    }
}