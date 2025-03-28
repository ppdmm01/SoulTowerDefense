using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������
/// </summary>
[CreateAssetMenu(fileName = "CombinationSO", menuName = "ScriptableObject/CombinationSO")]
public class CombinationSO : ScriptableObject
{
    /// <summary>
    /// ���λ������
    /// </summary>
    public enum CombinationPosType
    {
        Any, //����λ��
        UpAndDown, //����
        LeftAndRight, //����
        Around, //��Χ
    }

    /// <summary>
    /// �����Ʒ�ķ�ʽ
    /// </summary>
    public enum ItemCombinationType
    {
        Tag, //ͨ����ǩ����ʽ
        Data, //ͨ�����ݵ���ʽ
    }

    [Header("�������")]
    public CombinationPosType posType;
    [Header("�������")]
    public string combinationName;
    [Header("�������")]
    public string description;
    [Header("��������")]
    public ItemAttribute activeAttribute;

    [Header("�����Ʒ�ķ�ʽ")]
    public ItemCombinationType combinationType;
    [Header("������ã���ǩ��ʽ")]
    [Header("����")]
    public ItemTag tag;
    public int itemNum;
    [Header("������ã�������ʽ")]
    [Header("����")]
    public List<ItemSO> items;
    [Header("��/��")]
    public ItemSO item1;
    [Header("��/��")]
    public ItemSO item2;
}
