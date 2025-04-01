using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 属性管理器（存储一些全局的属性或效果）
/// </summary>
public class AttributeManager : Singleton<AttributeManager>
{
    private AttributeManager() 
    {
        growSpeed = new Dictionary<string, int>();
    }

    private Dictionary<string,int> growSpeed; //存储已激活的影响所有指定物品的效果

    /// <summary>
    /// 获取激活效果的描述
    /// </summary>
    public string GetAttributeDescription(ItemAttribute attribute)
    {
        string description = "";
        ItemActiveCondition condition = attribute.condition;
        //触发规则
        switch (attribute.attributeType)
        {
            case ItemAttribute.AttributeType.Global:
                description += "所有";
                break;
            case ItemAttribute.AttributeType.Link:
                switch (condition.pointType)
                {
                    case DetectionPoint.PointType.Star:
                        description += "<sprite=10>触发的";
                        break;
                    case DetectionPoint.PointType.Fire:
                        description += "<sprite=7>触发的";
                        break;
                }
                break;
        }
        //触发的标签或名字
        ItemTag[] tags = condition.tags;
        switch (condition.conditionType)
        {
            case ItemActiveCondition.ConditionType.Tag:
                //目前只考虑防御塔效果
                if (tags.Contains(ItemTag.Tower))
                {
                    if (tags.Contains(ItemTag.Force))
                    {
                        description += "力学塔：";
                    }
                    else if (tags.Contains(ItemTag.Heat))
                    {
                        description += "热学塔：";
                    }
                    else if (tags.Contains(ItemTag.Light))
                    {
                        description += "光学塔：";
                    }
                    else
                    {
                        description += "塔：";
                    }
                }
                break;
            case ItemActiveCondition.ConditionType.Name:
                description += condition.chineseName + "：";
                break;
        }
        //触发效果
        string growStr = ""; //记录成长效果
        for (int i = 0; i < attribute.activeEffects.Count(); i++)
        {
            ItemActiveEffect effect = attribute.activeEffects[i];
            switch (effect.BuffType)
            {
                case BuffType.None:
                    break;
                case BuffType.Burn:
                    //description += $"<color={Defines.redColor}>「灼烧」</color>";
                    description += "<sprite=1>";
                    growStr += "<sprite=1>";
                    //growStr += $"<color={Defines.redColor}>「灼烧」</color>";
                    break;
                case BuffType.Slow:
                    //description += $"<color={Defines.cyanColor}>「缓慢」</color>";
                    //growStr += $"<color={Defines.cyanColor}>「缓慢」</color>";
                    description += "<sprite=4>";
                    growStr += "<sprite=4>";
                    break;
                case BuffType.Stun:
                    //description += $"<color={Defines.greenColor}>「音震」</color>";
                    //growStr += $"<color={Defines.greenColor}>「音震」</color>";
                    description += "<sprite=6>";
                    growStr += "<sprite=6>";
                    break;
                case BuffType.Mark:
                    //description += $"<color={Defines.grayColor}>「印记」</color>";
                    //growStr += $"<color={Defines.grayColor}>「印记」</color>";
                    description += "<sprite=9>";
                    growStr += "<sprite=9>";
                    break;
            }
            switch (effect.effectType)
            {
                case ItemActiveEffect.EffectType.Hp:
                    description += $"<color={Defines.purpleColor}>血量+{Mathf.RoundToInt(effect.value)}</color>";
                    growStr += $"<color={Defines.purpleColor}>血量+{Mathf.RoundToInt(effect.growValue)}</color>";
                    break;
                case ItemActiveEffect.EffectType.Cost:
                    description += $"<color={Defines.purpleColor}>花费-{Mathf.RoundToInt(effect.value)}</color>";
                    growStr += $"<color={Defines.purpleColor}>花费-{Mathf.RoundToInt(effect.growValue)}</color>";
                    break;
                case ItemActiveEffect.EffectType.Output:
                    description += $"<color={Defines.purpleColor}>产量+{Mathf.RoundToInt(effect.value)}</color>";
                    growStr += $"<color={Defines.purpleColor}>产量+{Mathf.RoundToInt(effect.growValue)}</color>";
                    break;
                case ItemActiveEffect.EffectType.Cooldown:
                    description += $"<color={Defines.purpleColor}>生产冷却-{Mathf.RoundToInt(effect.value)}s</color>";
                    growStr += $"<color={Defines.purpleColor}>生产冷却-{Mathf.RoundToInt(effect.growValue)}s</color>";
                    break;
                case ItemActiveEffect.EffectType.DamageMultiplier:
                    description += $"<color={Defines.purpleColor}>物理伤害+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.purpleColor}>物理伤害+{Mathf.RoundToInt(effect.growValue * 100)}%</color>";
                    break;
                case ItemActiveEffect.EffectType.RangeMultiplier:
                    description += $"<color={Defines.purpleColor}>射程+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.purpleColor}>射程+{Mathf.RoundToInt(effect.growValue * 100)}%</color>";
                    break;
                case ItemActiveEffect.EffectType.IntervalMultiplier:
                    description += $"<color={Defines.purpleColor}>攻击冷却-{Mathf.Abs(Mathf.RoundToInt(effect.value * 100))}%</color>";
                    growStr += $"<color={Defines.purpleColor}>攻击冷却-{Mathf.Abs(Mathf.RoundToInt(effect.growValue * 100))}%</color>";
                    break;
                case ItemActiveEffect.EffectType.Buff_Duration:
                    description += $"<color={Defines.purpleColor}>持续时间+{effect.value.ToString("F1")}s</color>";
                    growStr += $"<color={Defines.purpleColor}>持续时间+{effect.growValue.ToString("F1")}s</color>";
                    break;
                case ItemActiveEffect.EffectType.Buff_TriggerChance:
                    description += $"<color={Defines.purpleColor}>触发几率+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.purpleColor}>触发几率+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    break;
                case ItemActiveEffect.EffectType.Buff_Damage:
                    description += $"<color={Defines.purpleColor}>伤害+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.purpleColor}>伤害+{Mathf.RoundToInt(effect.growValue * 100)}%</color>";
                    break;
                case ItemActiveEffect.EffectType.Buff_WoundMultiplier:
                    description += $"<color={Defines.purpleColor}>伤害倍率+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.purpleColor}>伤害倍率+{Mathf.RoundToInt(effect.growValue * 100)}%</color>";
                    break;
                case ItemActiveEffect.EffectType.GrowSpeed:
                    description += $"<color={Defines.purpleColor}>成长速度为原来的{Mathf.RoundToInt(effect.value)}倍</color>";
                    growStr += $"<color={Defines.purpleColor}>成长速度为原来的{Mathf.RoundToInt(effect.growValue)}倍</color>";
                    break;
            }
            if (i != attribute.activeEffects.Count() - 1)
            {
                description += "，";
                growStr += "，";
            }
            if (i == attribute.activeEffects.Count() - 1 && attribute.isGrow)
            {
                switch (attribute.growType)
                {
                    case ItemAttribute.GrowType.All:
                        description += $"\n(每经过一个地图节点：{growStr})" +
                            $"\n<已成长{attribute.growTime}次>";
                        break;
                    case ItemAttribute.GrowType.Random:
                        description += $"\n(每经过一个地图节点，随机增加下列其一效果：{growStr})" +
                           $"\n<已成长{attribute.growTime}次>";
                        break;
                    default:
                        break;
                }
            }
        }
        return description;
    }

    /// <summary>
    /// 实现属性
    /// </summary>
    /// <param name="itemName">物品名字</param>
    /// <param name="activeEffect">激活效果</param>
    public void SetAttributeFromName(string itemName, ItemActiveEffect[] activeEffects)
    {
        TowerData data = null;
        if (TowerManager.Instance.towerDatas.ContainsKey(itemName))
            data = TowerManager.Instance.towerDatas[itemName];

        BuffData buffData = null;
        foreach (ItemActiveEffect activeEffect in activeEffects)
        {
            if (data != null) buffData = data.GetBuffData(activeEffect.BuffType);
            switch (activeEffect.effectType)
            {
                case ItemActiveEffect.EffectType.Hp:
                    if (data != null)
                        data.hp += Mathf.RoundToInt(activeEffect.value);
                    break;
                case ItemActiveEffect.EffectType.Cost:
                    if (data != null)
                        data.cost += Mathf.RoundToInt(activeEffect.value);
                    break;
                case ItemActiveEffect.EffectType.Output:
                    if (data != null)
                        data.output += Mathf.RoundToInt(activeEffect.value);
                    break;
                case ItemActiveEffect.EffectType.Cooldown:
                    if (data != null)
                        data.cooldown += activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.DamageMultiplier:
                    if (data != null)
                    {
                        data.damageMultiplier += activeEffect.value;
                        data.UpdateAttribute();
                    }
                    break;
                case ItemActiveEffect.EffectType.RangeMultiplier:
                    if (data != null)
                    {
                        data.rangeMultiplier += activeEffect.value;
                        data.UpdateAttribute();
                    }
                    break;
                case ItemActiveEffect.EffectType.IntervalMultiplier:
                    if (data != null)
                    {
                        data.intervalMultiplier += activeEffect.value;
                        data.intervalMultiplier = Mathf.Clamp(data.intervalMultiplier,0.2f,100); //限制范围
                        data.UpdateAttribute();
                    }
                    break;
                case ItemActiveEffect.EffectType.Buff_Duration:
                    if (buffData != null) buffData.duration += activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.Buff_TriggerChance:
                    if (buffData != null) buffData.triggerChance += activeEffect.value;
                    data.UpdateAttribute();
                    break;
                case ItemActiveEffect.EffectType.Buff_Damage:
                    if (buffData != null) buffData.damageMultiplier += activeEffect.value;
                    data.UpdateAttribute();
                    break;
                case ItemActiveEffect.EffectType.Buff_WoundMultiplier:
                    if (buffData != null) buffData.woundMultiplier += activeEffect.value;
                    break;
                case ItemActiveEffect.EffectType.GrowSpeed:
                    //让所有指定物品的成长速度变为当前值
                    int speed = (int)activeEffect.value;
                    GridManager.Instance.SetItemGrowSpeed(itemName, speed);
                    if (!growSpeed.ContainsKey(itemName))
                        growSpeed.Add(itemName, speed);
                    break;
            }
        }
    }

    /// <summary>
    /// 通过标签实现属性
    /// </summary>
    /// <param name="tags">标签</param>
    /// <param name="activeEffects">激活效果</param>
    public void SetAttributeFromTag(ItemTag[] tags, ItemActiveEffect[] activeEffects)
    {
        bool flag = true; //标记是否满足标签条件
        foreach (TowerData data in TowerManager.Instance.towerDatas.Values)
        {
            flag = true;
            //只有所有标签满足并且检测点类型对应才行
            foreach (ItemTag tag in tags)
                if (!data.itemTags.Contains(tag))
                    flag = false;

            if (flag)
                SetAttributeFromName(data.towerName, activeEffects);
        }
    }


    /// <summary>
    /// 获取对应效果
    /// </summary>
    /// <returns></returns>
    public int GetItemGrowSpeed(string itemName)
    {
        if (growSpeed.ContainsKey(itemName))
            return growSpeed[itemName];
        return 1;
    }

    /// <summary>
    /// 清理
    /// </summary>
    public void ClearAllGrowSpeed()
    {
        foreach (string itemName in growSpeed.Keys)
        {
            GridManager.Instance.SetItemGrowSpeed(itemName, 1); //所有物品回到速度1
        }
        growSpeed.Clear();
    }
}
