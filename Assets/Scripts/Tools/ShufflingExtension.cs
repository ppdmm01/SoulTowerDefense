using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ��ͼ�����õ��Ĺ���
/// �˴�Ϊ���˵Ĵ��룬������д�ģ�����
/// </summary>
public static class ShufflingExtension
{
    // got it here: http://stackoverflow.com/questions/273313/randomize-a-listt/1262619#1262619 
    private static System.Random rng = new System.Random();

    /// <summary>
    /// ���б�����������
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
    /// ��ȡ�б��е����һ����
    /// </summary>
    public static T Random<T>(this IList<T> list)
    {
        return list[rng.Next(list.Count)];
    }
}
