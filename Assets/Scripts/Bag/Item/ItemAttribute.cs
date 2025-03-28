using System;
using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    /// 检测其他物品是否满足激活条件
    /// </summary>
    /// <param name="item">要检测的物品</param>
    /// <param name="pointType">检测点类型</param>
    /// <returns>是否满足</returns>
    public bool IsMatch(Item item)
    {
        //判断物品的名字或标签
        if (condition.conditionType == ItemActiveCondition.ConditionType.Name)
        {
            if (item.data.itemChineseName != condition.name) return false;
        }
        else
        {
            foreach (ItemTag tag in condition.tags)
                if (!item.data.itemTags.Contains(tag)) return false;
        }

        /*
         * 判断是否满足buff
         * activeEffects的buff类型分3种情况：全是None，Buff和None参杂在一起，全是Buff
         * 对于没有Buff的道具来说，在标签或名字匹配后，只要要求里有None即可激活该效果
         * 对于有Buff的道具来说，在标签或名字匹配后，只要要求里有None或者有匹配的Buff即可激活该效果
         */
        List<BuffType> buffTypeList = activeEffects.Select(effect => effect.BuffType).Distinct().ToList(); //获取效果所需buff类型
        if (item.nowItemBuffs.Count == 0)
        {
            if (buffTypeList.Contains(BuffType.None))
            {
                return true;
            }
        }
        else
        {
            foreach (BuffType buffType in buffTypeList)
            {
                if (buffType == BuffType.None || item.nowItemBuffs.Any(b => b == buffType)) return true;
            }
        }
        return false;
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
        this.BuffType = other.BuffType;
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
        Buff_Duration, //buff持续时间
        Buff_TriggerChance, //buff触发几率
        Buff_Damage, //buff伤害
        Buff_WoundMultiplier, //buff受伤倍率
        [Header("成长相关")]
        GrowSpeed //成长速度
    }


    public EffectType effectType; //效果类型
    public BuffType BuffType; //buff类型
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
    public string chineseName;
    public ItemTag[] tags;
}