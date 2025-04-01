using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RewardType
{
    Taixu, //太虚资源
    Item, //道具选择
    Tower, //防御塔奖励
}

[Serializable]
public class RewardData
{
    public RewardType rewardType;
    [Header("资源量")]
    public int minValue;
    public int maxValue;
    public RewardData(RewardType rewardType, int minValue, int maxValue)
    {
        this.rewardType = rewardType;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}
