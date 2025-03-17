using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    Burn, //����
    Slow, //����
    Stun, //ѣ��
}

/// <summary>
/// Buff����
/// </summary>
[Serializable]
public class BuffData
{
    [Header("buffʩ��������(%)")]
    public float triggerChance; //buff�������ʣ�ÿ�ι����ж���ʻᴥ����

    [Header("��������")]
    public BuffType buffType; //buff����
    public string buffName; //buff����
    public bool isTriggerOverTime; //�Ƿ��������
    public bool isStack; //buff�Ƿ���Զѵ�
    public float duration; //buff����ʱ��
    public float triggerInterval; //buff�������

    [Header("Ӱ�����")]
    public int damage = 0; //����˺� ������buff��
    public float speedMultiplier = 1; //�����ٶȱ��� ������buff��ѣ��buff��
    public float woundMultiplier = 1; //�������˱��� �����buff������buff��

    [Header("���Ա��ʼ���")]
    public float damageMultiplier = 1; //�˺�����

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
