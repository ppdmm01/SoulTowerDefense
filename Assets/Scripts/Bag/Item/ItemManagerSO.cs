using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemManagerSO",menuName = "ScriptableObject/ItemManagerSO")]
public class ItemManagerSO : ScriptableObject
{
    [Header("��Ʒ����")]
    public List<ItemSO> itemSOList;
}
