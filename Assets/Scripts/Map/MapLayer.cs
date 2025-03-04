using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ͼÿ��Ľڵ����
/// </summary>
[System.Serializable]
public class MapLayer
{
    [Tooltip("Ĭ�Ͻڵ����ͣ����randomizeNodesΪ0����100%Ϊ�����ͽڵ�")]
    public NodeType nodeType;
    [Tooltip("��ǰ��ڵ�ľ���")]
    public FloatMinMax distanceFromPreviousLayer;
    [Tooltip("ÿ������������ڵ�ľ���")]
    public float nodesApartDistance;
    [Tooltip("���λ�ã���Ϊ0�����ֱ��")]
    [Range(0f, 1f)] public float randomizePosition;
    [Tooltip("������������ͽڵ�ĸ��ʣ���Ϊ0����100%ΪĬ�Ͻڵ�����")]
    [Range(0f, 1f)] public float randomizeNodes;
}
