using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MapGenerator
{
    private static MapConfig config; //地图配置表

    private static List<float> layerDistances; //每层的距离
    private static readonly List<List<Node>> nodes = new List<List<Node>>(); //存储所有节点

    /// <summary>
    /// 根据地图配置表信息生成地图
    /// </summary>
    public static Map GetMap(MapConfig config)
    {
        if (config == null)
        {
            Debug.LogWarning("地图配置表为空！！！");
            return null;
        }

        MapGenerator.config = config;
        nodes.Clear();

        GenerateLayerDistances(); //随机生成每层间的距离

        //每层生成节点
        for (int i = 0; i < config.layers.Count; i++)
            PlaceLayer(i);

        //生成路径
        List<List<Vector2Int>> paths = GeneratePaths();

        //RandomizeNodePositions();

        SetUpConnections(paths);

        RemoveCrossConnections();

        //选择所有已连接的节点（没有选到的节点忽略）
        List<Node> nodesList = nodes.SelectMany(n => n).Where(n => n.incoming.Count > 0 || n.outgoing.Count > 0).ToList();

        //boss节点名称
        string bossNodeName = config.nodeBlueprints.Where(b => b.nodeType == NodeType.Boss).ToList().Random().name;
        return new Map(config.name, bossNodeName, nodesList, new List<Vector2Int>());
    }

    /// <summary>
    /// 随机生成每层间的距离
    /// </summary>
    private static void GenerateLayerDistances()
    {
        layerDistances = new List<float>();
        foreach (MapLayer layer in config.layers)
            layerDistances.Add(layer.distanceFromPreviousLayer.GetValue());
    }

    /// <summary>
    /// 获取从开始到指定层索引的距离
    /// </summary>
    private static float GetDistanceToLayer(int layerIndex)
    {
        if (layerIndex < 0 || layerIndex > layerDistances.Count) return 0f;

        return layerDistances.Take(layerIndex + 1).Sum();
    }

    /// <summary>
    /// 生成指定层的节点
    /// </summary>
    private static void PlaceLayer(int layerIndex)
    {
        MapLayer layer = config.layers[layerIndex]; //获取该层配置
        List<Node> nodesOnThisLayer = new List<Node>(); //存储该层节点

        float offset = layer.nodesApartDistance * config.GridWidth / 2f; //计算相对中心的偏移位置

        for (int i = 0; i < config.GridWidth; i++)
        {
            //获取支持随机的节点类型
            List<NodeType> supportedRandomNodeTypes =
                config.randomNodes.Where(t => config.nodeBlueprints.Any(b => b.nodeType == t)).ToList();

            //根据规则随机当前节点类型
            NodeType nodeType;
            if (Random.Range(0f, 1f) < layer.randomizeNodes && supportedRandomNodeTypes.Count > 0)
                nodeType = supportedRandomNodeTypes.Random(); //随机类型
            else
                nodeType = layer.nodeType; //默认类型

            //获取对应节点类型的蓝图名字
            string blueprintName = config.nodeBlueprints.Where(b => b.nodeType == nodeType).ToList().Random().name;

            //生成节点数据
            Node node = new Node(nodeType, blueprintName, new Vector2Int(i, layerIndex))
            {
                position = new Vector2(-offset + i * layer.nodesApartDistance, GetDistanceToLayer(layerIndex)),
            };
            Debug.Log("LayerIndex:"+layerIndex + ":" + (-offset + i * layer.nodesApartDistance));
            nodesOnThisLayer.Add(node);
        }

        nodes.Add(nodesOnThisLayer);
    }

    //private static void RandomizeNodePositions()
    //{
    //    for (int index = 0; index < nodes.Count; index++)
    //    {
    //        List<Node> list = nodes[index];
    //        MapLayer layer = config.layers[index];
    //        float distToNextLayer = index + 1 >= layerDistances.Count
    //            ? 0f
    //            : layerDistances[index + 1];
    //        float distToPreviousLayer = layerDistances[index];

    //        foreach (Node node in list)
    //        {
    //            float xRnd = Random.Range(-0.5f, 0.5f);
    //            float yRnd = Random.Range(-0.5f, 0.5f);

    //            float x = xRnd * layer.nodesApartDistance;
    //            float y = yRnd < 0 ? distToPreviousLayer * yRnd : distToNextLayer * yRnd;

    //            node.position += new Vector2(x, y) * layer.randomizePosition;
    //        }
    //    }
    //}

    /// <summary>
    /// 为路径上的所有节点建立联系
    /// </summary>
    private static void SetUpConnections(List<List<Vector2Int>> paths)
    {
        foreach (List<Vector2Int> path in paths)
        {
            for (int i = 0; i < path.Count - 1; ++i)
            {
                Node node = GetNode(path[i]);
                Node nextNode = GetNode(path[i + 1]);
                node.AddOutgoing(nextNode.point);
                nextNode.AddIncoming(node.point);
            }
        }
    }

    /// <summary>
    /// 移除交叉线
    /// </summary>
    private static void RemoveCrossConnections()
    {
        for (int i = 0; i < config.GridWidth - 1; ++i)
            for (int j = 0; j < config.layers.Count - 1; ++j)
            {
                //该节点和上、右、右上节点若任意其中一个不存在，则不会产生交叉
                Node node = GetNode(new Vector2Int(i, j));
                if (node == null || node.HasNoConnections()) continue;
                Node right = GetNode(new Vector2Int(i + 1, j));
                if (right == null || right.HasNoConnections()) continue;
                Node top = GetNode(new Vector2Int(i, j + 1));
                if (top == null || top.HasNoConnections()) continue;
                Node topRight = GetNode(new Vector2Int(i + 1, j + 1));
                if (topRight == null || topRight.HasNoConnections()) continue;

                // 交叉存在判断：如果节点没有往右走，或者右节点没有往左走，那么就不会产生交叉
                if (!node.outgoing.Any(element => element.Equals(topRight.point))) continue;
                if (!right.outgoing.Any(element => element.Equals(top.point))) continue;

                // 处理交叉节点
                // 1) 左右两节点都添加直线连接:
                node.AddOutgoing(top.point);
                top.AddIncoming(node.point);

                right.AddOutgoing(topRight.point);
                topRight.AddIncoming(right.point);

                // 随机一种处理方式
                float rnd = Random.Range(0f, 1f);
                if (rnd < 0.2f)
                {
                    // 两边的交叉线同时移除
                    // a) 
                    node.RemoveOutgoing(topRight.point);
                    topRight.RemoveIncoming(node.point);
                    // b) 
                    right.RemoveOutgoing(top.point);
                    top.RemoveIncoming(right.point);
                }
                else if (rnd < 0.6f)
                {
                    // a) 移除左侧节点的交叉线
                    node.RemoveOutgoing(topRight.point);
                    topRight.RemoveIncoming(node.point);
                }
                else
                {
                    // b) 移除右侧节点的交叉线
                    right.RemoveOutgoing(top.point);
                    top.RemoveIncoming(right.point);
                }
            }
    }

    private static Node GetNode(Vector2Int p)
    {
        if (p.y >= nodes.Count) return null;
        if (p.x >= nodes[p.y].Count) return null;

        return nodes[p.y][p.x];
    }

    /// <summary>
    /// 设置最终节点的位置为中间
    /// </summary>
    private static Vector2Int GetFinalNode()
    {
        int y = config.layers.Count - 1;
        if (config.GridWidth % 2 == 1)
            return new Vector2Int(config.GridWidth / 2, y);

        return Random.Range(0, 2) == 0
            ? new Vector2Int(config.GridWidth / 2, y)
            : new Vector2Int(config.GridWidth / 2 - 1, y);
    }

    /// <summary>
    /// 生成路径（从起点和终点前候选点中筛选指定数量的点，并生成路径）
    /// </summary>
    private static List<List<Vector2Int>> GeneratePaths()
    {
        Vector2Int finalNode = GetFinalNode();
        var paths = new List<List<Vector2Int>>(); //存储最终路径
        int numOfStartingNodes = config.numOfStartingNodes.GetValue();
        int numOfPreBossNodes = config.numOfPreBossNodes.GetValue();

        List<int> candidateXs = new List<int>();
        for (int i = 0; i < config.GridWidth; i++)
            candidateXs.Add(i);

        //从起点中随机挑选点
        candidateXs.Shuffle();
        IEnumerable<int> startingXs = candidateXs.Take(numOfStartingNodes);
        List<Vector2Int> startingPoints = (from x in startingXs select new Vector2Int(x, 0)).ToList();

        //从终点前随机挑选点
        candidateXs.Shuffle();
        IEnumerable<int> preBossXs = candidateXs.Take(numOfPreBossNodes);
        List<Vector2Int> preBossPoints = (from x in preBossXs select new Vector2Int(x, finalNode.y - 1)).ToList();

        //生成从起点到终点前的路径
        int numOfPaths = Mathf.Max(numOfStartingNodes, numOfPreBossNodes);
        for (int i = 0; i < numOfPaths; ++i)
        {
            Vector2Int startNode = startingPoints[i % numOfStartingNodes];
            Vector2Int endNode = preBossPoints[i % numOfPreBossNodes];
            List<Vector2Int> path = Path(startNode, endNode); //生成起点到终点前的路径
            path.Add(finalNode); //添加最点到路径
            paths.Add(path);
        }

        return paths;
    }

    /// <summary>
    /// 建立起点到终点的路径
    /// </summary>
    private static List<Vector2Int> Path(Vector2Int fromPoint, Vector2Int toPoint)
    {
        int toRow = toPoint.y; //终点行（y）
        int toCol = toPoint.x; //终点列（x）

        int lastNodeCol = fromPoint.x; //记录上一节点的列（x）

        List<Vector2Int> path = new List<Vector2Int> { fromPoint }; //存储路径
        List<int> candidateCols = new List<int>(); //存储候选点

        //计算规则：横向的距离小于等于纵向距离才满足，保证能从该节点到达终点
        for (int row = 1; row < toRow; ++row)
        {
            candidateCols.Clear();

            int verticalDistance = toRow - row;
            int horizontalDistance;

            //正前方的一个节点
            int forwardCol = lastNodeCol;
            horizontalDistance = Mathf.Abs(toCol - forwardCol);
            if (horizontalDistance <= verticalDistance)
                candidateCols.Add(lastNodeCol);

            //左前方的一个节点
            int leftCol = lastNodeCol - 1;
            horizontalDistance = Mathf.Abs(toCol - leftCol);
            if (leftCol >= 0 && horizontalDistance <= verticalDistance)
                candidateCols.Add(leftCol);

            //右前方的一个节点
            int rightCol = lastNodeCol + 1;
            horizontalDistance = Mathf.Abs(toCol - rightCol);
            if (rightCol < config.GridWidth && horizontalDistance <= verticalDistance)
                candidateCols.Add(rightCol);

            //从满足的路径中随机选一个
            int randomCandidateIndex = Random.Range(0, candidateCols.Count);
            int candidateCol = candidateCols[randomCandidateIndex];
            Vector2Int nextPoint = new Vector2Int(candidateCol, row);

            path.Add(nextPoint);

            lastNodeCol = candidateCol; //更新上一节点
        }

        path.Add(toPoint);

        return path;
    }
}
