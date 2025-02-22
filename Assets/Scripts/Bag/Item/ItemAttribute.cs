using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品属性（要素：属性类型、激活条件、激活效果）
/// </summary>
[Serializable]
public class ItemAttribute
{
    /// <summary>
    /// 属性类型枚举
    /// </summary>
    public enum AttributeType
    {
        Global, //全局属性
        Link, //联动属性
    }

    [Header("属性类型")]
    public AttributeType attributeType;

    [Header("激活的要求")]
    public ItemActiveCondition condition;

    [Header("属性激活效果")]
    public ItemActiveEffect[] activeEffects;

    [Header("属性说明")]
    public string description;

    /// <summary>
    /// 检测其他物品是否满足激活条件
    /// </summary>
    /// <param name="item">要检测的物品</param>
    /// <returns>是否满足</returns>
    public bool IsMatch(Item item)
    {
        if (condition.conditionType == ItemActiveCondition.ConditionType.Name)
        {
            if (item.data.itemName != condition.name) return false;
        }
        else
        {
            foreach (ItemTag tag in condition.tags)
                if (!item.data.itemTags.Contains(tag)) return false;
        }

        return true;
    }
}

/// <summary>
/// 物品属性激活效果
/// </summary>
[Serializable]
public class ItemActiveEffect
{
    /// <summary>
    /// 效果类型枚举
    /// </summary>
    public enum EffectType
    {
        Hp, //血量
        Cost, //花费
        Damage, //攻击伤害
        Range, //攻击范围
        Interval, //攻击间隔
        Output, //产量
        Cooldown, //生产冷却
    }

    public EffectType effectType; //效果类型
    public float value; //值
}

/// <summary>
/// 物品激活条件
/// </summary>
[Serializable]
public class ItemActiveCondition
{
    /// <summary>
    /// 激活条件类型
    /// </summary>
    public enum ConditionType
    {
        Tag, //标签
        Name, //名字
    }

    [Header("激活条件类型")]
    public ConditionType conditionType;
    [Header("条件")]
    public string name;
    public ItemTag[] tags;
}
