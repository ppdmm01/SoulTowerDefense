using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEngine.RuleTile.TilingRuleOutput;

/// <summary>
/// 物品
/// </summary>
public class Item : MonoBehaviour, IDragHandler,IPointerDownHandler,IPointerUpHandler,IPointerEnterHandler,IPointerExitHandler
{
    [Header("物品数据")]
    public ItemSO data; //物品数据
    public int currentRotation; //当前旋转度数
    private int lastCurrentRotation; //记录移动物品前的旋转度数
    private Coroutine rotateCoroutine; //旋转动画协程

    [Header("相关记录数据")]
    private Vector2 oldPos; //记录原本位置（实际坐标位置）
    [HideInInspector] public Vector2Int gridPos; //物品当前放置起始坐标（网格坐标位置，物品新位置）
    [HideInInspector] public Vector2Int oldGridPos; //记录物品移动前的起始坐标（网格坐标位置，物品旧位置）
    private Vector2Int lastFrameGridPos; //记录上一帧起始坐标（网格坐标位置，物品移动过程中的上一帧位置）
    private BagGrid bagGrid; //当前属于哪个背包
    private BagGrid oldBagGrid; //原本属于哪个背包

    private List<GameObject> starPoints; //记录目前亮的星星
    private List<GameObject> itemFrameList; //记录物品边框

    [Header("其他变量")]
    private CanvasGroup canvasGroup; //用于让物品半透明 和 防止遮挡射线检测
    private Image Icon; //图片
    [HideInInspector] public RectTransform rectTransform; //变换组件
    private bool isDrag; //是否被拖拽
    [HideInInspector] public bool isDelete; //是否准备删除

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Icon = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        starPoints = new List<GameObject>();
        itemFrameList = new List<GameObject>();
    }

    private void Update()
    {
        //右键旋转
        if (isDrag && Input.GetMouseButtonDown(1))
        {
            RotateItem(-90); //顺时针旋转
        }

        //检测物品的所属背包变化（目前使用鼠标点检测即可，减少计算量）
        if (isDrag)
        {
            //检测是否在背包中
            if (BagManager.Instance.IsInsideBag(Input.mousePosition,"bag") && bagGrid.bagName != "bag")
            {
                oldBagGrid = bagGrid;
                bagGrid = BagManager.Instance.BagDic["bag"];
            }
            //检测是否在储物箱中
            if (BagManager.Instance.IsInsideBag(Input.mousePosition, "storageBox") && bagGrid.bagName != "storageBox")
            {
                oldBagGrid = bagGrid;
                bagGrid = BagManager.Instance.BagDic["storageBox"];
            }
        }
    }

    public void Init(BagGrid bagGrid)
    {
        isDrag = false;
        currentRotation = 0;
        lastCurrentRotation = currentRotation;

        oldGridPos = Defines.nullValue;
        lastFrameGridPos = gridPos;

        this.bagGrid = bagGrid;
        oldBagGrid = this.bagGrid;

        Icon.sprite = data.icon; //更换图片
        Icon.alphaHitTestMinimumThreshold = 0.1f; //设置透明度阈值

        Vector2Int size = GetSize();
        rectTransform.sizeDelta = new Vector2(size.x * Defines.cellSize, size.y * Defines.cellSize); //根据数据设置物品大小  

        //生成物品边框
        CreateItemFrame();
        HideItemFrame();
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

    //显示信息
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetConnectItems();
        UIManager.Instance.ShowPanel<BagMessagePanel>().SetInfo(data.itemName,data.description);
    }

    //隐藏信息
    public void OnPointerExit(PointerEventData eventData)
    {
        HideAllStar();
        UIManager.Instance.HidePanel<BagMessagePanel>(false);
    }

    private void Begin(PointerEventData eventData)
    {
        oldPos = transform.position; //记录原本位置
        lastCurrentRotation = currentRotation; //记录上一次旋转度数

        transform.position = eventData.position; //物品吸附
        canvasGroup.alpha = 0.6f; //半透明  
        canvasGroup.blocksRaycasts = false; //防止遮挡射线  
        isDrag = true;

        //取消当前格子占用
        if (bagGrid.CheckBound(this, gridPos))
            bagGrid.RemoveItem(this, gridPos);

        UpdatePreview(); //更新预览
        ShowItemFrame(); //显示边框
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

        CancelPreview(gridPos);//取消当前预览
        HideItemFrame(); //隐藏边框

        if (isDelete) return; //如果准备删除，则跳过

        //尝试放置物品
        if (bagGrid.CanPlaceItem(this, gridPos))
        {
            bagGrid.PlaceItem(this, gridPos); //放置物品
            oldBagGrid = bagGrid; //更新老背包
            oldGridPos = gridPos; //更新老位置坐标
        }
        else
        {
            if (oldBagGrid != bagGrid)
                bagGrid = oldBagGrid; //背包回溯到原来的
            if (lastCurrentRotation != currentRotation)
            {
                RotateItem(lastCurrentRotation - currentRotation); //回溯到原来的角度
                CancelPreview(gridPos); //旋转时会更新预览，需要再次关掉
            }

            if (bagGrid.CanPlaceItem(this, oldGridPos))
            {
                bagGrid.PlaceItem(this, oldGridPos); //恢复原本格子占用
                gridPos = oldGridPos; //位置回溯到原来的
                transform.position = oldPos; //回弹  
            }
            else
            {
                Debug.LogError($"物品 {data.itemName} 无法放置，背包可能已满");
                //TODO:后续处理（删除、开新一页等）
            }
        }
    }
    #endregion

    #region 预览相关
    //更新预览
    public void UpdatePreview()
    {
        CancelPreview(lastFrameGridPos); //取消上一次的预览格子

        //更新位置
        gridPos = GetStartGridPoint(Input.mousePosition);
        lastFrameGridPos = gridPos; //记录

        ShowPreview(gridPos); //更新当前预览格子
    }

    //取消指定位置的预览
    public void CancelPreview(Vector2Int gridPos)
    {
        bagGrid.ItemPreview(this, gridPos, false);
        HideAllStar();
    }

    //显示指定位置的预览
    public void ShowPreview(Vector2Int gridPos)
    {
        bagGrid.ItemPreview(this, gridPos, true);
        GetConnectItems();
        transform.SetAsLastSibling(); //设置在父级的最后一层，拖拽的物品要显示在最前面
    }

    //生成物品边框
    public void CreateItemFrame()
    {
        Vector2 size = GetSize();
        Vector2Int originOffset = GetOriginOffset();
        bool[,] matrix = GetRotateMatrix();

        //计算每个边框位置并生成
        for (int x = 0; x < ItemShape.MatrixLen; x++)
        {
            for (int y = 0; y < ItemShape.MatrixLen; y++)
            {
                if (matrix[x, y])
                {
                    GameObject itemFrameObj = UIManager.Instance.CreateUIObj("Bag/ItemFrame");
                    Vector2Int point = new Vector2Int(x,y) - originOffset; //计算转换后物品位置坐标（转换成从(0,0)开始，方便计算）
                    Vector2 mousePoint = new Vector2((size.x - 1) / 2, (size.y - 1) / 2); //计算鼠标点的位置下标（含小数点）
                    Vector2 offset = point - mousePoint; //计算鼠标与该边框所在位置的偏移
             
                    itemFrameObj.transform.localPosition = new Vector3(offset.x * Defines.cellSize, offset.y * Defines.cellSize, 0);
                    itemFrameList.Add(itemFrameObj);    
                }
            }
        }
    }

    //显示边框
    public void ShowItemFrame()
    {
        foreach (GameObject obj in itemFrameList)
        {
            obj.SetActive(true);
        }
    }

    //隐藏边框
    public void HideItemFrame()
    {
        foreach (GameObject obj in itemFrameList)
        {
            obj.SetActive(false);
        }
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

        //清空上一帧的预览
        CancelPreview(lastFrameGridPos);

        //旋转并播放动画
        currentRotation = (currentRotation + angle + 360) % 360; //加上360，防止出现负数
        RotationAnimation();

        //旋转后重新更新预览
        gridPos = GetStartGridPoint(Input.mousePosition);
        lastFrameGridPos = gridPos; //更新记录

        ShowPreview(gridPos); //更新当前预览格子
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

    #region 检测周围物品相关

    //获取周围物品并显示所有星星
    public List<Item> GetConnectItems()
    {
        List<Item> neighbors = new List<Item>();

        foreach (DetectionPoint point in data.detectionPoints)
        {
            //计算实际检测位置
            Vector2Int rotatedOffset = point.GetRotatePoint(currentRotation); //获取当前角度目标位置的点
            Vector2Int checkPos = gridPos + rotatedOffset; //在背包中的位置

            //边界检查  
            if (!bagGrid.CheckPoint(checkPos)) continue;

            //获取目标物品  
            ItemSlot slot = bagGrid.slots[checkPos.x, checkPos.y];
            if (slot.nowItem == null)
            {
                CreateStar(slot.transform.position, false);
            }
            else
            {
                CreateStar(slot.transform.position, true);
                neighbors.Add(slot.nowItem);
            }

            //// 标签匹配检查  
            //if (point.requiredTags.Length > 0 &&
            //    !slot.item.data.itemTags.Intersect(point.requiredTags).Any())
            //    continue;
        }

        return neighbors.Distinct().ToList(); //去重
    }

    //设置星星图片
    public void CreateStar(Vector2 pos,bool isShow)
    {
        GameObject starObj = UIManager.Instance.CreateUIObj("Bag/StarPoint");
        starObj.transform.position = pos;
        starObj.GetComponent<StarPoint>().SetStarActive(isShow);
        starPoints.Add(starObj);
    }

    //隐藏所有星星
    public void HideAllStar()
    {
        foreach (GameObject obj in starPoints)
        {
            UIManager.Instance.DestroyUIObj(obj);
        }
        starPoints.Clear();
    }
    #endregion

    /// <summary>
    /// 删除自己
    /// </summary>
    public void DeleteMe()
    {
        if (bagGrid.CanPlaceItem(this, oldGridPos))
            bagGrid.RemoveItem(this, oldGridPos);
        else
            bagGrid.items.Remove(this);

        UIManager.Instance.GetPanel<BagPanel>().UpdateBagInfo();

        //销毁
        foreach (GameObject obj in itemFrameList)
            Destroy(obj);
        itemFrameList.Clear();
        Destroy(this.gameObject);
    }
}