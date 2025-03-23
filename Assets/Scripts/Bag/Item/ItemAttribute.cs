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
    /// ��������ö��
    /// </summary>
    public enum AttributeType
    {
        Global, //ȫ������
        Link, //��������
    }

    public enum GrowType
    {
        All, //ȫ������һ��ɳ�
        Random, //���һ�����Գɳ�
    }

    [Header("��������")]
    public AttributeType attributeType;

    [Header("�����Ҫ��")]
    public ItemActiveCondition condition;

    [Header("�ɳ�")]
    public bool isGrow = false;   
    public GrowType growType;
    [HideInInspector] public int growTime; //�ɳ�����

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

    /// <summary>
    ///  ��Ʒ�ɳ�
    /// </summary>
    /// <param name="count">�ɳ��غ�</param>
    /// <returns>�ɳ��������ֵ</returns>
    public void Grow(int count)
    {
        if (!isGrow) return;
        growTime += count;
        switch (growType)
        {
            case GrowType.Random:
                ItemActiveEffect growEffect = activeEffects.Random(); //���һ����Ҫ�ɳ�������
                growEffect.Grow(count);
                break;
            case GrowType.All:
                foreach (ItemActiveEffect effect in activeEffects)
                    effect.Grow(count);
                break;
        }
    }

    //��ȡ�����Ч��
    public ItemActiveEffect[] GetActiveEffects()
    {
        ItemActiveEffect[] list = new ItemActiveEffect[activeEffects.Length];
        for (int i = 0;i <activeEffects.Length;i++)
            list[i] = new ItemActiveEffect(activeEffects[i]);
        return list;
    }
}

/// <summary>
/// ��Ʒ���Լ���Ч��
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
        SlowBuff_Duration, //����ʱ��
        SlowBuff_TriggerChance, //��������
    }


    public EffectType effectType; //Ч������
    public float value; //ֵ
    [Header("�ɳ�")]
    public float growValue; //�ɳ�ֵ

    /// <summary>
    /// �ɳ�����ȡ�ɳ��������ֵ
    /// </summary>
    /// <param name="count">�ɳ��غ�</param>
    public void Grow(int count)
    {
        value += count * growValue;
    }
}

/// <summary>
/// ��Ʒ��������
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