using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �ڵ���
/// </summary>
public class Node
{
    public readonly Vector2Int point; //�ڵ�λ��
    public readonly List<Vector2Int> incoming = new List<Vector2Int>(); //����ýڵ�Ľڵ��б�
    public readonly List<Vector2Int> outgoing = new List<Vector2Int>(); //ȥ���Ľڵ��б�
    public readonly NodeType nodeType; //�ڵ�����
    public readonly string blueprintName; //�ڵ��Ӧ��ͼ����
    public Vector2 position; //ʵ��λ��

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
    /// �Ƿ�û���κ�����
    /// </summary>
    /// <returns></returns>
    public bool HasNoConnections()
    {
        return incoming.Count == 0 && outgoing.Count == 0;
    }
}
