using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 物品
/// </summary>
public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("物品数据")]
    public ItemSO data; //物品数据

    [Header("相关记录数据")]
    private Vector2 oldPos; //记录原本位置（实际坐标位置）
    private Vector2Int gridPos; //物品放置起始坐标（网格坐标位置，物品新位置）
    private Vector2Int oldGridPos; //记录原本的起始坐标（网格坐标位置，物品旧位置）
    private Vector2Int lastFrameGridPos; //记录上一帧格子的坐标（网格坐标位置，物品移动过程中的上一帧位置）
    private BagGrid bagGrid; //当前属于哪个背包

    [Header("其他变量")]
    private CanvasGroup canvasGroup; //用于让物品半透明 和 防止遮挡射线检测
    private Image Icon; //图片
    private RectTransform rectTransform; //变换组件

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Icon = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        Init();
    }

    private void Init()
    {
        oldGridPos = Defines.nullValue;
        lastFrameGridPos = gridPos;
        bagGrid = BagManager.Instance.BagDic["bag"]; //记录所属背包
        Icon.sprite = data.icon; //更换图片
        rectTransform.sizeDelta = new Vector2(data.size.x * Defines.cellSize, data.size.y * Defines.cellSize); //根据数据设置物品大小  
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        oldPos = transform.position; //记录原本位置
        canvasGroup.alpha = 0.6f; //半透明  
        canvasGroup.blocksRaycasts = false; //防止遮挡射线  

        //取消当前格子占用
        if (oldGridPos != Defines.nullValue) //如果原先有记录才能取消
            bagGrid.RemoveItem(this, oldGridPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;

        //---格子预览功能---
        //计算放置位置
        Vector2Int centerGridPos = ScreenToGridPoint(eventData.position); //物品中心所在网格坐标
        gridPos = CalculateStartGridPoint(centerGridPos); //当前的格子起始坐标
        //更新预览
        if (lastFrameGridPos != gridPos && bagGrid.CheckBound(this, gridPos))
        {
            Debug.Log("last:" + lastFrameGridPos);
            Debug.Log("now:" + gridPos);
            bagGrid.ItemPreview(this, lastFrameGridPos, false); //取消上一次的预览格子
            lastFrameGridPos = gridPos; //记录
            bagGrid.ItemPreview(this, gridPos, true); //更新当前预览格子
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //物品恢复原来的状态
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 计算放置位置  
        //Vector2Int centerGridPos = ScreenToGridPoint(eventData.position); //物品中心所在网格坐标
        //Vector2Int gridPos = CalculateStartGridPoint(centerGridPos); //当前的格子起始坐标
        //Debug.Log("gridPos:"+gridPos);
        //Debug.Log("oldGridPos:" + oldGridPos);

        bagGrid.ItemPreview(this, gridPos, false);//取消预览

        if (bagGrid.CanPlaceItem(this, gridPos))
        {
            //print("放置物品成功");
            bagGrid.PlaceItem(this, gridPos); //放置物品
            oldGridPos = gridPos; //记录格子起始坐标
        }
        else
        {
            //print("放置物品失败");
            if (oldGridPos != Defines.nullValue && bagGrid.CheckBound(this,oldGridPos)) //只有物品原本位置合理才能恢复
                bagGrid.PlaceItem(this, oldGridPos); //恢复原本格子占用
            transform.position = oldPos; //回弹  
        }
    }

    // 转换屏幕坐标到网格坐标  
    private Vector2Int ScreenToGridPoint(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            bagGrid.transform as RectTransform, //以背包为父对象
            screenPos,
            null,
            out Vector2 localPos
        );
        int gridX = Mathf.FloorToInt(localPos.x / Defines.cellSize);
        int gridY = Mathf.FloorToInt(localPos.y / Defines.cellSize);
        return new Vector2Int(gridX, gridY);
    }

    // 计算物体的起始网格坐标（传入物品中心坐标）
    private Vector2Int CalculateStartGridPoint(Vector2Int centerPoint)
    {
        Vector2Int pos = Vector2Int.zero;
        pos.x = centerPoint.x - data.size.x/2;
        pos.y = centerPoint.y - data.size.y/2;
        return pos;
    }
}
