using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WavesUI : BaseMeshEffect
{
    public enum enumMoveDirection
    {
        MoveToLeft,
        MoveToRight,
    }

    public enum enumWavesDirection
    {
        NONE = 0,
        WavesTop = 1,
        WavesBottom = 2,
        WavesTopAndBottom = 3,
        WavesLeft = 4,
        WavesRight = 5,
        WavesLeftAndRight = 6,
        WavesAll = 7,
    }

    public float speed = 1f;
    public float resolution = 10.0f;
    public enumMoveDirection moveDirection = enumMoveDirection.MoveToLeft;
    public enumWavesDirection waveDirection = enumWavesDirection.WavesTop;
    public float wavesHeight = 10f;
    public float wavesDistance = 10f;
    public float wavesOffset = 0;
    protected RectTransform rectTrans;
    protected List<UIVertex> reuse_quads = new List<UIVertex>();
    List<UIVertex> vertexList = new List<UIVertex>();
    List<Vector3> vertexPosList = new List<Vector3>();
    float time = 0;


    protected override void Start()
    {
        rectTrans = GetComponent<RectTransform>();
        time = wavesOffset;
    }

    private void Update()
    {
        time += Time.deltaTime * speed;
        graphic.SetVerticesDirty();
    }

    public override void ModifyMesh(Mesh mesh)
    {
        VertexHelper vh = new VertexHelper(mesh);
        ModifyMesh(vh);

        vh.FillMesh(mesh);
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (vertexList.Count == 0)
        {
            vh.GetUIVertexStream(vertexList);

            tessellateGraphic(vertexList);

            foreach (UIVertex v in vertexList)
            {
                vertexPosList.Add(v.position);
            }

        }

        for (int i = 0; i < vertexList.Count; i++)
        {
            var uiVertex = vertexList[i];

            if (rectTrans == null) return;

            float horRatio = (uiVertex.position.x + rectTrans.rect.width * rectTrans.pivot.x) / rectTrans.rect.width;
            float verRatio = (uiVertex.position.y + rectTrans.rect.height * rectTrans.pivot.y) / rectTrans.rect.height;

            Vector3 p = uiVertex.position;// * Random.value;

            switch (waveDirection)
            {
                case enumWavesDirection.NONE:
                    p = vertexPosList[i];
                    break;
                case enumWavesDirection.WavesTop:
                    if (moveDirection == enumMoveDirection.MoveToLeft)
                    {
                        p.x = vertexPosList[i].x;
                        p.y = vertexPosList[i].y + Mathf.Sin(time + horRatio * wavesDistance) * verRatio * wavesHeight;
                    }
                    else
                    {
                        p.x = vertexPosList[i].x;
                        p.y = vertexPosList[i].y + Mathf.Sin(time - horRatio * wavesDistance) * verRatio * wavesHeight;
                    }
                    break;
                case enumWavesDirection.WavesBottom:
                    if (moveDirection == enumMoveDirection.MoveToLeft)
                    {
                        p.x = vertexPosList[i].x;
                        p.y = vertexPosList[i].y + Mathf.Sin(time + horRatio * wavesDistance) * (1 - verRatio) * wavesHeight;
                    }
                    else
                    {
                        p.x = vertexPosList[i].x;
                        p.y = vertexPosList[i].y + Mathf.Sin(time - horRatio * wavesDistance) * (1 - verRatio) * wavesHeight;
                    }
                    break;
                case enumWavesDirection.WavesTopAndBottom:
                    if (moveDirection == enumMoveDirection.MoveToLeft)
                    {
                        p.x = vertexPosList[i].x;
                        p.y = vertexPosList[i].y + Mathf.Sin(time + horRatio * wavesDistance) * verRatio * wavesHeight;
                        p.y += Mathf.Sin(time + horRatio * wavesDistance) * (1 - verRatio) * wavesHeight;
                    }
                    else
                    {
                        p.x = vertexPosList[i].x;
                        p.y = vertexPosList[i].y + Mathf.Sin(time - horRatio * wavesDistance) * verRatio * wavesHeight;
                        p.y += Mathf.Sin(time - horRatio * wavesDistance) * (1 - verRatio) * wavesHeight;
                    }
                    break;
                case enumWavesDirection.WavesLeft:
                    if (moveDirection == enumMoveDirection.MoveToLeft)
                    {
                        p.x = vertexPosList[i].x + Mathf.Sin(time + verRatio * wavesDistance) * (1 - horRatio) * wavesHeight;
                        p.y = vertexPosList[i].y;
                    }
                    else
                    {
                        p.x = vertexPosList[i].x + Mathf.Sin(time - verRatio * wavesDistance) * (1 - horRatio) * wavesHeight;
                        p.y = vertexPosList[i].y;
                    }
                    break;
                case enumWavesDirection.WavesRight:
                    if (moveDirection == enumMoveDirection.MoveToLeft)
                    {
                        p.x = vertexPosList[i].x + Mathf.Sin(time + verRatio * wavesDistance) * horRatio * wavesHeight;
                        p.y = vertexPosList[i].y;
                    }
                    else
                    {
                        p.x = vertexPosList[i].x + Mathf.Sin(time - verRatio * wavesDistance) * horRatio * wavesHeight;
                        p.y = vertexPosList[i].y;
                    }
                    break;
                case enumWavesDirection.WavesLeftAndRight:
                    if (moveDirection == enumMoveDirection.MoveToLeft)
                    {
                        p.x = vertexPosList[i].x + Mathf.Sin(time + verRatio * wavesDistance) * (1 - horRatio) * wavesHeight;
                        p.x += Mathf.Sin(time + verRatio * wavesDistance) * horRatio * wavesHeight;
                        p.y = vertexPosList[i].y;
                    }
                    else
                    {
                        p.x = vertexPosList[i].x + Mathf.Sin(time - verRatio * wavesDistance) * (1 - horRatio) * wavesHeight;
                        p.x += Mathf.Sin(time - verRatio * wavesDistance) * horRatio * wavesHeight;
                        p.y = vertexPosList[i].y;
                    }
                    break;
                case enumWavesDirection.WavesAll:
                    if (moveDirection == enumMoveDirection.MoveToLeft)
                    {
                        p.x = vertexPosList[i].x + Mathf.Sin(time + verRatio * wavesDistance) * (1 - horRatio) * wavesHeight;
                        p.x += Mathf.Sin(time + verRatio * wavesDistance) * horRatio * wavesHeight;

                        p.y = vertexPosList[i].y + Mathf.Sin(time + horRatio * wavesDistance) * verRatio * wavesHeight;
                        p.y += Mathf.Sin(time + horRatio * wavesDistance) * (1 - verRatio) * wavesHeight;
                    }
                    else
                    {
                        p.x = vertexPosList[i].x + Mathf.Sin(time - verRatio * wavesDistance) * (1 - horRatio) * wavesHeight;
                        p.x += Mathf.Sin(time - verRatio * wavesDistance) * horRatio * wavesHeight;

                        p.y = vertexPosList[i].y + Mathf.Sin(time - horRatio * wavesDistance) * verRatio * wavesHeight;
                        p.y += Mathf.Sin(time - horRatio * wavesDistance) * (1 - verRatio) * wavesHeight;
                    }
                    break;
            }

            uiVertex.position = p;
            vertexList[i] = uiVertex;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertexList);
    }

    public void ChangeWaveDirection(enumWavesDirection waveDirection)
    {
        this.waveDirection = waveDirection;
    }

    public void ChangeWaveDirection(int wave)
    {
        switch (wave)
        {
            case 0:
                waveDirection = enumWavesDirection.NONE;
                break;
            case 1:
                waveDirection = enumWavesDirection.WavesTop;
                break;
            case 2:
                waveDirection = enumWavesDirection.WavesBottom;
                break;
            case 3:
                waveDirection = enumWavesDirection.WavesTopAndBottom;
                break;
            case 4:
                waveDirection = enumWavesDirection.WavesLeft;
                break;
            case 5:
                waveDirection = enumWavesDirection.WavesRight;
                break;
            case 6:
                waveDirection = enumWavesDirection.WavesLeftAndRight;
                break;
            case 7:
                waveDirection = enumWavesDirection.WavesAll;
                break;
        }

    }

    #region tessellate
    protected void tessellateGraphic(List<UIVertex> _verts)
    {
        for (int v = 0; v < _verts.Count; v += 6)
        {
            reuse_quads.Add(_verts[v]); // bottom left
            reuse_quads.Add(_verts[v + 1]); // top left
            reuse_quads.Add(_verts[v + 2]); // top right
                                            // verts[3] is redundant, top right
            reuse_quads.Add(_verts[v + 4]); // bottom right
                                            // verts[5] is redundant, bottom left
        }

        int oriQuadNum = reuse_quads.Count / 4;
        for (int q = 0; q < oriQuadNum; q++)
        {
            tessellateQuad(reuse_quads, q * 4);
        }

        // remove original quads
        reuse_quads.RemoveRange(0, oriQuadNum * 4);

        _verts.Clear();

        // process new quads and turn them into triangles
        for (int q = 0; q < reuse_quads.Count; q += 4)
        {
            _verts.Add(reuse_quads[q]);
            _verts.Add(reuse_quads[q + 1]);
            _verts.Add(reuse_quads[q + 2]);
            _verts.Add(reuse_quads[q + 2]);
            _verts.Add(reuse_quads[q + 3]);
            _verts.Add(reuse_quads[q]);
        }

        reuse_quads.Clear();
    }

    protected void tessellateQuad(List<UIVertex> _quads, int _thisQuadIdx)
    {
        UIVertex v_bottomLeft = _quads[_thisQuadIdx];
        UIVertex v_topLeft = _quads[_thisQuadIdx + 1];
        UIVertex v_topRight = _quads[_thisQuadIdx + 2];
        UIVertex v_bottomRight = _quads[_thisQuadIdx + 3];

        float quadSize = 100.0f / resolution;

        int heightQuadEdgeNum = Mathf.Max(1, Mathf.CeilToInt((v_topLeft.position - v_bottomLeft.position).magnitude / quadSize));
        int widthQuadEdgeNum = Mathf.Max(1, Mathf.CeilToInt((v_topRight.position - v_topLeft.position).magnitude / quadSize));

        int quadIdx = 0;

        for (int x = 0; x < widthQuadEdgeNum; x++)
        {
            for (int y = 0; y < heightQuadEdgeNum; y++, quadIdx++)
            {
                _quads.Add(new UIVertex());
                _quads.Add(new UIVertex());
                _quads.Add(new UIVertex());
                _quads.Add(new UIVertex());

                float xRatio = (float)x / widthQuadEdgeNum;
                float yRatio = (float)y / heightQuadEdgeNum;
                float xPlusOneRatio = (float)(x + 1) / widthQuadEdgeNum;
                float yPlusOneRatio = (float)(y + 1) / heightQuadEdgeNum;

                _quads[_quads.Count - 4] = uiVertexBerp(v_bottomLeft, v_topLeft, v_topRight, v_bottomRight, xRatio, yRatio);
                _quads[_quads.Count - 3] = uiVertexBerp(v_bottomLeft, v_topLeft, v_topRight, v_bottomRight, xRatio, yPlusOneRatio);
                _quads[_quads.Count - 2] = uiVertexBerp(v_bottomLeft, v_topLeft, v_topRight, v_bottomRight, xPlusOneRatio, yPlusOneRatio);
                _quads[_quads.Count - 1] = uiVertexBerp(v_bottomLeft, v_topLeft, v_topRight, v_bottomRight, xPlusOneRatio, yRatio);

            }
        }
    }

    protected UIVertex uiVertexBerp(UIVertex v_bottomLeft, UIVertex v_topLeft, UIVertex v_topRight, UIVertex v_bottomRight, float _xTime, float _yTime)
    {
        UIVertex topX = uiVertexLerp(v_topLeft, v_topRight, _xTime);
        UIVertex bottomX = uiVertexLerp(v_bottomLeft, v_bottomRight, _xTime);
        return uiVertexLerp(bottomX, topX, _yTime);
    }

    protected UIVertex uiVertexLerp(UIVertex _a, UIVertex _b, float _time)
    {
        UIVertex tmpUIVertex = new UIVertex();

        tmpUIVertex.position = Vector3.Lerp(_a.position, _b.position, _time);
        tmpUIVertex.normal = Vector3.Lerp(_a.normal, _b.normal, _time);
        tmpUIVertex.tangent = Vector3.Lerp(_a.tangent, _b.tangent, _time);
        tmpUIVertex.uv0 = Vector2.Lerp(_a.uv0, _b.uv0, _time);
        tmpUIVertex.uv1 = Vector2.Lerp(_a.uv1, _b.uv1, _time);
        tmpUIVertex.color = Color.Lerp(_a.color, _b.color, _time);

        return tmpUIVertex;
    }
    #endregion
}