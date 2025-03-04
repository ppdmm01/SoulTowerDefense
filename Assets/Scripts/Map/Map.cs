using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

/// <summary>
/// ��ͼ��Ϣ
/// </summary>
public class Map
{
    public List<Node> nodes; //�ڵ�
    public List<Vector2Int> path; //��¼���ѡ���·��
    public string bossNodeName;
    public string configName; //��ͼ��

    public Map(string configName, string bossNodeName, List<Node> nodes, List<Vector2Int> path)
    {
        this.configName = configName;
        this.bossNodeName = bossNodeName;
        this.nodes = nodes;
        this.path = path;
    }

    /// <summary>
    /// ��ȡboss�ڵ�
    /// </summary>
    /// <returns></returns>
    public Node GetBossNode()
    {
        return nodes.FirstOrDefault(n => n.nodeType == NodeType.Boss);
    }

    /// <summary>
    /// ��ȡ��ͼ�ľ���
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
    /// ��ȡָ��λ�õĽڵ�
    /// </summary>
    public Node GetNode(Vector2Int point)
    {
        return nodes.FirstOrDefault(n => n.point.Equals(point));
    }
}
