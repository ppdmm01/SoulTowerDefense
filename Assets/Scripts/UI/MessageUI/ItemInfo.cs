using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// 物品属性信息
/// </summary>
public class ItemInfo : MonoBehaviour
{
    public TextMeshProUGUI itemName; //物品名字
    private Dictionary<string, AttributeInfo> itemAttributes = new Dictionary<string, AttributeInfo>(); //物品属性信息
    public float nowHeight; //记录当前信息高度

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="data">防御塔数据</param>
    public void SetInfo(ItemSO data,List<ItemAttribute> nowAttributes)
    {
        nowHeight = itemName.rectTransform.sizeDelta.y;
        itemName.text = data.itemChineseName;
        //类型
        string typeInfo = $"<color={Defines.blueColor}>物品类型：</color>";
        if (data.itemTags.Contains(ItemTag.Tower)) typeInfo += ColorTextTools.ColorText("【防御塔】", "#FF3030");
        else typeInfo += ColorTextTools.ColorText("【道具】", "#00B6FF");
        CreateAttributeInfo("Type", typeInfo);
        //塔
        if (data.itemTags.Contains(ItemTag.Tower))
        {
            string towerName = TowerManager.Instance.GetTowerSO_ByName(data.itemName).towerChineseName;
            string info =
                $"放置在背包中：可使用{ColorTextTools.ColorTextWithBrackets(towerName, "red")}\n" +
                $"若重复放置：属性不叠加，但会参与触发联动效果";
            CreateAttributeInfo("TowerItem", info);
        }
        //物品属性
        if (data.itemAttributes.Count > 0)
        {
            string itemAttributeInfo = $"<color={Defines.blueColor}>基础属性：</color>\n";
            string itemAttributeInfo2 = $"<color={Defines.blueColor}>联动属性：</color>\n";
            //获取属性的描述
            if (nowAttributes == null) nowAttributes = data.itemAttributes;
            foreach (ItemAttribute attribute in nowAttributes)
            {
                if (attribute.attributeType == ItemAttribute.AttributeType.Global)
                    itemAttributeInfo += GetAttributeDescription(attribute) + "\n";
                else
                    itemAttributeInfo2 += GetAttributeDescription(attribute) + "\n";
            }
            //判空
            if (itemAttributeInfo != $"<color={Defines.blueColor}>基础属性：</color>\n")
                CreateAttributeInfo("GlobalAttribute", itemAttributeInfo);
            if (itemAttributeInfo2 != $"<color={Defines.blueColor}>联动属性：</color>\n")
                CreateAttributeInfo("LinkAttribute", itemAttributeInfo2);
        }
        //标签
        string tagInfo = $"<color={Defines.blueColor}>标签：</color>";
        foreach (ItemTag tag in data.itemTags)
        {
            switch (tag)
            {
                case ItemTag.Force:
                    tagInfo += "【力】";
                    break;
                case ItemTag.Heat:
                    tagInfo += "【热】";
                    break;
                case ItemTag.Light:
                    tagInfo += "【光】";
                    break;
                case ItemTag.Sound:
                    tagInfo += "【声】";
                    break;
                case ItemTag.Book:
                    tagInfo += "【书】";
                    break;
                case ItemTag.Tower:
                    tagInfo += "【塔】";
                    break;
            }
        }
        CreateAttributeInfo("itemTags", tagInfo);
        //更新背景高度
        (transform as RectTransform).sizeDelta = new Vector2((transform as RectTransform).sizeDelta.x, nowHeight + 50);
    }

    /// <summary>
    /// 创建属性
    /// </summary>
    /// <param name="info">属性信息</param>
    private void CreateAttributeInfo(string name, string info)
    {
        AttributeInfo attributeInfo;
        if (!itemAttributes.ContainsKey(name))
        {
            //创建属性对象并设置层级
            GameObject attributeObj = Instantiate(Resources.Load<GameObject>("UI/UIObj/AttributeInfo"));
            attributeObj.transform.SetParent(transform, false);
            //设置位置
            RectTransform attributeTrans = attributeObj.transform as RectTransform;
            attributeTrans.anchoredPosition = new Vector2(attributeTrans.anchoredPosition.x, -nowHeight);
            //获取属性信息条
            attributeInfo = attributeObj.GetComponent<AttributeInfo>();
            //记录
            itemAttributes.Add(name, attributeInfo);
        }
        else
        {
            //获取属性信息条
            attributeInfo = itemAttributes[name];
        }
        //设置信息
        attributeInfo.SetInfo(info);
        //计算高度
        nowHeight += attributeInfo.GetHeight();
    }

    /// <summary>
    /// 移除所有属性
    /// </summary>
    public void RemoveAllAttributeInfo()
    {
        nowHeight = itemName.rectTransform.sizeDelta.y;
        foreach (var attributeInfo in itemAttributes.Values)
        {
            Destroy(attributeInfo.gameObject);
        }
        itemAttributes.Clear();
    }

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
                        description += "星星触发的";
                        break;
                    case DetectionPoint.PointType.Fire:
                        description += "火焰触发的";
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
                }
                break;
            case ItemActiveCondition.ConditionType.Name:
                description += condition.name + "：";
                break;
        }
        //触发效果
        string growStr = ""; //记录成长效果
        for (int i = 0;i < attribute.activeEffects.Count();i++)
        {
            ItemActiveEffect effect = attribute.activeEffects[i];
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
                case ItemActiveEffect.EffectType.BurnBuff_Duration:
                    description += $"<color={Defines.redColor}>「灼烧」</color>" +
                        $"<color={Defines.purpleColor}>持续时间+{effect.value.ToString("F1")}s</color>";
                    growStr += $"<color={Defines.redColor}>「灼烧」</color>" +
                        $"<color={Defines.purpleColor}>持续时间+{effect.growValue.ToString("F1")}s</color>";
                    break;
                case ItemActiveEffect.EffectType.BurnBuff_Damage:
                    description += $"<color={Defines.redColor}>「灼烧」</color>" +
                        $"<color={Defines.purpleColor}>伤害+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.redColor}>「灼烧」</color>" +
                        $"<color={Defines.purpleColor}>伤害+{Mathf.RoundToInt(effect.growValue * 100)}%</color>";
                    break;
                case ItemActiveEffect.EffectType.BurnBuff_TriggerChance:
                    description += $"<color={Defines.redColor}>「灼烧」</color>" +
                        $"<color={Defines.purpleColor}>触发几率+{effect.value.ToString("F1")}%</color>";
                    growStr += $"<color={Defines.redColor}>「灼烧」</color>" +
                        $"<color={Defines.purpleColor}>触发几率+{effect.growValue.ToString("F1")}%</color>";
                    break;
                case ItemActiveEffect.EffectType.SlowBuff_Duration:
                    description += $"<color={Defines.greenColor}>「缓慢」</color>" +
                        $"<color={Defines.purpleColor}>持续时间+{effect.value.ToString("F1")}s</color>";
                    growStr += $"<color={Defines.greenColor}>「缓慢」</color>" +
                        $"<color={Defines.purpleColor}>持续时间+{effect.growValue.ToString("F1")}s</color>";
                    break;
                case ItemActiveEffect.EffectType.SlowBuff_TriggerChance:
                    description += $"<color={Defines.greenColor}>「缓慢」</color>" +
                        $"<color={Defines.purpleColor}>触发几率+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.greenColor}>「缓慢」</color>" +
                        $"<color={Defines.purpleColor}>触发几率+{Mathf.RoundToInt(effect.growValue * 100)}%</color>";
                    break;
                default:
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
                            $"<color={Defines.purpleColor}> <已成长{attribute.growTime}次></color>";
                        break;
                    case ItemAttribute.GrowType.Random:
                        description += $"\n(每经过一个地图节点，随机增加下列其一效果：{growStr})" +
                            $"<color={Defines.purpleColor}> <已成长{attribute.growTime}次></color>";
                        break;
                    default:
                        break;
                }
            }
        }
        return description;
    }
}
