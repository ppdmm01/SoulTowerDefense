using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    Burn, //灼烧
    Slow, //减速
    Stun, //眩晕
}

/// <summary>
/// Buff数据
/// </summary>
[Serializable]
public class BuffData
{
    [Header("buff施加者设置(%)")]
    public float triggerChance; //buff触发几率（每次攻击有多大几率会触发）

    [Header("基础设置")]
    public BuffType buffType; //buff类型
    public string buffName; //buff名字
    public bool isTriggerOverTime; //是否持续触发
    public bool isStack; //buff是否可以堆叠
    public float duration; //buff持续时间
    public float triggerInterval; //buff触发间隔

    [Header("影响参数")]
    public int damage = 0; //造成伤害 （灼烧buff）
    public float speedMultiplier = 1; //敌人速度倍率 （减速buff，眩晕buff）
    public float woundMultiplier = 1; //敌人受伤倍率 （标记buff，护甲buff）

    [Header("属性倍率计算")]
    public float damageMultiplier = 1; //伤害倍率

    public BuffData(BuffData other)
    {
        triggerChance = other.triggerChance;
        buffType = other.buffType;
        buffName = other.buffName;
        isTriggerOverTime = other.isTriggerOverTime;
        isStack = other.isStack;
        duration = other.duration;
        triggerInterval = other.triggerInterval;
        damage = other.damage;
        woundMultiplier = other.woundMultiplier;
        speedMultiplier = other.speedMultiplier;
        damageMultiplier = other.damageMultiplier;
    }
}
