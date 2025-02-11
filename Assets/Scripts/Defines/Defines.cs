using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定义一些常量
/// </summary>
public static class Defines
{
    //背包相关常量
    public static float cellSize = 64f; //每个格子大小
    public static Color invalidColor = Color.red; //背包格被占用时的颜色
    public static Color validColor = Color.white; //背包格未占用时的颜色
    public static Vector2Int nullValue = new Vector2Int(-1,-1); //初始化值，相当于null
}
