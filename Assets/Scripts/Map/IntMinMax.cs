using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 获取两个Int值中间的值的数据结构
/// </summary>
[System.Serializable]
public class IntMinMax
{
    public int min; //最大值
    public int max; //最小值

    //获取最大最小值间的随机值
    public int GetValue()
    {
        return Random.Range(min, max + 1);
    }
}
