using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ��Ʒ
/// </summary>
public class Item : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("��Ʒ����")]
    public ItemSO data; //��Ʒ����
    public int growSpeed = 1; //�ɳ��ٶ�
    public List<ItemAttribute> nowAttributes; //��Ҫʵʱ��¼���ԣ���Ϊ��һЩ�ɳ�����ߣ�

    [Header("��Ʒ��ת���")]
    [HideInInspector] public int currentRotation; //��ǰ��ת����
    [HideInInspector] public int lastCurrentRotation; //��¼�ƶ���Ʒǰ����ת����
    private Coroutine rotateCoroutine; //��ת����Э��

    [Header("λ����ؼ�¼����")]
    private Vector2 oldPos; //��¼ԭ��λ�ã�ʵ������λ�ã�
    [HideInInspector] public Vector2Int gridPos; //��Ʒ��ǰ������ʼ���꣨��������λ�ã���Ʒ��λ�ã�
    [HideInInspector] public Vector2Int oldGridPos; //��¼��Ʒ�ƶ�ǰ����ʼ���꣨��������λ�ã���Ʒ��λ�ã�
    private Vector2Int lastFrameGridPos; //��¼��һ֡��ʼ���꣨��������λ�ã���Ʒ�ƶ������е���һ֡λ�ã�
    [HideInInspector] public BaseGrid grid; //��ǰ�����ĸ�����
    [HideInInspector] public BaseGrid oldGrid; //ԭ�������ĸ�����
    public List<Vector2Int> usedPos; //��Ʒռ�ݵĸ�������

    [Header("�Ӿ����")]
    private List<GameObject> starPoints; //��¼Ŀǰ��������
    private List<GameObject> itemFrameList; //��¼��Ʒ�߿�

    [Header("��������")]
    private CanvasGroup canvasGroup; //��������Ʒ��͸�� �� ��ֹ�ڵ����߼��
    private Image Icon; //ͼƬ
    [HideInInspector] public RectTransform rectTransform; //�任���
    private bool isDrag; //�Ƿ���ק
    [HideInInspector] public bool isDelete; //�Ƿ�׼��ɾ��
    [HideInInspector] public bool isUpdateInfo; //λ�û�Ƕȱ䶯ʱ�Ƿ������Ϣ

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Icon = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        starPoints = new List<GameObject>();
        itemFrameList = new List<GameObject>();
        usedPos = new List<Vector2Int>();
    }

    private void Update()
    {
        //�Ҽ���ת
        if (isDrag && Input.GetMouseButtonDown(1))
        {
            RotateItem(-90); //˳ʱ����ת
        }

        //�����Ʒ�����������仯��Ŀǰʹ�������⼴�ɣ����ټ�������
        if (isDrag)
        {
            //����Ƿ��ڱ�����
            if (GridManager.Instance.IsInsideGrid(Input.mousePosition, "Bag") && grid.gridName != "Bag")
            {
                grid = GridManager.Instance.GetBagByName("Bag");
            }
            //����Ƿ��ڴ�������
            if (GridManager.Instance.IsInsideGrid(Input.mousePosition, "StorageBox") && grid.gridName != "StorageBox")
            {
                grid = GridManager.Instance.GetBagByName("StorageBox");
            }
            //����Ƿ��ڶ���¯��
            if (GridManager.Instance.IsInsideGrid(Input.mousePosition, "ForgeGrid") && grid.gridName != "ForgeGrid")
            {
                grid = GridManager.Instance.GetBagByName("ForgeGrid");
            }
        }
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <param name="data">��Ʒ����</param>
    /// <param name="grid">�������ĸ�������</param>
    public void Init(ItemSO data, BaseGrid grid)
    {
        this.data = data;

        nowAttributes = data.GetItemAttributes(); //��¼����

        isUpdateInfo = true;
        isDrag = false;
        currentRotation = 0;
        lastCurrentRotation = currentRotation;

        oldGridPos = Defines.nullValue;
        lastFrameGridPos = gridPos;

        this.grid = grid;
        oldGrid = this.grid;

        Icon.sprite = data.icon; //����ͼƬ

        Vector2Int size = GetSize();
        rectTransform.sizeDelta = new Vector2(size.x * Defines.cellSize, size.y * Defines.cellSize); //��������������Ʒ��С  

        //������Ʒ�߿�
        CreateItemFrame();
        HideItemFrame();
    }


    #region �������
    //�����Ʒ
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Begin(eventData);
    }

    //�϶���Ʒ
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Process(eventData);
    }

    //������Ʒ
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            End(eventData);
    }

    //��ʾ��Ϣ
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ItemManager.Instance.dragTarget == null)
        {
            GetConnectItems(true);
            UIManager.Instance.GetPanel<BagPanel>()?.ShowItemInfo(data,nowAttributes);
        }
    }

    //������Ϣ
    public void OnPointerExit(PointerEventData eventData)
    {
        HideAllStar();
        UIManager.Instance.GetPanel<BagPanel>()?.HideItemInfo();
    }

    private void Begin(PointerEventData eventData)
    {
        oldPos = transform.position; //��¼ԭ��λ��
        lastCurrentRotation = currentRotation; //��¼��һ����ת����
        ItemManager.Instance.dragTarget = this;

        transform.position = eventData.position; //��Ʒ����
        canvasGroup.alpha = 0.6f; //��͸��  
        canvasGroup.blocksRaycasts = false; //��ֹ�ڵ�����  
        isDrag = true;

        //ȡ����ǰ����ռ��
        if (grid.CheckBound(this, gridPos))
            grid.RemoveItem(this, gridPos);

        UpdatePreview(); //����Ԥ��
        ShowItemFrame(); //��ʾ�߿�
    }

    private void Process(PointerEventData eventData)
    {
        transform.position = eventData.position;
        //����Ԥ��
        UpdatePreview();
    }

    private void End(PointerEventData eventData)
    {
        //��Ʒ�ָ�ԭ����״̬
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        isDrag = false;
        ItemManager.Instance.dragTarget = null;

        CancelPreview(gridPos);//ȡ����ǰԤ��
        HideItemFrame(); //���ر߿�

        if (isDelete) return; //���׼��ɾ����������

        //���Է�����Ʒ
        if (grid.CanPlaceItem(this, gridPos) 
            && (!oldGrid.isLocked || (oldGrid.isLocked && grid != oldGrid))) //ԭ�����񲻴��� ���� �����˵����ƶ����������ȥ�˾�����óɹ�
        {
            grid.PlaceItem(this, gridPos); //������Ʒ
            if (oldGrid.gridName == "StoreGrid")
            {
                //������Ʒ
                BuyThisItem();
            }
            oldGrid = grid; //�����ϱ���
            oldGridPos = gridPos; //������λ������
        }
        else
        {
            RecoverItem(); //�ָ���Ʒλ��
        }
    }

    //�ָ�����λ��״̬
    public void RecoverItem()
    {
        if (oldGrid != grid)
            grid = oldGrid; //�������ݵ�ԭ����

        if (lastCurrentRotation != currentRotation)
        {
            RotateItem(lastCurrentRotation - currentRotation); //���ݵ�ԭ���ĽǶ�
            CancelPreview(gridPos); //��תʱ�����Ԥ������Ҫ�ٴιص�
        }

        if (grid.CanPlaceItem(this, oldGridPos))
        {
            grid.PlaceItem(this, oldGridPos); //�ָ�ԭ������ռ��
            gridPos = oldGridPos; //λ�û��ݵ�ԭ����
            transform.position = oldPos; //�ص�  
        }
        else
        {
            Debug.LogError($"��Ʒ {data.itemName} �޷����ã�������������");
            //��ʾ
            UIManager.Instance.CreateUIObj("UI/UIObj/TipInfo", UIManager.Instance.topCanvasTrans);
        }
    }
    #endregion

    #region Ԥ�����
    //����Ԥ��
    public void UpdatePreview()
    {
        CancelPreview(lastFrameGridPos); //ȡ����һ�ε�Ԥ������

        //����λ��
        gridPos = GetStartGridPoint(Input.mousePosition);
        lastFrameGridPos = gridPos; //��¼

        ShowPreview(gridPos); //���µ�ǰԤ������
    }

    //ȡ��ָ��λ�õ�Ԥ��
    public void CancelPreview(Vector2Int gridPos)
    {
        grid.ItemPreview(this, gridPos, false);
        HideAllStar();
    }

    //��ʾָ��λ�õ�Ԥ��
    public void ShowPreview(Vector2Int gridPos)
    {
        grid.ItemPreview(this, gridPos, true);
        if (grid.isOpenPreview || !isDrag) //������Ԥ������û����קʱ����ʾ����
            GetConnectItems(true);
        transform.SetAsLastSibling(); //�����ڸ��������һ�㣬��ק����ƷҪ��ʾ����ǰ��
    }

    //������Ʒ�߿�
    public void CreateItemFrame()
    {
        Vector2 size = GetSize();
        Vector2Int originOffset = GetOriginOffset();
        bool[,] matrix = GetRotateMatrix();

        //����ÿ���߿�λ�ò�����
        for (int x = 0; x < ItemShape.MatrixLen; x++)
        {
            for (int y = 0; y < ItemShape.MatrixLen; y++)
            {
                if (matrix[x, y])
                {
                    GameObject itemFrameObj = UIManager.Instance.CreateUIObj("Bag/ItemFrame", transform);
                    Vector2Int point = new Vector2Int(x, y) - originOffset; //����ת������Ʒλ�����꣨ת���ɴ�(0,0)��ʼ��������㣩
                    Vector2 mousePoint = new Vector2((size.x - 1) / 2, (size.y - 1) / 2); //���������λ���±꣨��С���㣩
                    Vector2 offset = point - mousePoint; //���������ñ߿�����λ�õ�ƫ��

                    itemFrameObj.transform.localPosition = new Vector3(offset.x * Defines.cellSize, offset.y * Defines.cellSize, 0);
                    itemFrameList.Add(itemFrameObj);
                }
            }
        }
    }

    //��ʾ�߿�
    public void ShowItemFrame()
    {
        foreach (GameObject obj in itemFrameList)
        {
            obj.SetActive(true);
        }
    }

    //���ر߿�
    public void HideItemFrame()
    {
        foreach (GameObject obj in itemFrameList)
        {
            obj.SetActive(false);
        }
    }
    #endregion

    #region ����ͻ�ȡ�������
    //��ȡ������ʼ����
    public Vector2Int GetStartGridPoint(Vector2 screenPos)
    {
        Vector2Int centerGridPos = ScreenToGridPoint(screenPos); //��Ʒ����������������
        return CalculateStartGridPoint(centerGridPos); //��ǰ�ĸ�����ʼ���� 
    }

    // ת����Ļ���굽��������  
    private Vector2Int ScreenToGridPoint(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            grid.transform as RectTransform, //�Ա���Ϊ������
            screenPos,
            null,
            out Vector2 localPos
        );
        int gridX = Mathf.FloorToInt(localPos.x / Defines.cellSize);
        int gridY = Mathf.FloorToInt(localPos.y / Defines.cellSize);
        return new Vector2Int(gridX, gridY);
    }

    // �����������ʼ�������꣨������Ʒ�������꣩
    private Vector2Int CalculateStartGridPoint(Vector2Int centerPoint)
    {
        Vector2Int size = GetSize();
        Vector2Int offset = GetOriginOffset();
        //���㷽�������ĵ� - ��Ʒ���γ�/2 - ��Ʒ��Ч�������½� �� ��״�������½�ԭ��� ƫ��
        return new Vector2Int(centerPoint.x - size.x / 2 - offset.x, centerPoint.y - size.y / 2 - offset.y);
    }

    //��ȡ��ǰ�Ĵ�С��������ת�Ƕȶ��仯��
    public Vector2Int GetSize()
    {
        return data.shape.GetEffectiveSize(currentRotation);
    }

    //��ȡ��״��ԭ���ƫ��
    public Vector2Int GetOriginOffset()
    {
        return data.shape.GetOriginOffset(currentRotation);
    }

    public Vector2Int GetEndOffset()
    {
        return data.shape.GetEndOffset(currentRotation);
    }

    //��ȡ��ת��ľ���
    public bool[,] GetRotateMatrix()
    {
        return data.shape.GetRotatedMatrix(currentRotation);
    }

    //������Ʒռ�õĸ���λ����Ϣ
    public void ClearUsedPos()
    {
        usedPos.Clear();
    }

    //�����Ʒռ�õĸ���λ����Ϣ
    public void AddUsedPos(Vector2Int pos)
    {
        usedPos.Add(pos);
    }

    //��ȡ��Χ��Ʒ
    public List<Item> GetAroundItems()
    {
        List<Item> aroundItems = new List<Item>();
        Vector2Int up;
        Vector2Int down;
        Vector2Int left;
        Vector2Int right;
        foreach (Vector2Int pos in usedPos)
        {
            up = pos + Vector2Int.up;
            down = pos + Vector2Int.down;
            left = pos + Vector2Int.left;
            right = pos + Vector2Int.right;
            if (grid.CheckPoint(up))
            {
                Item item = grid.GetSlot(up).nowItem;
                if (item != null && item != this && !aroundItems.Contains(item)) aroundItems.Add(item);
            }
            if (grid.CheckPoint(down))
            {
                Item item = grid.GetSlot(down).nowItem;
                if (item != null && item != this && !aroundItems.Contains(item)) aroundItems.Add(item);
            }
            if (grid.CheckPoint(left))
            {
                Item item = grid.GetSlot(left).nowItem;
                if (item != null && item != this && !aroundItems.Contains(item)) aroundItems.Add(item);
            }
            if (grid.CheckPoint(right))
            {
                Item item = grid.GetSlot(right).nowItem;
                if (item != null && item != this && !aroundItems.Contains(item)) aroundItems.Add(item);
            }
        }
        return aroundItems;
    }
    #endregion

    #region ��ת���
    //��ת��Ʒ
    private void RotateItem(int angle)
    {
        if (!data.shape.allowRotation) return;

        //�����һ֡��Ԥ��
        CancelPreview(lastFrameGridPos);

        //��ת�����Ŷ���
        currentRotation = (currentRotation + angle + 360) % 360; //����360����ֹ���ָ���
        RotationAnimation();

        //��ת�����¸���Ԥ��
        gridPos = GetStartGridPoint(Input.mousePosition);
        lastFrameGridPos = gridPos; //���¼�¼

        ShowPreview(gridPos); //���µ�ǰԤ������
    }

    //��ת����
    private void RotationAnimation()
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);
        rotateCoroutine = StartCoroutine(RotateSmoothly());
    }

    //ƽ����ת����
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

    #region ���Ǽ�����أ������Ʒ�������ԣ�

    //��ȡ��Χ���Ǵ���Ʒ����ʾ���ǣ�isShowStar�Ƿ���ʾ���ǣ�
    public List<ConnectItemInfo> GetConnectItems(bool isShowStar = false)
    {
        List<ConnectItemInfo> neighborItemInfos = new List<ConnectItemInfo>();
        foreach (DetectionPoint point in data.detectionPoints)
        {
            //����ʵ�ʼ��λ��
            Vector2Int rotatedOffset = point.GetRotatePoint(currentRotation); //��ȡ��ǰ�Ƕ�Ŀ��λ�õĵ�
            Vector2Int checkPos = gridPos + rotatedOffset; //�ڱ����е�λ��

            //�߽���  
            if (!grid.CheckPoint(checkPos)) continue;

            //��ȡĿ����Ʒ�� 
            ItemSlot slot = grid.slots[checkPos.x, checkPos.y];
            //��ȡ���������
            ItemAttribute attribute = null;
            if (slot.nowItem != null)
                attribute = MatchLinkAttribute(slot.nowItem, point.pointType);
            //�ж��Ƿ񼤻�ɹ�
            if (slot.nowItem == null ||  attribute == null || 
                neighborItemInfos.Any(info => info.item == slot.nowItem))
            {
                if (isShowStar)
                    CreateStar(slot.transform.position, false, point.pointType); //����ʧ��
            }
            else
            {
                ConnectItemInfo connectItem = new ConnectItemInfo(slot.nowItem,attribute);
                if (isShowStar)
                    CreateStar(slot.transform.position, true, point.pointType); //����ɹ�
                neighborItemInfos.Add(connectItem);
            }
        }

        return neighborItemInfos.ToList();
    }

    //��������ͼƬ
    public void CreateStar(Vector2 pos, bool isShow, DetectionPoint.PointType type)
    {
        GameObject starObj = UIManager.Instance.CreateUIObjByPoolMgr("Bag/StarPoint");
        starObj.transform.position = pos;
        starObj.GetComponent<StarPoint>().SetStarActive(isShow, type);
        starPoints.Add(starObj);
    }

    //������������
    public void HideAllStar()
    {
        foreach (GameObject obj in starPoints)
        {
            UIManager.Instance.DestroyUIObjByPoolMgr(obj);
        }
        starPoints.Clear();
    }

    //ƥ��ָ����Ʒ�Ƿ��������Ʒ�� �������Լ���Ҫ��
    //2025.3.17���ģ����벻ͬ�� �������ͣ����Բ��ܼ������Ҫ���ˣ�ֻ����Ӧ �������� ��Ҫ�󼴿�
    private ItemAttribute MatchLinkAttribute(Item item,DetectionPoint.PointType pointType)
    {
        foreach (ItemAttribute attribute in nowAttributes)
        {
            if (attribute.attributeType == ItemAttribute.AttributeType.Global) continue; //ȫ�ֵĲ�����
            if (attribute.condition.pointType != pointType) continue; //Ҫ��ļ������Ͳ�һ�£�������

            if (attribute.IsMatch(item)) 
                return attribute; //���ƥ������һ������Ҫ�������Լ���ɹ�
        }
        return null;
    }
    #endregion

    #region �̵����
    //�������Ʒ
    private void BuyThisItem()
    {
        Debug.Log("����ɹ�");
    }
    #endregion

    /// <summary>
    /// ɾ���Լ�
    /// </summary>
    public void DeleteMe()
    {
        if (oldGrid.CheckBound(this, oldGridPos))
            oldGrid.RemoveItem(this, oldGridPos);
        else
            oldGrid.items.Remove(this);

        //���·�������Ϣ
        if (isUpdateInfo)
            GridManager.Instance.UpdateFightTowerInfo();

        //����
        //foreach (GameObject obj in itemFrameList)
        //    UIManager.Instance.DestroyUIObj(obj);
        itemFrameList.Clear();
        usedPos.Clear();
        Destroy(this.gameObject);
    }

    public void SetIsUpdateInfo(bool isUpdateInfo)
    {
        this.isUpdateInfo = isUpdateInfo;
    }
}