using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

/// <summary>
/// 地图信息
/// </summary>
public class Map
{
    public List<Node> nodes; //节点
    public List<Vector2Int> path; //记录玩家选择的路径
    public string bossNodeName;
    public string configName; //地图名

    public Map(string configName, string bossNodeName, List<Node> nodes, List<Vector2Int> path)
    {
        this.configName = configName;
        this.bossNodeName = bossNodeName;
        this.nodes = nodes;
        this.path = path;
    }

    /// <summary>
    /// 获取boss节点
    /// </summary>
    /// <returns></returns>
    public Node GetBossNode()
    {
        return nodes.FirstOrDefault(n => n.nodeType == NodeType.Boss);
    }

    /// <summary>
    /// 获取地图的距离
    /// </summary>
    public float DistanceBetweenFirstAndLastLayers()
    {
        Node bossNode = GetBossNode();
        Node firstLayerNode = nodes.FirstOrDefault(n => n.point.y == 0);

        if (bossNode == null || firstLayerNode == null)
            return 0f;

        return bossNode.position.y - firstLayerNode.position.y;
    }

    /// <summary>
    /// 获取指定位置的节点
    /// </summary>
    public Node GetNode(Vector2Int point)
    {
        return nodes.FirstOrDefault(n => n.point.Equals(point));
    }
}
