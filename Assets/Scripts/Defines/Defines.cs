using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定义一些常量
/// </summary>
public static class Defines
{
    //背包相关常量
    public static readonly float cellSize = 48f; //每个格子大小
    public static readonly float spacing = 2f; //每个格子间距
    public static readonly Color invalidColor = new Color32(200,200,200,150); //背包格被占用时的颜色
    public static readonly Color validColor = Color.white; //背包格未占用时的颜色
    public static readonly Color previewInvalidColor = Color.red; //预览非法颜色
    public static readonly Color previewValidColor = Color.green; //预览合法颜色
    public static readonly Vector2Int nullValue = new Vector2Int(-1,-1); //初始化值，相当于null

    //塔防相关常量
    public static readonly Color validRangeColor = new Color(0, 0, 0, 0.2f); //合法范围的颜色
    public static readonly Color invalidRangeColor = new Color(1f, 0f, 0f, 0.2f); //无效范围的颜色

    //波次相关常量
    public static readonly float waitTime = 5f; //每波之间的休息时间
}
