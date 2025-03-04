using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 获取两个Float值中间的值的数据结构
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
