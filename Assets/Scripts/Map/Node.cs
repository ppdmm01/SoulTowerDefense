using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 节点类
/// </summary>
public class Node
{
    public readonly Vector2Int point; //节点位置
    public readonly List<Vector2Int> incoming = new List<Vector2Int>(); //进入该节点的节点列表
    public readonly List<Vector2Int> outgoing = new List<Vector2Int>(); //去往的节点列表
    public readonly NodeType nodeType; //节点类型
    public readonly string blueprintName; //节点对应蓝图名字
    public Vector2 position; //实际位置

    public Node(NodeType nodeType, string blueprintName, Vector2Int point)
    {
        this.nodeType = nodeType;
        this.blueprintName = blueprintName;
        this.point = point;
    }

    public void AddIncoming(Vector2Int p)
    {
        if (incoming.Any(element => element.Equals(p)))
            return;

        incoming.Add(p);
    }

    public void AddOutgoing(Vector2Int p)
    {
        if (outgoing.Any(element => element.Equals(p)))
            return;

        outgoing.Add(p);
    }

    public void RemoveIncoming(Vector2Int p)
    {
        incoming.RemoveAll(element => element.Equals(p));
    }

    public void RemoveOutgoing(Vector2Int p)
    {
        outgoing.RemoveAll(element => element.Equals(p));
    }

    /// <summary>
    /// 是否没有任何连接
    /// </summary>
    /// <returns></returns>
    public bool HasNoConnections()
    {
        return incoming.Count == 0 && outgoing.Count == 0;
    }
}
