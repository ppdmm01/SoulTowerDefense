using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RewardType
{
    Taixu, //̫����Դ
    Item, //����ѡ��
}

[Serializable]
public class RewardData
{
    public RewardType rewardType;
    [Header("��Դ��")]
    public int value;
}
