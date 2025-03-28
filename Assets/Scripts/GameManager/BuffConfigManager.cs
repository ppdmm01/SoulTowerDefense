using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffConfigManager : Singleton<BuffConfigManager>
{
    private BuffConfigSO data;

    private BuffConfigManager()
    {
        if (data == null)
        {
            data = Resources.Load<BuffConfigSO>("Data/BuffConfigSO");
            if (data == null)
                Debug.LogError("����BuffConfigSOʧ�ܣ�");
        }
    }

    /// <summary>
    /// ��ȡbuff��������
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public BuffData GetBuffConfigData(BuffType type)
    {
        if (type == BuffType.None) return null;
        else return new BuffData(data.BuffDataList.FirstOrDefault(buffData => buffData.buffType == type));
    }
}
