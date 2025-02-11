using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO",menuName = "ScriptableObject/ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("基础属性")]
    public string itemName; //物品名称
    public Vector2Int size = Vector2Int.one; // 物品占格尺寸  
    public Sprite icon; //图片

    [Header("物品标签")]
    public string[] itemTags; // 物品标签（如"力","药水"等）  

    //[Header("预览设置")]
    //public Color validColor = Color.green; //合法的颜色
    //public Color invalidColor = Color.red; //非法的颜色
}
