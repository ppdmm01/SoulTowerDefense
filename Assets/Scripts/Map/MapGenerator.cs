using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MapGenerator
{
    private static MapConfig config; //��ͼ���ñ�

    private static List<float> layerDistances; //ÿ��ľ���
    private static readonly List<List<Node>> nodes = new List<List<Node>>(); //�洢���нڵ�

    /// <summary>
    /// ���ݵ�ͼ���ñ���Ϣ���ɵ�ͼ
    /// </summary>
    public static Map GetMap(MapConfig config)
    {
        if (config == null)
        {
            Debug.LogWarning("��ͼ���ñ�Ϊ�գ�����");
            return null;
        }

        MapGenerator.config = config;
        nodes.Clear();

        GenerateLayerDistances(); //�������ÿ���ľ���

        //ÿ�����ɽڵ�
        for (int i = 0; i < config.layers.Count; i++)
            PlaceLayer(i);

        //����·��
        List<List<Vector2Int>> paths = GeneratePaths();

        //RandomizeNodePositions();

        SetUpConnections(paths);

        RemoveCrossConnections();

        //ѡ�����������ӵĽڵ㣨û��ѡ���Ľڵ���ԣ�
        List<Node> nodesList = nodes.SelectMany(n => n).Where(n => n.incoming.Count > 0 || n.outgoing.Count > 0).ToList();

        //boss�ڵ�����
        string bossNodeName = config.nodeBlueprints.Where(b => b.nodeType == NodeType.Boss).ToList().Random().name;
        return new Map(config.name, bossNodeName, nodesList, new List<Vector2Int>());
    }

    /// <summary>
    /// �������ÿ���ľ���
    /// </summary>
    private static void GenerateLayerDistances()
    {
        layerDistances = new List<float>();
        foreach (MapLayer layer in config.layers)
            layerDistances.Add(layer.distanceFromPreviousLayer.GetValue());
    }

    /// <summary>
    /// ��ȡ�ӿ�ʼ��ָ���������ľ���
    /// </summary>
    private static float GetDistanceToLayer(int layerIndex)
    {
        if (layerIndex < 0 || layerIndex > layerDistances.Count) return 0f;

        return layerDistances.Take(layerIndex + 1).Sum();
    }

    /// <summary>
    /// ����ָ����Ľڵ�
    /// </summary>
    private static void PlaceLayer(int layerIndex)
    {
        MapLayer layer = config.layers[layerIndex]; //��ȡ�ò�����
        List<Node> nodesOnThisLayer = new List<Node>(); //�洢�ò�ڵ�

        float offset = layer.nodesApartDistance * config.GridWidth / 2f; //����������ĵ�ƫ��λ��

        for (int i = 0; i < config.GridWidth; i++)
        {
            //��ȡ֧������Ľڵ�����
            List<NodeType> supportedRandomNodeTypes =
                config.randomNodes.Where(t => config.nodeBlueprints.Any(b => b.nodeType == t)).ToList();

            //���ݹ��������ǰ�ڵ�����
            NodeType nodeType;
            if (Random.Range(0f, 1f) < layer.randomizeNodes && supportedRandomNodeTypes.Count > 0)
                nodeType = supportedRandomNodeTypes.Random(); //�������
            else
                nodeType = layer.nodeType; //Ĭ������

            //��ȡ��Ӧ�ڵ����͵���ͼ����
            string blueprintName = config.nodeBlueprints.Where(b => b.nodeType == nodeType).ToList().Random().name;

            //���ɽڵ�����
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
    /// Ϊ·���ϵ����нڵ㽨����ϵ
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
    /// �Ƴ�������
    /// </summary>
    private static void RemoveCrossConnections()
    {
        for (int i = 0; i < config.GridWidth - 1; ++i)
            for (int j = 0; j < config.layers.Count - 1; ++j)
            {
                //�ýڵ���ϡ��ҡ����Ͻڵ�����������һ�������ڣ��򲻻��������
                Node node = GetNode(new Vector2Int(i, j));
                if (node == null || node.HasNoConnections()) continue;
                Node right = GetNode(new Vector2Int(i + 1, j));
                if (right == null || right.HasNoConnections()) continue;
                Node top = GetNode(new Vector2Int(i, j + 1));
                if (top == null || top.HasNoConnections()) continue;
                Node topRight = GetNode(new Vector2Int(i + 1, j + 1));
                if (topRight == null || topRight.HasNoConnections()) continue;

                // ��������жϣ�����ڵ�û�������ߣ������ҽڵ�û�������ߣ���ô�Ͳ����������
                if (!node.outgoing.Any(element => element.Equals(topRight.point))) continue;
                if (!right.outgoing.Any(element => element.Equals(top.point))) continue;

                // ������ڵ�
                // 1) �������ڵ㶼���ֱ������:
                node.AddOutgoing(top.point);
                top.AddIncoming(node.point);

                right.AddOutgoing(topRight.point);
                topRight.AddIncoming(right.point);

                // ���һ�ִ���ʽ
                float rnd = Random.Range(0f, 1f);
                if (rnd < 0.2f)
                {
                    // ���ߵĽ�����ͬʱ�Ƴ�
                    // a) 
                    node.RemoveOutgoing(topRight.point);
                    topRight.RemoveIncoming(node.point);
                    // b) 
                    right.RemoveOutgoing(top.point);
                    top.RemoveIncoming(right.point);
                }
                else if (rnd < 0.6f)
                {
                    // a) �Ƴ����ڵ�Ľ�����
                    node.RemoveOutgoing(topRight.point);
                    topRight.RemoveIncoming(node.point);
                }
                else
                {
                    // b) �Ƴ��Ҳ�ڵ�Ľ�����
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
    /// �������սڵ��λ��Ϊ�м�
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
    /// ����·�����������յ�ǰ��ѡ����ɸѡָ�������ĵ㣬������·����
    /// </summary>
    private static List<List<Vector2Int>> GeneratePaths()
    {
        Vector2Int finalNode = GetFinalNode();
        var paths = new List<List<Vector2Int>>(); //�洢����·��
        int numOfStartingNodes = config.numOfStartingNodes.GetValue();
        int numOfPreBossNodes = config.numOfPreBossNodes.GetValue();

        List<int> candidateXs = new List<int>();
        for (int i = 0; i < config.GridWidth; i++)
            candidateXs.Add(i);

        //������������ѡ��
        candidateXs.Shuffle();
        IEnumerable<int> startingXs = candidateXs.Take(numOfStartingNodes);
        List<Vector2Int> startingPoints = (from x in startingXs select new Vector2Int(x, 0)).ToList();

        //���յ�ǰ�����ѡ��
        candidateXs.Shuffle();
        IEnumerable<int> preBossXs = candidateXs.Take(numOfPreBossNodes);
        List<Vector2Int> preBossPoints = (from x in preBossXs select new Vector2Int(x, finalNode.y - 1)).ToList();

        //���ɴ���㵽�յ�ǰ��·��
        int numOfPaths = Mathf.Max(numOfStartingNodes, numOfPreBossNodes);
        for (int i = 0; i < numOfPaths; ++i)
        {
            Vector2Int startNode = startingPoints[i % numOfStartingNodes];
            Vector2Int endNode = preBossPoints[i % numOfPreBossNodes];
            List<Vector2Int> path = Path(startNode, endNode); //������㵽�յ�ǰ��·��
            path.Add(finalNode); //�����㵽·��
            paths.Add(path);
        }

        return paths;
    }

    /// <summary>
    /// ������㵽�յ��·��
    /// </summary>
    private static List<Vector2Int> Path(Vector2Int fromPoint, Vector2Int toPoint)
    {
        int toRow = toPoint.y; //�յ��У�y��
        int toCol = toPoint.x; //�յ��У�x��

        int lastNodeCol = fromPoint.x; //��¼��һ�ڵ���У�x��

        List<Vector2Int> path = new List<Vector2Int> { fromPoint }; //�洢·��
        List<int> candidateCols = new List<int>(); //�洢��ѡ��

        //������򣺺���ľ���С�ڵ��������������㣬��֤�ܴӸýڵ㵽���յ�
        for (int row = 1; row < toRow; ++row)
        {
            candidateCols.Clear();

            int verticalDistance = toRow - row;
            int horizontalDistance;

            //��ǰ����һ���ڵ�
            int forwardCol = lastNodeCol;
            horizontalDistance = Mathf.Abs(toCol - forwardCol);
            if (horizontalDistance <= verticalDistance)
                candidateCols.Add(lastNodeCol);

            //��ǰ����һ���ڵ�
            int leftCol = lastNodeCol - 1;
            horizontalDistance = Mathf.Abs(toCol - leftCol);
            if (leftCol >= 0 && horizontalDistance <= verticalDistance)
                candidateCols.Add(leftCol);

            //��ǰ����һ���ڵ�
            int rightCol = lastNodeCol + 1;
            horizontalDistance = Mathf.Abs(toCol - rightCol);
            if (rightCol < config.GridWidth && horizontalDistance <= verticalDistance)
                candidateCols.Add(rightCol);

            //�������·�������ѡһ��
            int randomCandidateIndex = Random.Range(0, candidateCols.Count);
            int candidateCol = candidateCols[randomCandidateIndex];
            Vector2Int nextPoint = new Vector2Int(candidateCol, row);

            path.Add(nextPoint);

            lastNodeCol = candidateCol; //������һ�ڵ�
        }

        path.Add(toPoint);

        return path;
    }
}
