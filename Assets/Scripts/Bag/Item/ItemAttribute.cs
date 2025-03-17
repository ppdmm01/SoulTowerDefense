using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ʒ���ԣ�Ҫ�أ��������͡���������������Ч����
/// </summary>
[Serializable]
public class ItemAttribute
{
    /// <summary>
    /// ��������ö��
    /// </summary>
    public enum AttributeType
    {
        Global, //ȫ������
        Link, //��������
    }

    [Header("��������")]
    public AttributeType attributeType;

    [Header("�����Ҫ��")]
    public ItemActiveCondition condition;

    [Header("���Լ���Ч��")]
    public ItemActiveEffect[] activeEffects;

    [Header("����˵��")]
    public string description;

    /// <summary>
    /// ���������Ʒ�Ƿ����㼤������
    /// </summary>
    /// <param name="item">Ҫ������Ʒ</param>
    /// <param name="pointType">��������</param>
    /// <returns>�Ƿ�����</returns>
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
/// ��Ʒ���Լ���Ч��
/// </summary>
[Serializable]
public class ItemActiveEffect
{
    /// <summary>
    /// Ч������ö��
    /// </summary>
    public enum EffectType
    {
        [Header("����������")]
        Hp, //Ѫ��
        Cost, //����
        Output, //����
        Cooldown, //������ȴ

        DamageMultiplier, //��������
        RangeMultiplier, //��̱���
        IntervalMultiplier, //�����������
        [Header("Buff���")]
        BurnBuff_Duration, //����ʱ��
        BurnBuff_Damage, //�����˺�
        BurnBuff_TriggerChance, //���ռ���
    }

    public EffectType effectType; //Ч������
    public float value; //ֵ
}

/// <summary>
/// ��Ʒ��������
/// </summary>
[Serializable]
public class ItemActiveCondition
{
    /// <summary>
    /// ������������
    /// </summary>
    public enum ConditionType
    {
        Tag, //��ǩ
        Name, //����
    }

    [Header("������������")]
    public ConditionType conditionType;
    [Header("����ļ�������")]
    public DetectionPoint.PointType pointType;
    [Header("����")]
    public string name;
    public ItemTag[] tags;
}