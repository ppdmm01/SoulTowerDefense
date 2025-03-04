using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapConfig",menuName = "ScriptableObject/MapConfig")]
public class MapConfig : ScriptableObject
{
    [Tooltip("节点蓝图，存储节点类型及图片")]
    public List<NodeBlueprint> nodeBlueprints;
    [Tooltip("当层数的节点（layers）的随机池")]
    public List<NodeType> randomNodes = new List<NodeType>
            {NodeType.Store, NodeType.Treasure, NodeType.MinorEnemy, NodeType.Forge};
    public int GridWidth => Mathf.Max(numOfPreBossNodes.max, numOfStartingNodes.max);

    [Tooltip("Boss前一个房间的节点数量")]
    public IntMinMax numOfPreBossNodes;
    [Tooltip("起始房间的节点数量")]
    public IntMinMax numOfStartingNodes;

    [Tooltip("每一层的分布规则")]
    public List<MapLayer> layers;
}
