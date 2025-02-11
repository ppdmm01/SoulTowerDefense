using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO",menuName = "ScriptableObject/ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("��������")]
    public string itemName; //��Ʒ����
    public Vector2Int size = Vector2Int.one; // ��Ʒռ��ߴ�  
    public Sprite icon; //ͼƬ

    [Header("��Ʒ��ǩ")]
    public string[] itemTags; // ��Ʒ��ǩ����"��","ҩˮ"�ȣ�  
}
