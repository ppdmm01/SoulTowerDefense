using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ȡ����Floatֵ�м��ֵ�����ݽṹ
/// </summary>
[System.Serializable]
public class FloatMinMax
{
    public float min;
    public float max;

    public float GetValue()
    {
        return Random.Range(min, max);
    }
}
