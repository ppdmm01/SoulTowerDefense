using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapConfig",menuName = "ScriptableObject/MapConfig")]
public class MapConfig : ScriptableObject
{
    [Tooltip("�ڵ���ͼ���洢�ڵ����ͼ�ͼƬ")]
    public List<NodeBlueprint> nodeBlueprints;
    [Tooltip("�������Ľڵ㣨layers���������")]
    public List<NodeType> randomNodes = new List<NodeType>
            {NodeType.Store, NodeType.Treasure, NodeType.MinorEnemy, NodeType.Forge};
    public int GridWidth => Mathf.Max(numOfPreBossNodes.max, numOfStartingNodes.max);

    [Tooltip("Bossǰһ������Ľڵ�����")]
    public IntMinMax numOfPreBossNodes;
    [Tooltip("��ʼ����Ľڵ�����")]
    public IntMinMax numOfStartingNodes;

    [Tooltip("ÿһ��ķֲ�����")]
    public List<MapLayer> layers;
}
