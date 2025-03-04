using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图每层的节点规则
/// </summary>
[System.Serializable]
public class MapLayer
{
    [Tooltip("默认节点类型，如果randomizeNodes为0，则100%为该类型节点")]
    public NodeType nodeType;
    [Tooltip("离前面节点的距离")]
    public FloatMinMax distanceFromPreviousLayer;
    [Tooltip("每层的相邻两个节点的距离")]
    public float nodesApartDistance;
    [Tooltip("随机位置，若为0，则呈直线")]
    [Range(0f, 1f)] public float randomizePosition;
    [Tooltip("随机到其他类型节点的概率，若为0，则100%为默认节点类型")]
    [Range(0f, 1f)] public float randomizeNodes;
}
