using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储防御塔最终数据（配置数据+动态修改的数据）
/// </summary>
public class TowerData
{
    [Header("防御塔描述")]
    public string towerName;
    public string towerChineseName;
    public string description;
    public Sprite towerIcon;

    [Header("基础属性")]
    public int hp; //血量
    public int cost; //花费

    [Header("攻击相关")]
    public bool isAttacker; //是否是攻击者
    public int damage; //伤害
    public float range; //攻击范围
    public float interval; //攻击间隔

    [Header("生产资源相关")]
    public bool isProducer; //是否生产资源
    public int output; //每次产量
    public float cooldown; //生产冷却时间

    [Header("标签")]
    public List<ItemTag> itemTags; //用于与物品产生联动

    /// <summary>
    /// 初始化用
    /// </summary>
    /// <param name="towerSO"></param>
    public void Init(TowerSO towerSO)
    {
        towerName = towerSO.towerName;
        towerChineseName = towerSO.towerChineseName;
        description = towerSO.description;
        towerIcon = towerSO.towerIcon;
        isAttacker = towerSO.isAttacker;
        hp = towerSO.hp;
        cost = towerSO.cost;
        damage = towerSO.damage;
        range = towerSO.range;
        interval = towerSO.interval;
        isProducer = towerSO.isProducer;
        output = towerSO.output;
        cooldown = towerSO.cooldown;
        itemTags = towerSO.itemTags;
    }
}
