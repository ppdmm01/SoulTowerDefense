using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SynthesisSO",menuName = "ScriptableObject/SynthesisSO")]
public class SynthesisSO : ScriptableObject
{
    //��Ʒ����
    public enum ItemType
    {
        Data, //����ĳ����Ʒ
        Tag, //ĳһ���ǩ����Ʒ
    }
    public int id; //id
    public List<SynthesisItem> recipe; //�䷽
    public List<SynthesisItem> product; //��Ʒ

    public bool isRandomTagItem = true; //�䷽�ͳ�Ʒ�е���Ʒ��ǩ��ͬʱ���Ƿ������Ʒ 
    [Header("��Ʒ��õ�buff���")]
    public BuffType buff;

    //ͳ���䷽��Ҫ����Ʒ����
    public int CountRecipeNum()
    {
        int num = 0;
        for (int i = 0; i < recipe.Count; i++)
        {
            num += recipe[i].num;
        }
        return num;
    }

    //�䷽���Ƿ�����˸���Ʒ��ͨ���������Ʒ�������ݲ鿴��
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
    //��Ʒ����
    public enum ItemType
    {
        Data, //����ĳ����Ʒ
        Tag, //ĳһ���ǩ����Ʒ
    }

    public ItemType type;
    public List<ItemTag> tags;
    public ItemSO data;
    public int num; //��Ҫ������
}
