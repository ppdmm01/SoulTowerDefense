using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ȡ����Intֵ�м��ֵ�����ݽṹ
/// </summary>
[System.Serializable]
public class IntMinMax
{
    public int min; //���ֵ
    public int max; //��Сֵ

    //��ȡ�����Сֵ������ֵ
    public int GetValue()
    {
        return Random.Range(min, max + 1);
    }
}
