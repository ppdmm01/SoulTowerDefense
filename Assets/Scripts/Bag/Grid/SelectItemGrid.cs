using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可以选择物品的网格
/// </summary>
public class SelectItemGrid : BaseGrid
{
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
