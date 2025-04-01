using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一个关卡的信息
/// </summary>
[CreateAssetMenu(fileName = "LevelSO", menuName = "ScriptableObject/LevelSO")]
public class LevelSO : ScriptableObject
{
    [Header("等级")]
    public int level; //该关卡的等级
    [Header("波次")]
    public List<WaveInfo> waveInfos; //存储波次

    public List<RewardData> rewardDatas; //胜利奖励

    /// <summary>
    /// 获取波次信息
    /// </summary>
    /// <param name="waveNum">波次</param>
    /// <returns>信息</returns>
    public WaveInfo GetWaveInfo(int waveNum)
    {
        if (waveNum < 1 || waveNum > waveInfos.Count) return null;
        return waveInfos[waveNum-1];
    }
}

/// <summary>
/// 一波的信息
/// </summary>
[Serializable]
public class WaveInfo
{
    [Header("出怪信息")]
    public List<SpawnInfo> spawnInfos; //存储出怪信息
}

/// <summary>
/// 生成一次敌人的信息
/// </summary>
[Serializable]
public class SpawnInfo
{
    [Header("敌人名字")]
    public string enemyName;
    [Header("总数量")]
    public int totalNum; //总数量
    [Header("一次生成数量")]
    public int spawnNum; //一次生成的数量
    [Header("出怪频率")]
    public float frequency; //出怪频率（多久一次）
    [Header("延时")]
    public float delayTime; //这波开始后延时多久才开始出怪

    /// <summary>
    /// 获取从波数开始到怪全部出完所需要的时间
    /// </summary>
    /// <returns></returns>
    public float GetTotalTime()
    {
        int spawnTimes = totalNum / spawnNum; //出怪的时间段数（出怪次数-1）
        return spawnTimes * frequency + delayTime;
    }
}
