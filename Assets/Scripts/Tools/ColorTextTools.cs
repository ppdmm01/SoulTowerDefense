using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ�ı�����
/// </summary>
public static class ColorTextTools
{
    /// <summary>
    /// ������ɫ�ı�
    /// </summary>
    /// <param name="info">��Ϣ</param>
    /// <param name="color">��ɫ</param>
    /// <returns>�ı�</returns>
    public static string ColorText(string info,string color)
    {
        return $"<color={color}>{info}</color>";
    }

    /// <summary>
    /// ���ش���������ɫ�ı�
    /// </summary>
    /// <param name="info">��Ϣ</param>
    /// <param name="color">��ɫ</param>
    /// <returns>�ı�</returns>
    public static string ColorTextWithBrackets(string info, string color)
    {
        return $"<color={color}>��{info}��</color>";
    }

    /// <summary>
    /// ͨ��Intֵ������ɫ�������̡������죩
    /// </summary>
    /// <param name="value">ֵ</param>
    /// /// <param name="isReverse">�Ƿ�ת</param>
    /// <returns></returns>
    public static string ColorTextWithInt(int value, bool isReverse = false)
    {
        if (value == 0) return "";
        if (isReverse)
        {
            if (value < 0)
                return ColorText(value.ToString(), "green");
            else
                return ColorText($"+{value}", "red");
        }
        else
        {
            if (value < 0)
                return ColorText(value.ToString(), "red");
            else
                return ColorText($"+{value}", "green");
        }
    }

    /// <summary>
    /// ͨ��Floatֵ������ɫ�������̡������죩
    /// </summary>
    /// <param name="value">ֵ</param>
    /// <param name="isReverse">�Ƿ�ת</param>
    /// <returns></returns>
    public static string ColorTextWithFloat(float value,bool isReverse = false)
    {
        if (value == 0) return "";
        if (isReverse)
        {
            if (value < 0)
                return ColorText(value.ToString("F2"), "green");
            else
                return ColorText($"+{value.ToString("F2")}", "red");
        }
        else
        {
            if (value < 0)
                return ColorText(value.ToString("F2"), "red");
            else
                return ColorText($"+{value.ToString("F2")}", "green");
        }
    }
}
