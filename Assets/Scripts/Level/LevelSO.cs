using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// һ���ؿ�����Ϣ
/// </summary>
[CreateAssetMenu(fileName = "LevelSO", menuName = "ScriptableObject/LevelSO")]
public class LevelSO : ScriptableObject
{
    [Header("�ȼ�")]
    public int level; //�ùؿ��ĵȼ�
    [Header("����")]
    public List<WaveInfo> waveInfos; //�洢����

    public List<RewardData> rewardDatas; //ʤ������

    /// <summary>
    /// ��ȡ������Ϣ
    /// </summary>
    /// <param name="waveNum">����</param>
    /// <returns>��Ϣ</returns>
    public WaveInfo GetWaveInfo(int waveNum)
    {
        if (waveNum < 1 || waveNum > waveInfos.Count) return null;
        return waveInfos[waveNum-1];
    }
}

/// <summary>
/// һ������Ϣ
/// </summary>
[Serializable]
public class WaveInfo
{
    [Header("������Ϣ")]
    public List<SpawnInfo> spawnInfos; //�洢������Ϣ
}

/// <summary>
/// ����һ�ε��˵���Ϣ
/// </summary>
[Serializable]
public class SpawnInfo
{
    [Header("��������")]
    public string enemyName;
    [Header("������")]
    public int totalNum; //������
    [Header("һ����������")]
    public int spawnNum; //һ�����ɵ�����
    [Header("����Ƶ��")]
    public float frequency; //����Ƶ�ʣ����һ�Σ�
    [Header("��ʱ")]
    public float delayTime; //�Ⲩ��ʼ����ʱ��òſ�ʼ����

    /// <summary>
    /// ��ȡ�Ӳ�����ʼ����ȫ����������Ҫ��ʱ��
    /// </summary>
    /// <returns></returns>
    public float GetTotalTime()
    {
        int spawnTimes = totalNum / spawnNum; //���ֵ�ʱ����������ִ���-1��
        return spawnTimes * frequency + delayTime;
    }
}
