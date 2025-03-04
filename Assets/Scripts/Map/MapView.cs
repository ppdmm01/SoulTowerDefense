using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapView : MonoBehaviour
{
    /// <summary>
    /// ��ͼ����
    /// </summary>
    public enum MapOrientation
    {
        BottomToTop,
        TopToBottom,
        RightToLeft,
        LeftToRight
    }

    public MapManager mapManager;
    public MapOrientation orientation;

    [Tooltip("���е�ͼ���ñ�")]
    public List<MapConfig> allMapConfigs;
    //�ڵ�Ԥ����
    public GameObject nodePrefab;
    [Tooltip("��ͼ��Ե����Ļ��ƫ�ƾ���")]
    public float orientationOffset;
    [Header("��������")]
    [Tooltip("Ϊ������ʾ��ͼ")]
    public Sprite background;
    public Color32 backgroundColor = Color.white;
    [Tooltip("��ͼ��")]
    public float xSize;
    [Tooltip("��ͼ�����ӵ�ƫ�ƾ���")]
    public float yOffset;

    [Header("��������")]
    public GameObject linePrefab;
    [Tooltip("�����ĵ�")]
    [Range(3, 10)]
    public int linePointsCount = 10;
    [Tooltip("�ڵ㵽�������˵ľ���")]
    public float offsetFromNodes = 0.5f;

    [Header("��ɫ����")]
    [Tooltip("�ɴ��ɷ��ʵĽڵ���ɫ")]
    public Color32 visitedColor = Color.white;
    [Tooltip("��ס�Ľڵ���ɫ")]
    public Color32 lockedColor = Color.gray;
    [Tooltip("�ɴ��ɷ��ʵ�·����ɫ")]
    public Color32 lineVisitedColor = Color.white;
    [Tooltip("���ɴ�·����ɫ")]
    public Color32 lineLockedColor = Color.gray;

    protected GameObject firstParent;
    protected GameObject mapParent;
    private List<List<Vector2Int>> paths;
    private Camera cam;
    // ALL nodes:
    public readonly List<MapNode> MapNodes = new List<MapNode>();
    protected readonly List<LineConnection> lineConnections = new List<LineConnection>();

    public static MapView Instance;

    public Map Map { get; protected set; }

    private void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }

    protected virtual void ClearMap()
    {
        if (firstParent != null)
            Destroy(firstParent);

        MapNodes.Clear();
        lineConnections.Clear();
    }

    public virtual void ShowMap(Map m)
    {
        if (m == null)
        {
            Debug.LogWarning("��MapView.ShowMap()���ĵ�ͼΪ�գ�����");
            return;
        }

        Map = m;

        ClearMap(); //����֮ǰ�ĵ�ͼ

        CreateMapParent(); //������ͼ������

        CreateNodes(m.nodes); //�����ڵ�

        DrawLines();

        SetOrientation();

        ResetNodesRotation();

        SetAttainableNodes();

        //SetLineColors();

        CreateMapBackground(m);
    }

    /// <summary>
    /// ��������
    /// </summary>
    protected virtual void CreateMapBackground(Map m)
    {
        if (background == null) return;

        GameObject backgroundObject = new GameObject("Background");
        backgroundObject.transform.SetParent(mapParent.transform);
        MapNode bossNode = MapNodes.FirstOrDefault(node => node.Node.nodeType == NodeType.Boss);
        float span = m.DistanceBetweenFirstAndLastLayers();
        backgroundObject.transform.localPosition = new Vector3(bossNode.transform.localPosition.x, span / 2f, 0f);
        backgroundObject.transform.localRotation = Quaternion.identity;
        SpriteRenderer sr = backgroundObject.AddComponent<SpriteRenderer>();
        sr.color = backgroundColor;
        sr.drawMode = SpriteDrawMode.Sliced;
        sr.sprite = background;
        sr.size = new Vector2(xSize, span + yOffset * 2f);
    }

    /// <summary>
    /// ������ͼ������������ק��
    /// </summary>
    protected virtual void CreateMapParent()
    {
        //����������
        firstParent = new GameObject("OuterMapParent"); //���ڷ�����������м�
        mapParent = new GameObject("MapParentWithAScroll"); //���ڹ���
        mapParent.transform.SetParent(firstParent.transform);
        //��ӹ����ű������ö���ķ���
        ScrollNonUI scrollNonUi = mapParent.AddComponent<ScrollNonUI>();
        scrollNonUi.freezeX = orientation == MapOrientation.BottomToTop || orientation == MapOrientation.TopToBottom;
        scrollNonUi.freezeY = orientation == MapOrientation.LeftToRight || orientation == MapOrientation.RightToLeft;
        //�����ײ�����ڼ��
        BoxCollider boxCollider = mapParent.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(100, 100, 1);
    }

    /// <summary>
    /// ����ȫ����ͼ�ڵ�
    /// </summary>
    protected void CreateNodes(IEnumerable<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            MapNode mapNode = CreateMapNode(node);
            MapNodes.Add(mapNode);
        }
    }

    /// <summary>
    /// ����������ͼ�ڵ�
    /// </summary>
    protected virtual MapNode CreateMapNode(Node node)
    {
        GameObject mapNodeObject = Instantiate(nodePrefab, mapParent.transform);
        MapNode mapNode = mapNodeObject.GetComponent<MapNode>();
        NodeBlueprint blueprint = GetBlueprint(node.blueprintName);
        mapNode.SetUp(node, blueprint);
        mapNode.transform.localPosition = node.position;
        return mapNode;
    }

    /// <summary>
    /// ���ÿɴ�ڵ�
    /// </summary>
    public void SetAttainableNodes()
    {
        //������ȫ���ڵ�Ϊ���ɴ�
        foreach (MapNode node in MapNodes)
            node.SetState(NodeStates.Locked);

        if (mapManager.CurrentMap.path.Count == 0)
        {
            // δ��ʼѡ��·�ߣ���һ��ȫ��Ϊ�ɴ�ڵ�
            foreach (MapNode node in MapNodes.Where(n => n.Node.point.y == 0))
                node.SetState(NodeStates.Attainable);
        }
        else
        {
            // �Ѿ��߹���·����Ϊ�ѷ���
            foreach (Vector2Int point in mapManager.CurrentMap.path)
            {
                MapNode mapNode = GetNode(point);
                if (mapNode != null)
                    mapNode.SetState(NodeStates.Visited);
            }

            //��ȡ��ǰ�ڵ�
            Vector2Int currentPoint = mapManager.CurrentMap.path[mapManager.CurrentMap.path.Count - 1];
            Node currentNode = mapManager.CurrentMap.GetNode(currentPoint);

            //�������е�ǰ���ߵ��Ľڵ�Ϊ�ɴ�ڵ�
            foreach (Vector2Int point in currentNode.outgoing)
            {
                MapNode mapNode = GetNode(point);
                if (mapNode != null)
                    mapNode.SetState(NodeStates.Attainable);
            }
        }
    }

    /// <summary>
    /// ���÷���
    /// </summary>
    protected virtual void SetOrientation()
    {
        ScrollNonUI scrollNonUi = mapParent.GetComponent<ScrollNonUI>();
        float span = mapManager.CurrentMap.DistanceBetweenFirstAndLastLayers();
        MapNode bossNode = MapNodes.FirstOrDefault(node => node.Node.nodeType == NodeType.Boss);
        Debug.Log("Map span in set orientation: " + span + " camera aspect: " + cam.aspect);

        // �����ŵ�ͼ�����������λ��
        firstParent.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
        float offset = orientationOffset; //��ͼ��Ե����Ļ��ƫ�ƾ���

        //�����趨���������õ�ͼ
        switch (orientation)
        {
            case MapOrientation.BottomToTop:
                if (scrollNonUi != null)
                {
                    scrollNonUi.yConstraints.max = 0;
                    scrollNonUi.yConstraints.min = -(span + 2f * offset);
                }
                firstParent.transform.localPosition += new Vector3(0, offset, 0);
                break;
            case MapOrientation.TopToBottom:
                //��ת
                mapParent.transform.eulerAngles = new Vector3(0, 0, 180);
                //���ù���������
                if (scrollNonUi != null)
                {
                    scrollNonUi.yConstraints.min = 0;
                    scrollNonUi.yConstraints.max = span + 2f * offset;
                }
                //�ƶ�������������ƫ�ƾ���һ��
                firstParent.transform.localPosition += new Vector3(0, -offset, 0);
                break;
            case MapOrientation.RightToLeft:
                offset *= cam.aspect; //���Կ�߱ȼ������ƫ�ƾ���
                mapParent.transform.eulerAngles = new Vector3(0, 0, 90);
                firstParent.transform.localPosition -= new Vector3(offset, bossNode.transform.position.y, 0);
                if (scrollNonUi != null)
                {
                    scrollNonUi.xConstraints.max = span + 2f * offset;
                    scrollNonUi.xConstraints.min = 0;
                }
                break;
            case MapOrientation.LeftToRight:
                offset *= cam.aspect;
                mapParent.transform.eulerAngles = new Vector3(0, 0, -90);
                firstParent.transform.localPosition += new Vector3(offset, -bossNode.transform.position.y, 0);
                if (scrollNonUi != null)
                {
                    scrollNonUi.xConstraints.max = 0;
                    scrollNonUi.xConstraints.min = -(span + 2f * offset);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    //public virtual void SetLineColors()
    //{
    //    // set all lines to grayed out first:
    //    foreach (LineConnection connection in lineConnections)
    //        connection.SetColor(lineLockedColor);

    //    // set all lines that are a part of the path to visited color:
    //    // if we have not started moving on the map yet, leave everything as is:
    //    if (mapManager.CurrentMap.path.Count == 0)
    //        return;

    //    // in any case, we mark outgoing connections from the final node with visible/attainable color:
    //    Vector2Int currentPoint = mapManager.CurrentMap.path[mapManager.CurrentMap.path.Count - 1];
    //    Node currentNode = mapManager.CurrentMap.GetNode(currentPoint);

    //    foreach (Vector2Int point in currentNode.outgoing)
    //    {
    //        LineConnection lineConnection = lineConnections.FirstOrDefault(conn => conn.from.Node == currentNode &&
    //                                                                    conn.to.Node.point.Equals(point));
    //        lineConnection?.SetColor(lineVisitedColor);
    //    }

    //    if (mapManager.CurrentMap.path.Count <= 1) return;

    //    for (int i = 0; i < mapManager.CurrentMap.path.Count - 1; i++)
    //    {
    //        Vector2Int current = mapManager.CurrentMap.path[i];
    //        Vector2Int next = mapManager.CurrentMap.path[i + 1];
    //        LineConnection lineConnection = lineConnections.FirstOrDefault(conn => conn.@from.Node.point.Equals(current) &&
    //                                                                    conn.to.Node.point.Equals(next));
    //        lineConnection?.SetColor(lineVisitedColor);
    //    }
    //}

    //protected virtual void SetOrientation()
    //{
    //    ScrollNonUI scrollNonUi = mapParent.GetComponent<ScrollNonUI>();
    //    float span = mapManager.CurrentMap.DistanceBetweenFirstAndLastLayers();
    //    MapNode bossNode = MapNodes.FirstOrDefault(node => node.Node.nodeType == NodeType.Boss);
    //    Debug.Log("Map span in set orientation: " + span + " camera aspect: " + cam.aspect);

    //    // setting first parent to be right in front of the camera first:
    //    firstParent.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
    //    float offset = orientationOffset;
    //    switch (orientation)
    //    {
    //        case MapOrientation.BottomToTop:
    //            if (scrollNonUi != null)
    //            {
    //                scrollNonUi.yConstraints.max = 0;
    //                scrollNonUi.yConstraints.min = -(span + 2f * offset);
    //            }
    //            firstParent.transform.localPosition += new Vector3(0, offset, 0);
    //            break;
    //        case MapOrientation.TopToBottom:
    //            mapParent.transform.eulerAngles = new Vector3(0, 0, 180);
    //            if (scrollNonUi != null)
    //            {
    //                scrollNonUi.yConstraints.min = 0;
    //                scrollNonUi.yConstraints.max = span + 2f * offset;
    //            }
    //            // factor in map span:
    //            firstParent.transform.localPosition += new Vector3(0, -offset, 0);
    //            break;
    //        case MapOrientation.RightToLeft:
    //            offset *= cam.aspect;
    //            mapParent.transform.eulerAngles = new Vector3(0, 0, 90);
    //            // factor in map span:
    //            firstParent.transform.localPosition -= new Vector3(offset, bossNode.transform.position.y, 0);
    //            if (scrollNonUi != null)
    //            {
    //                scrollNonUi.xConstraints.max = span + 2f * offset;
    //                scrollNonUi.xConstraints.min = 0;
    //            }
    //            break;
    //        case MapOrientation.LeftToRight:
    //            offset *= cam.aspect;
    //            mapParent.transform.eulerAngles = new Vector3(0, 0, -90);
    //            firstParent.transform.localPosition += new Vector3(offset, -bossNode.transform.position.y, 0);
    //            if (scrollNonUi != null)
    //            {
    //                scrollNonUi.xConstraints.max = 0;
    //                scrollNonUi.xConstraints.min = -(span + 2f * offset);
    //            }
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException();
    //    }
    //}

    //private void DrawLines()
    //{
    //    foreach (MapNode node in MapNodes)
    //    {
    //        foreach (Vector2Int connection in node.Node.outgoing)
    //            AddLineConnection(node, GetNode(connection));
    //    }
    //}

    /// <summary>
    /// ��������ͼ�����ת�Ƕ�
    /// </summary>
    private void ResetNodesRotation()
    {
        foreach (MapNode node in MapNodes)
            node.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// ����
    /// </summary>
    private void DrawLines()
    {
        foreach (MapNode node in MapNodes)
        {
            foreach (Vector2Int connection in node.Node.outgoing)
                AddLineConnection(node, GetNode(connection));
        }
    }

    /// <summary>
    /// ��������������
    /// </summary>
    protected virtual void AddLineConnection(MapNode from, MapNode to)
    {
        if (linePrefab == null) return;

        GameObject lineObject = Instantiate(linePrefab, mapParent.transform);
        LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();
        Vector3 fromPoint = from.transform.position +
                            (to.transform.position - from.transform.position).normalized * offsetFromNodes;

        Vector3 toPoint = to.transform.position +
                          (from.transform.position - to.transform.position).normalized * offsetFromNodes;

        // ��local space����:
        lineObject.transform.position = fromPoint;
        lineRenderer.useWorldSpace = false;

        //���õ�λ
        // line renderer with 2 points only does not handle transparency properly:
        lineRenderer.positionCount = linePointsCount;
        for (int i = 0; i < linePointsCount; i++)
        {
            lineRenderer.SetPosition(i,
                Vector3.Lerp(Vector3.zero, toPoint - fromPoint, (float)i / (linePointsCount - 1)));
        }

        //DottedLineRenderer dottedLine = lineObject.GetComponent<DottedLineRenderer>();
        //if (dottedLine != null) dottedLine.ScaleMaterial();

        lineConnections.Add(new LineConnection(lineRenderer, from, to));
    }

    /// <summary>
    /// ��ȡָ��λ�õĽڵ�
    /// </summary>
    protected MapNode GetNode(Vector2Int p)
    {
        return MapNodes.FirstOrDefault(n => n.Node.point.Equals(p));
    }

    protected MapConfig GetConfig(string configName)
    {
        return allMapConfigs.FirstOrDefault(c => c.name == configName);
    }

    protected NodeBlueprint GetBlueprint(NodeType type)
    {
        MapConfig config = GetConfig(mapManager.CurrentMap.configName);
        return config.nodeBlueprints.FirstOrDefault(n => n.nodeType == type);
    }

    protected NodeBlueprint GetBlueprint(string blueprintName)
    {
        MapConfig config = GetConfig(mapManager.CurrentMap.configName);
        return config.nodeBlueprints.FirstOrDefault(n => n.name == blueprintName);
    }
}
