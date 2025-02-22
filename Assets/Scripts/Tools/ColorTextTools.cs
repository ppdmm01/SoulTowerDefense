using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 颜色文本工具
/// </summary>
public static class ColorTextTools
{
    /// <summary>
    /// 返回颜色文本
    /// </summary>
    /// <param name="info">信息</param>
    /// <param name="color">颜色</param>
    /// <returns>文本</returns>
    public static string ColorText(string info,string color)
    {
        return $"<color={color}>{info}</color>";
    }

    /// <summary>
    /// 返回带括弧的颜色文本
    /// </summary>
    /// <param name="info">信息</param>
    /// <param name="color">颜色</param>
    /// <returns>文本</returns>
    public static string ColorTextWithBrackets(string info, string color)
    {
        return $"<color={color}>【{info}】</color>";
    }

    /// <summary>
    /// 通过Int值返回颜色（正：绿、负：红）
    /// </summary>
    /// <param name="value">值</param>
    /// /// <param name="isReverse">是否反转</param>
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
    /// 通过Float值返回颜色（正：绿、负：红）
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="isReverse">是否反转</param>
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
