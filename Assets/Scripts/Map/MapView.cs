using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapView : MonoBehaviour
{
    /// <summary>
    /// 地图方向
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

    [Tooltip("所有地图配置表")]
    public List<MapConfig> allMapConfigs;
    //节点预制体
    public GameObject nodePrefab;
    [Tooltip("地图边缘离屏幕的偏移距离")]
    public float orientationOffset;
    [Header("背景设置")]
    [Tooltip("为空则不显示地图")]
    public Sprite background;
    public Color32 backgroundColor = Color.white;
    [Tooltip("地图宽")]
    public float xSize;
    [Tooltip("地图高增加的偏移距离")]
    public float yOffset;

    [Header("线条设置")]
    public GameObject linePrefab;
    [Tooltip("线条的点")]
    [Range(3, 10)]
    public int linePointsCount = 10;
    [Tooltip("节点到线条开端的距离")]
    public float offsetFromNodes = 0.5f;

    [Header("颜色设置")]
    [Tooltip("可达或可访问的节点颜色")]
    public Color32 visitedColor = Color.white;
    [Tooltip("锁住的节点颜色")]
    public Color32 lockedColor = Color.gray;
    [Tooltip("可达或可访问的路径颜色")]
    public Color32 lineVisitedColor = Color.white;
    [Tooltip("不可达路径颜色")]
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
            Debug.LogWarning("在MapView.ShowMap()传的地图为空！！！");
            return;
        }

        Map = m;

        ClearMap(); //清理之前的地图

        CreateMapParent(); //创建地图父对象

        CreateNodes(m.nodes); //创建节点

        DrawLines();

        SetOrientation();

        ResetNodesRotation();

        SetAttainableNodes();

        //SetLineColors();

        CreateMapBackground(m);
    }

    /// <summary>
    /// 创建背景
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
    /// 创建地图父对象（用于拖拽）
    /// </summary>
    protected virtual void CreateMapParent()
    {
        //创建父对象
        firstParent = new GameObject("OuterMapParent"); //用于放在摄像机正中间
        mapParent = new GameObject("MapParentWithAScroll"); //用于滚动
        mapParent.transform.SetParent(firstParent.transform);
        //添加滚动脚本并设置冻结的方向
        ScrollNonUI scrollNonUi = mapParent.AddComponent<ScrollNonUI>();
        scrollNonUi.freezeX = orientation == MapOrientation.BottomToTop || orientation == MapOrientation.TopToBottom;
        scrollNonUi.freezeY = orientation == MapOrientation.LeftToRight || orientation == MapOrientation.RightToLeft;
        //添加碰撞体用于检测
        BoxCollider boxCollider = mapParent.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(100, 100, 1);
    }

    /// <summary>
    /// 创建全部地图节点
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
    /// 创建单个地图节点
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
    /// 设置可达节点
    /// </summary>
    public void SetAttainableNodes()
    {
        //先设置全部节点为不可达
        foreach (MapNode node in MapNodes)
            node.SetState(NodeStates.Locked);

        if (mapManager.CurrentMap.path.Count == 0)
        {
            // 未开始选择路线，第一层全设为可达节点
            foreach (MapNode node in MapNodes.Where(n => n.Node.point.y == 0))
                node.SetState(NodeStates.Attainable);
        }
        else
        {
            // 已经走过的路，设为已访问
            foreach (Vector2Int point in mapManager.CurrentMap.path)
            {
                MapNode mapNode = GetNode(point);
                if (mapNode != null)
                    mapNode.SetState(NodeStates.Visited);
            }

            //获取当前节点
            Vector2Int currentPoint = mapManager.CurrentMap.path[mapManager.CurrentMap.path.Count - 1];
            Node currentNode = mapManager.CurrentMap.GetNode(currentPoint);

            //设置所有当前能走到的节点为可达节点
            foreach (Vector2Int point in currentNode.outgoing)
            {
                MapNode mapNode = GetNode(point);
                if (mapNode != null)
                    mapNode.SetState(NodeStates.Attainable);
            }
        }
    }

    /// <summary>
    /// 设置方向
    /// </summary>
    protected virtual void SetOrientation()
    {
        ScrollNonUI scrollNonUi = mapParent.GetComponent<ScrollNonUI>();
        float span = mapManager.CurrentMap.DistanceBetweenFirstAndLastLayers();
        MapNode bossNode = MapNodes.FirstOrDefault(node => node.Node.nodeType == NodeType.Boss);
        Debug.Log("Map span in set orientation: " + span + " camera aspect: " + cam.aspect);

        // 将整张地图设置在相机的位置
        firstParent.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
        float offset = orientationOffset; //地图边缘离屏幕的偏移距离

        //根据设定的走向设置地图
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
                //旋转
                mapParent.transform.eulerAngles = new Vector3(0, 0, 180);
                //设置滚动的区间
                if (scrollNonUi != null)
                {
                    scrollNonUi.yConstraints.min = 0;
                    scrollNonUi.yConstraints.max = span + 2f * offset;
                }
                //移动父对象，让上下偏移距离一样
                firstParent.transform.localPosition += new Vector3(0, -offset, 0);
                break;
            case MapOrientation.RightToLeft:
                offset *= cam.aspect; //乘以宽高比计算横向偏移距离
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
    /// 重置所有图标的旋转角度
    /// </summary>
    private void ResetNodesRotation()
    {
        foreach (MapNode node in MapNodes)
            node.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// 画线
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
    /// 添加连接两点的线
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

        // 在local space画线:
        lineObject.transform.position = fromPoint;
        lineRenderer.useWorldSpace = false;

        //设置点位
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
    /// 获取指定位置的节点
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
