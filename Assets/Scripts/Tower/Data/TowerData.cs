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
    public string description;

    [Header("基础属性")]
    public int hp; //血量
    public int cost; //花费

    [Header("攻击相关")]
    public int damage; //伤害
    public float range; //攻击范围
    public float interval; //攻击间隔

    [Header("生产资源相关")]
    public int output; //每次产量
    public float cooldown; //生产冷却时间

    /// <summary>
    /// 初始化用
    /// </summary>
    /// <param name="towerSO"></param>
    public void Init(TowerSO towerSO)
    {
        towerName = towerSO.name;
        description = towerSO.description;
        hp = towerSO.hp;
        cost = towerSO.cost;
        damage = towerSO.damage;
        range = towerSO.range;
        interval = towerSO.interval;
        output = towerSO.output;
        cooldown = towerSO.cooldown;
    }
}
