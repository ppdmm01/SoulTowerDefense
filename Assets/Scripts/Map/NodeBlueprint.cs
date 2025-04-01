using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ڵ�����
/// </summary>
public enum NodeType
{
    MinorEnemy, //��ͨ�ؿ�
    Boss, //Boss
    Crystal, //ˮ����Ƭ
    Store, //����
    Forge, //���췻
    Treasure, //����
    Event, //�¼�
}

/// <summary>
/// �ڵ���ͼ
/// </summary>
[CreateAssetMenu(fileName = "NodeBlueprint", menuName = "ScriptableObject/NodeBlueprint")]
public class NodeBlueprint : ScriptableObject
{
    public Sprite icon; //ͼƬ
    public NodeType nodeType; //����
}
