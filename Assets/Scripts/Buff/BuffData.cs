using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buff数据
/// </summary>
[Serializable]
public class BuffData
{
    [Header("buff施加者设置(%)")]
    public float triggerChance; //buff触发几率（每次攻击有多大几率会触发）

    [Header("基础设置")]
    public string buffName; //buff名字
    public bool IsTriggerOverTime; //是否持续触发
    public bool isStack; //buff是否可以堆叠
    public float duration; //buff持续时间
    public float triggerInterval; //buff触发间隔

    [Header("影响参数")]
    public int damage = 0; //伤害 （灼烧buff）
    public float speedMultiplier = 1; //速度倍率 （减速buff，眩晕buff）
    public float damageMultiplier = 1; //受伤倍率 （标记buff，护甲buff）
}
