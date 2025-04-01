using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ���Թ��������洢һЩȫ�ֵ����Ի�Ч����
/// </summary>
public class AttributeManager : Singleton<AttributeManager>
{
    private AttributeManager() 
    {
        growSpeed = new Dictionary<string, int>();
    }

    private Dictionary<string,int> growSpeed; //�洢�Ѽ����Ӱ������ָ����Ʒ��Ч��

    /// <summary>
    /// ��ȡ����Ч��������
    /// </summary>
    public string GetAttributeDescription(ItemAttribute attribute)
    {
        string description = "";
        ItemActiveCondition condition = attribute.condition;
        //��������
        switch (attribute.attributeType)
        {
            case ItemAttribute.AttributeType.Global:
                description += "����";
                break;
            case ItemAttribute.AttributeType.Link:
                switch (condition.pointType)
                {
                    case DetectionPoint.PointType.Star:
                        description += "<sprite=10>������";
                        break;
                    case DetectionPoint.PointType.Fire:
                        description += "<sprite=7>������";
                        break;
                }
                break;
        }
        //�����ı�ǩ������
        ItemTag[] tags = condition.tags;
        switch (condition.conditionType)
        {
            case ItemActiveCondition.ConditionType.Tag:
                //Ŀǰֻ���Ƿ�����Ч��
                if (tags.Contains(ItemTag.Tower))
                {
                    if (tags.Contains(ItemTag.Force))
                    {
                        description += "��ѧ����";
                    }
                    else if (tags.Contains(ItemTag.Heat))
                    {
                        description += "��ѧ����";
                    }
                    else if (tags.Contains(ItemTag.Light))
                    {
                        description += "��ѧ����";
                    }
                    else
                    {
                        description += "����";
                    }
                }
                break;
            case ItemActiveCondition.ConditionType.Name:
                description += condition.chineseName + "��";
                break;
        }
        //����Ч��
        string growStr = ""; //��¼�ɳ�Ч��
        for (int i = 0; i < attribute.activeEffects.Count(); i++)
        {
            ItemActiveEffect effect = attribute.activeEffects[i];
            switch (effect.BuffType)
            {
                case BuffType.None:
                    break;
                case BuffType.Burn:
                    //description += $"<color={Defines.redColor}>�����ա�</color>";
                    description += "<sprite=1>";
                    growStr += "<sprite=1>";
                    //growStr += $"<color={Defines.redColor}>�����ա�</color>";
                    break;
                case BuffType.Slow:
                    //description += $"<color={Defines.cyanColor}>��������</color>";
                    //growStr += $"<color={Defines.cyanColor}>��������</color>";
                    description += "<sprite=4>";
                    growStr += "<sprite=4>";
                    break;
                case BuffType.Stun:
                    //description += $"<color={Defines.greenColor}>������</color>";
                    //growStr += $"<color={Defines.greenColor}>������</color>";
                    description += "<sprite=6>";
                    growStr += "<sprite=6>";
                    break;
                case BuffType.Mark:
                    //description += $"<color={Defines.grayColor}>��ӡ�ǡ�</color>";
                    //growStr += $"<color={Defines.grayColor}>��ӡ�ǡ�</color>";
                    description += "<sprite=9>";
                    growStr += "<sprite=9>";
                    break;
            }
            switch (effect.effectType)
            {
                case ItemActiveEffect.EffectType.Hp:
                    description += $"<color={Defines.purpleColor}>Ѫ��+{Mathf.RoundToInt(effect.value)}</color>";
                    growStr += $"<color={Defines.purpleColor}>Ѫ��+{Mathf.RoundToInt(effect.growValue)}</color>";
                    break;
                case ItemActiveEffect.EffectType.Cost:
                    description += $"<color={Defines.purpleColor}>����-{Mathf.RoundToInt(effect.value)}</color>";
                    growStr += $"<color={Defines.purpleColor}>����-{Mathf.RoundToInt(effect.growValue)}</color>";
                    break;
                case ItemActiveEffect.EffectType.Output:
                    description += $"<color={Defines.purpleColor}>����+{Mathf.RoundToInt(effect.value)}</color>";
                    growStr += $"<color={Defines.purpleColor}>����+{Mathf.RoundToInt(effect.growValue)}</color>";
                    break;
                case ItemActiveEffect.EffectType.Cooldown:
                    description += $"<color={Defines.purpleColor}>������ȴ-{Mathf.RoundToInt(effect.value)}s</color>";
                    growStr += $"<color={Defines.purpleColor}>������ȴ-{Mathf.RoundToInt(effect.growValue)}s</color>";
                    break;
                case ItemActiveEffect.EffectType.DamageMultiplier:
                    description += $"<color={Defines.purpleColor}>�����˺�+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.purpleColor}>�����˺�+{Mathf.RoundToInt(effect.growValue * 100)}%</color>";
                    break;
                case ItemActiveEffect.EffectType.RangeMultiplier:
                    description += $"<color={Defines.purpleColor}>���+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.purpleColor}>���+{Mathf.RoundToInt(effect.growValue * 100)}%</color>";
                    break;
                case ItemActiveEffect.EffectType.IntervalMultiplier:
                    description += $"<color={Defines.purpleColor}>������ȴ-{Mathf.Abs(Mathf.RoundToInt(effect.value * 100))}%</color>";
                    growStr += $"<color={Defines.purpleColor}>������ȴ-{Mathf.Abs(Mathf.RoundToInt(effect.growValue * 100))}%</color>";
                    break;
                case ItemActiveEffect.EffectType.Buff_Duration:
                    description += $"<color={Defines.purpleColor}>����ʱ��+{effect.value.ToString("F1")}s</color>";
                    growStr += $"<color={Defines.purpleColor}>����ʱ��+{effect.growValue.ToString("F1")}s</color>";
                    break;
                case ItemActiveEffect.EffectType.Buff_TriggerChance:
                    description += $"<color={Defines.purpleColor}>��������+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.purpleColor}>��������+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    break;
                case ItemActiveEffect.EffectType.Buff_Damage:
                    description += $"<color={Defines.purpleColor}>�˺�+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.purpleColor}>�˺�+{Mathf.RoundToInt(effect.growValue * 100)}%</color>";
                    break;
                case ItemActiveEffect.EffectType.Buff_WoundMultiplier:
                    description += $"<color={Defines.purpleColor}>�˺�����+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.purpleColor}>�˺�����+{Mathf.RoundToInt(effect.growValue * 100)}%</color>";
                    break;
                case ItemActiveEffect.EffectType.GrowSpeed:
                    description += $"<color={Defines.purpleColor}>�ɳ��ٶ�Ϊԭ����{Mathf.RoundToInt(effect.value)}��</color>";
                    growStr += $"<color={Defines.purpleColor}>�ɳ��ٶ�Ϊԭ����{Mathf.RoundToInt(effect.growValue)}��</color>";
                    break;
            }
            if (i != attribute.activeEffects.Count() - 1)
            {
                description += "��";
                growStr += "��";
            }
            if (i == attribute.activeEffects.Count() - 1 && attribute.isGrow)
            {
                switch (attribute.growType)
                {
                    case ItemAttribute.GrowType.All:
                        description += $"\n(ÿ����һ����ͼ�ڵ㣺{growStr})" +
                            $"\n<�ѳɳ�{attribute.growTime}��>";
                        break;
                    case ItemAttribute.GrowType.Random:
                        description += $"\n(ÿ����һ����ͼ�ڵ㣬�������������һЧ����{growStr})" +
                           $"\n<�ѳɳ�{attribute.growTime}��>";
                        break;
                    default:
                        break;
                }
            }
        }
        return description;
    }

    /// <summary>
    /// ʵ������
    /// </summary>
    /// <param name="itemName">��Ʒ����</param>
    /// <param name="activeEffect">����Ч��</param>
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
                        data.intervalMultiplier = Mathf.Clamp(data.intervalMultiplier,0.2f,100); //���Ʒ�Χ
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
                    //������ָ����Ʒ�ĳɳ��ٶȱ�Ϊ��ǰֵ
                    int speed = (int)activeEffect.value;
                    GridManager.Instance.SetItemGrowSpeed(itemName, speed);
                    if (!growSpeed.ContainsKey(itemName))
                        growSpeed.Add(itemName, speed);
                    break;
            }
        }
    }

    /// <summary>
    /// ͨ����ǩʵ������
    /// </summary>
    /// <param name="tags">��ǩ</param>
    /// <param name="activeEffects">����Ч��</param>
    public void SetAttributeFromTag(ItemTag[] tags, ItemActiveEffect[] activeEffects)
    {
        bool flag = true; //����Ƿ������ǩ����
        foreach (TowerData data in TowerManager.Instance.towerDatas.Values)
        {
            flag = true;
            //ֻ�����б�ǩ���㲢�Ҽ������Ͷ�Ӧ����
            foreach (ItemTag tag in tags)
                if (!data.itemTags.Contains(tag))
                    flag = false;

            if (flag)
                SetAttributeFromName(data.towerName, activeEffects);
        }
    }


    /// <summary>
    /// ��ȡ��ӦЧ��
    /// </summary>
    /// <returns></returns>
    public int GetItemGrowSpeed(string itemName)
    {
        if (growSpeed.ContainsKey(itemName))
            return growSpeed[itemName];
        return 1;
    }

    /// <summary>
    /// ����
    /// </summary>
    public void ClearAllGrowSpeed()
    {
        foreach (string itemName in growSpeed.Keys)
        {
            GridManager.Instance.SetItemGrowSpeed(itemName, 1); //������Ʒ�ص��ٶ�1
        }
        growSpeed.Clear();
    }
}
