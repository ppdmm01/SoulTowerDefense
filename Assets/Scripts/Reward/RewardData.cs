using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RewardType
{
    Taixu, //太虚资源
    Item, //道具选择
}

[Serializable]
public class RewardData
{
    public RewardType rewardType;
    [Header("资源量")]
    public int value;
}
