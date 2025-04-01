using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RewardType
{
    Taixu, //̫����Դ
    Item, //����ѡ��
    Tower, //����������
}

[Serializable]
public class RewardData
{
    public RewardType rewardType;
    [Header("��Դ��")]
    public int minValue;
    public int maxValue;
    public RewardData(RewardType rewardType, int minValue, int maxValue)
    {
        this.rewardType = rewardType;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}
