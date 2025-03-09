using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 商店物品栏（BagGrid的改版）
/// </summary>
public class StoreGrid : BaseGrid
{
    public TextMeshProUGUI priceTxt; //价格文本

    /// <summary>
    /// 刷新物品
    /// </summary>
    public void RefreshItem(ItemSO itemData)
    {
        //清楚货架上的物品
        DestroyAllItems();
        //添加物品
        BagManager.Instance.AddItemByName(itemData.itemName,this);
        //设置价格
        priceTxt.text = itemData.price.ToString();
    }

    /// <summary>
    /// 尝试放置物品
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public override bool TryAutoPlaceItem(Item item)
    {
        // 重置旋转角度  
        item.currentRotation = 0;
        item.rectTransform.rotation = Quaternion.Euler(0, 0, item.currentRotation);
        //固定位置放置
        int x = gridWidth / 2 - 1;
        int y = gridHeight / 2;
        item.gridPos = new Vector2Int(x, y);
        if (CanPlaceItem(item, item.gridPos))
        {
            PlaceItem(item, item.gridPos);
            item.oldGridPos = item.gridPos; //更新老位置坐标
            return true;
        }
        return false;
    }
}