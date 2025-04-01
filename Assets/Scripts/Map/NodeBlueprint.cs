using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 节点类型
/// </summary>
public enum NodeType
{
    MinorEnemy, //普通关卡
    Boss, //Boss
    Crystal, //水晶碎片
    Store, //商铺
    Forge, //锻造坊
    Treasure, //宝箱
    Event, //事件
}

/// <summary>
/// 节点蓝图
/// </summary>
[CreateAssetMenu(fileName = "NodeBlueprint", menuName = "ScriptableObject/NodeBlueprint")]
public class NodeBlueprint : ScriptableObject
{
    public Sprite icon; //图片
    public NodeType nodeType; //类型
}
