using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SynthesisSO",menuName = "ScriptableObject/SynthesisSO")]
public class SynthesisSO : ScriptableObject
{
    //物品类型
    public enum ItemType
    {
        Data, //具体某个物品
        Tag, //某一类标签的物品
    }
    public int id; //id
    public List<SynthesisItem> recipe; //配方
    public List<SynthesisItem> product; //成品

    public bool isRandomTagItem = true; //配方和成品中的物品标签相同时，是否随机物品 
    [Header("成品获得的buff标记")]
    public BuffType buff;

    //统计配方需要的物品数量
    public int CountRecipeNum()
    {
        int num = 0;
        for (int i = 0; i < recipe.Count; i++)
        {
            num += recipe[i].num;
        }
        return num;
    }

    //配方中是否包含了该物品（通过具体的物品配置数据查看）
    public bool IsRecipeContainsItemByData(ItemSO data)
    {
        foreach (SynthesisItem synthesisItem in recipe)
        {
            if (synthesisItem.type == SynthesisItem.ItemType.Data && synthesisItem.data.id == data.id) return true;
        }
        return false;
    }
}

[Serializable]
public class SynthesisItem
{
    //物品类型
    public enum ItemType
    {
        Data, //具体某个物品
        Tag, //某一类标签的物品
    }

    public ItemType type;
    public List<ItemTag> tags;
    public ItemSO data;
    public int num; //需要的数量
}
