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
    public ItemAttribute() 
    {
        growTime = 0;
    }
    public ItemAttribute(ItemAttribute other)
    {
        this.attributeType = other.attributeType;
        this.condition = new ItemActiveCondition(other.condition);
        this.isGrow = other.isGrow;
        this.growType = other.growType;
        this.growTime = other.growTime;
        this.activeEffects = other.GetActiveEffects();
        this.description = other.description;
    }
    /// <summary>
    /// 属性类型枚举
    /// </summary>
    public enum AttributeType
    {
        Global, //全局属性
        Link, //联动属性
    }

    public enum GrowType
    {
        All, //全部属性一起成长
        Random, //随机一个属性成长
    }

    [Header("属性类型")]
    public AttributeType attributeType;

    [Header("激活的要求")]
    public ItemActiveCondition condition;

    [Header("成长")]
    public bool isGrow = false;   
    public GrowType growType;
    [HideInInspector] public int growTime; //成长次数

    [Header("属性激活效果")]
    public ItemActiveEffect[] activeEffects;

    [Header("属性说明")]
    public string description;

    /// <summary>
    /// 检测其他物品是否满足激活条件
    /// </summary>
    /// <param name="item">要检测的物品</param>
    /// <param name="pointType">检测点类型</param>
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

    /// <summary>
    ///  物品成长
    /// </summary>
    /// <param name="count">成长回合</param>
    /// <returns>成长后的属性值</returns>
    public void Grow(int count)
    {
        if (!isGrow) return;
        growTime += count;
        switch (growType)
        {
            case GrowType.Random:
                ItemActiveEffect growEffect = activeEffects.Random(); //随机一个需要成长的属性
                growEffect.Grow(count);
                break;
            case GrowType.All:
                foreach (ItemActiveEffect effect in activeEffects)
                    effect.Grow(count);
                break;
        }
    }

    //获取激活的效果
    public ItemActiveEffect[] GetActiveEffects()
    {
        ItemActiveEffect[] list = new ItemActiveEffect[activeEffects.Length];
        for (int i = 0;i <activeEffects.Length;i++)
            list[i] = new ItemActiveEffect(activeEffects[i]);
        return list;
    }
}

/// <summary>
/// 物品属性激活效果
/// </summary>
[Serializable]
public class ItemActiveEffect
{
    public ItemActiveEffect() { }   
    public ItemActiveEffect(ItemActiveEffect other)
    {
        this.effectType = other.effectType;
        this.value = other.value;
        this.growValue = other.growValue;
    }
    /// <summary>
    /// 效果类型枚举
    /// </summary>
    public enum EffectType
    {
        [Header("防御塔属性")]
        Hp, //血量
        Cost, //花费
        Output, //产量
        Cooldown, //生产冷却

        DamageMultiplier, //攻击倍率
        RangeMultiplier, //射程倍率
        IntervalMultiplier, //攻击间隔倍率
        [Header("Buff相关")]
        BurnBuff_Duration, //灼烧时间
        BurnBuff_Damage, //灼烧伤害
        BurnBuff_TriggerChance, //灼烧几率
        SlowBuff_Duration, //缓慢时间
        SlowBuff_TriggerChance, //缓慢几率
    }


    public EffectType effectType; //效果类型
    public float value; //值
    [Header("成长")]
    public float growValue; //成长值

    /// <summary>
    /// 成长并获取成长后的属性值
    /// </summary>
    /// <param name="count">成长回合</param>
    public void Grow(int count)
    {
        value += count * growValue;
    }
}

/// <summary>
/// 物品激活条件
/// </summary>
[Serializable]
public class ItemActiveCondition
{
    public ItemActiveCondition() { }
    public ItemActiveCondition(ItemActiveCondition other)
    {
        this.conditionType = other.conditionType;
        this.pointType = other.pointType;
        this.name = other.name;
        this.tags = other.tags;
    }
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
    [Header("激活的检测点类型")]
    public DetectionPoint.PointType pointType;
    [Header("条件")]
    public string name;
    public ItemTag[] tags;
}