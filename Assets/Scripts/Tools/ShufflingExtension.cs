using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 地图生成用到的工具
/// 此处为别人的代码，不是我写的！！！
/// </summary>
public static class ShufflingExtension
{
    // got it here: http://stackoverflow.com/questions/273313/randomize-a-listt/1262619#1262619 
    private static System.Random rng = new System.Random();

    /// <summary>
    /// 对列表进行随机打乱
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// 获取列表中的随机一个数
    /// </summary>
    public static T Random<T>(this IList<T> list)
    {
        return list[rng.Next(list.Count)];
    }
}
