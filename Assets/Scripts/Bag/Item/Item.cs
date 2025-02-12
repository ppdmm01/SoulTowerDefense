using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

/// <summary>
/// 物品
/// </summary>
public class Item : MonoBehaviour, IDragHandler,IPointerDownHandler,IPointerUpHandler
{
    [Header("物品数据")]
    public ItemSO data; //物品数据
    public int currentRotation; //当前旋转度数
    private Coroutine rotateCoroutine; //旋转动画协程

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
    private bool isDrag; //是否被拖拽

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Icon = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        Init();
    }

    private void Update()
    {
        //右键旋转
        if (isDrag && Input.GetMouseButtonDown(1))
        {
            RotateItem(-90); //顺时针旋转
        }
    }

    private void Init()
    {
        isDrag = false;
        currentRotation = 0;
        oldGridPos = Defines.nullValue;
        lastFrameGridPos = gridPos;
        bagGrid = BagManager.Instance.BagDic["bag"]; //记录所属背包
        Icon.sprite = data.icon; //更换图片
        Vector2Int size = GetSize();
        rectTransform.sizeDelta = new Vector2(size.x * Defines.cellSize, size.y * Defines.cellSize); //根据数据设置物品大小  
    }


    #region 操作相关
    //点击物品
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Begin(eventData);
    }

    //拖动物品
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Process(eventData);
    }

    //放下物品
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            End(eventData);
    }

    private void Begin(PointerEventData eventData)
    {
        oldPos = transform.position; //记录原本位置
        transform.position = eventData.position; //物品吸附
        canvasGroup.alpha = 0.6f; //半透明  
        canvasGroup.blocksRaycasts = false; //防止遮挡射线  
        isDrag = true;

        //取消当前格子占用
        if (bagGrid.CheckBound(this,oldGridPos)) //如果原先有记录才能取消
            bagGrid.RemoveItem(this, oldGridPos);
        //更新预览
        UpdatePreview();
    }

    private void Process(PointerEventData eventData)
    {
        transform.position = eventData.position;
        //更新预览
        UpdatePreview();
    }

    private void End(PointerEventData eventData)
    {
        //物品恢复原来的状态
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        isDrag = false;

        bagGrid.ItemPreview(this, gridPos, false);//取消当前预览

        //尝试放置物品
        if (bagGrid.CanPlaceItem(this, gridPos))
        {
            bagGrid.PlaceItem(this, gridPos); //放置物品
            oldGridPos = gridPos; //记录格子起始坐标
        }
        else
        {
            if (bagGrid.CheckBound(this,oldGridPos))
                bagGrid.PlaceItem(this, oldGridPos); //恢复原本格子占用
            transform.position = oldPos; //回弹  
        }
    }

    //更新预览
    public void UpdatePreview()
    {
        gridPos = GetStartGridPoint(Input.mousePosition);
        bagGrid.ItemPreview(this, lastFrameGridPos, false); //取消上一次的预览格子
        lastFrameGridPos = gridPos; //记录
        bagGrid.ItemPreview(this, gridPos, true); //更新当前预览格子
    }
    #endregion

    #region 计算相关
    //获取网格起始坐标
    public Vector2Int GetStartGridPoint(Vector2 screenPos)
    {
        Vector2Int centerGridPos = ScreenToGridPoint(screenPos); //物品中心所在网格坐标
        return CalculateStartGridPoint(centerGridPos); //当前的格子起始坐标 
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
        Vector2Int size = GetSize();
        Vector2Int offset = GetOriginOffset();
        //计算方法：中心点 - 物品矩形长/2 - 物品有效区域左下角 离 形状矩阵左下角原点的 偏移
        //centerPoint - size/2 - offset
        return new Vector2Int(centerPoint.x - size.x / 2 - offset.x, centerPoint.y - size.y / 2 - offset.y);
    }

    //获取当前的大小（根据旋转角度而变化）
    public Vector2Int GetSize()
    {
        return data.shape.GetEffectiveSize(currentRotation);
    }

    //获取形状离原点的偏移
    public Vector2Int GetOriginOffset()
    {
        return data.shape.GetOriginOffset(currentRotation);
    }

    public Vector2Int GetEndOffset()
    {
        return data.shape.GetEndOffset(currentRotation);
    }

    //获取旋转后的矩阵
    public bool[,] GetRotateMatrix()
    {
        return data.shape.GetRotatedMatrix(currentRotation);
    }
    #endregion

    #region 旋转相关
    //旋转物品
    private void RotateItem(int angle)
    {
        if (!data.shape.allowRotation) return;

        //清空之前的预览
        bagGrid.ItemPreview(this, lastFrameGridPos, false);

        //旋转并播放动画
        currentRotation = (currentRotation + angle + 360) % 360; //加上360，防止出现负数
        RotationAnimation();

        //旋转后重新更新预览
        gridPos = GetStartGridPoint(Input.mousePosition);
        lastFrameGridPos = gridPos; //更新记录
        bagGrid.ItemPreview(this, gridPos, true);
    }

    //旋转动画
    private void RotationAnimation()
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);
        rotateCoroutine = StartCoroutine(RotateSmoothly());
    }

    //平滑旋转动画
    IEnumerator RotateSmoothly()
    {
        float duration = 0.2f;
        Quaternion start = rectTransform.rotation;
        Quaternion end = Quaternion.Euler(0, 0, currentRotation);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            rectTransform.rotation = Quaternion.Slerp(start, end, t / duration);
            yield return null;
        }
        rectTransform.rotation = end;
    }
    #endregion

}
