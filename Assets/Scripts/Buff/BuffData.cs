using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buff����
/// </summary>
[Serializable]
public class BuffData
{
    [Header("buffʩ��������(%)")]
    public float triggerChance; //buff�������ʣ�ÿ�ι����ж���ʻᴥ����

    [Header("��������")]
    public string buffName; //buff����
    public bool IsTriggerOverTime; //�Ƿ��������
    public bool isStack; //buff�Ƿ���Զѵ�
    public float duration; //buff����ʱ��
    public float triggerInterval; //buff�������

    [Header("Ӱ�����")]
    public int damage = 0; //�˺� ������buff��
    public float speedMultiplier = 1; //�ٶȱ��� ������buff��ѣ��buff��
    public float damageMultiplier = 1; //���˱��� �����buff������buff��
}
