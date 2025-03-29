using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ѡ����Ʒ������
/// </summary>
public class SelectItemGrid : BaseGrid
{
    /// <summary>
    /// ���Է�����Ʒ
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public override bool TryAutoPlaceItem(Item item)
    {
        // ������ת�Ƕ�  
        item.currentRotation = 0;
        item.rectTransform.rotation = Quaternion.Euler(0, 0, item.currentRotation);
        //�̶�λ�÷���
        int x = gridWidth / 2 - 1;
        int y = gridHeight / 2;
        item.gridPos = new Vector2Int(x, y);
        if (CanPlaceItem(item, item.gridPos))
        {
            PlaceItem(item, item.gridPos);
            item.oldGridPos = item.gridPos; //������λ������
            return true;
        }
        return false;
    }
}
