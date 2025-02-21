using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemManagerSO",menuName = "ScriptableObject/ItemManagerSO")]
public class ItemManagerSO : ScriptableObject
{
    [Header("物品数据")]
    public List<ItemSO> itemSOList;
}
