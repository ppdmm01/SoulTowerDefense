using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 组合数据
/// </summary>
[CreateAssetMenu(fileName = "CombinationSO", menuName = "ScriptableObject/CombinationSO")]
public class CombinationSO : ScriptableObject
{
    /// <summary>
    /// 组合位置类型
    /// </summary>
    public enum CombinationPosType
    {
        Any, //任意位置
        UpAndDown, //上下
        LeftAndRight, //左右
        Around, //周围
    }

    /// <summary>
    /// 组合物品的方式
    /// </summary>
    public enum ItemCombinationType
    {
        Tag, //通过标签的形式
        Data, //通过数据的形式
    }

    [Header("组合类型")]
    public CombinationPosType posType;
    [Header("组合名称")]
    public string combinationName;
    [Header("组合描述")]
    public string description;
    [Header("激活属性")]
    public ItemAttribute activeAttribute;

    [Header("组合物品的方式")]
    public ItemCombinationType combinationType;
    [Header("组合配置：标签形式")]
    [Header("任意")]
    public ItemTag tag;
    public int itemNum;
    [Header("组合配置：数据形式")]
    [Header("任意")]
    public List<ItemSO> items;
    [Header("上/左")]
    public ItemSO item1;
    [Header("下/右")]
    public ItemSO item2;
}
