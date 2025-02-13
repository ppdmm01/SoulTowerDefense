using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

/// <summary>
/// ��Ʒ
/// </summary>
public class Item : MonoBehaviour, IDragHandler,IPointerDownHandler,IPointerUpHandler
{
    [Header("��Ʒ����")]
    public ItemSO data; //��Ʒ����
    public int currentRotation; //��ǰ��ת����
    private int lastCurrentRotation; //��¼�ƶ���Ʒǰ����ת����
    private Coroutine rotateCoroutine; //��ת����Э��

    [Header("��ؼ�¼����")]
    private Vector2 oldPos; //��¼ԭ��λ�ã�ʵ������λ�ã�
    [HideInInspector] public Vector2Int gridPos; //��Ʒ��ǰ������ʼ���꣨��������λ�ã���Ʒ��λ�ã�
    [HideInInspector] public Vector2Int oldGridPos; //��¼��Ʒ�ƶ�ǰ����ʼ���꣨��������λ�ã���Ʒ��λ�ã�
    private Vector2Int lastFrameGridPos; //��¼��һ֡��ʼ���꣨��������λ�ã���Ʒ�ƶ������е���һ֡λ�ã�
    private BagGrid bagGrid; //��ǰ�����ĸ�����
    private BagGrid oldBagGrid; //ԭ�������ĸ�����

    [Header("��������")]
    private CanvasGroup canvasGroup; //��������Ʒ��͸�� �� ��ֹ�ڵ����߼��
    private Image Icon; //ͼƬ
    [HideInInspector] public RectTransform rectTransform; //�任���
    private bool isDrag; //�Ƿ���ק

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Icon = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
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
            if (BagManager.Instance.IsInsideBag(Input.mousePosition,"bag") && bagGrid.bagName != "bag")
            {
                oldBagGrid = bagGrid;
                bagGrid = BagManager.Instance.BagDic["bag"];
            }
            //����Ƿ��ڴ�������
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

        Icon.sprite = data.icon; //����ͼƬ

        Vector2Int size = GetSize();
        rectTransform.sizeDelta = new Vector2(size.x * Defines.cellSize, size.y * Defines.cellSize); //��������������Ʒ��С  
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

    private void Begin(PointerEventData eventData)
    {
        oldPos = transform.position; //��¼ԭ��λ��
        lastCurrentRotation = currentRotation; //��¼��һ����ת����

        transform.position = eventData.position; //��Ʒ����
        canvasGroup.alpha = 0.6f; //��͸��  
        canvasGroup.blocksRaycasts = false; //��ֹ�ڵ�����  
        isDrag = true;

        //ȡ����ǰ����ռ��
        if (bagGrid.CheckBound(this, gridPos))
            bagGrid.RemoveItem(this, gridPos);
        //����Ԥ��
        UpdatePreview();
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

        bagGrid.ItemPreview(this, gridPos, false);//ȡ����ǰԤ��

        //���Է�����Ʒ
        if (bagGrid.CanPlaceItem(this, gridPos))
        {
            bagGrid.PlaceItem(this, gridPos); //������Ʒ
            oldBagGrid = bagGrid; //�����ϱ���
            oldGridPos = gridPos; //������λ������
        }
        else
        {
            Debug.Log("Old:" + oldBagGrid);
            Debug.Log("New:" + bagGrid);
            if (oldBagGrid != bagGrid)
                bagGrid = oldBagGrid; //�������ݵ�ԭ����
            if (lastCurrentRotation != currentRotation)
            {
                RotateItem(lastCurrentRotation - currentRotation); //���ݵ�ԭ���ĽǶ�
                bagGrid.ItemPreview(this, gridPos, false); //��תʱ�����Ԥ������Ҫ�ٴιص�
            }

            if (bagGrid.CanPlaceItem(this, oldGridPos))
            {
                bagGrid.PlaceItem(this, oldGridPos); //�ָ�ԭ������ռ��
                gridPos = oldGridPos; //λ�û��ݵ�ԭ����
                transform.position = oldPos; //�ص�  
            }
            else
            {
                Debug.LogError($"��Ʒ {data.itemName} �޷����ã�������������");
                //TODO:��������ɾ��������һҳ�ȣ�
            }
        }
    }

    //����Ԥ��
    public void UpdatePreview()
    {
        gridPos = GetStartGridPoint(Input.mousePosition);
        bagGrid.ItemPreview(this, lastFrameGridPos, false); //ȡ����һ�ε�Ԥ������
        lastFrameGridPos = gridPos; //��¼
        bagGrid.ItemPreview(this, gridPos, true); //���µ�ǰԤ������
    }
    #endregion

    #region �������
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
            bagGrid.transform as RectTransform, //�Ա���Ϊ������
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
    #endregion

    #region ��ת���
    //��ת��Ʒ
    private void RotateItem(int angle)
    {
        if (!data.shape.allowRotation) return;

        //�����һ֡��Ԥ��
        bagGrid.ItemPreview(this, lastFrameGridPos, false);

        //��ת�����Ŷ���
        currentRotation = (currentRotation + angle + 360) % 360; //����360����ֹ���ָ���
        RotationAnimation();

        //��ת�����¸���Ԥ��
        gridPos = GetStartGridPoint(Input.mousePosition);
        lastFrameGridPos = gridPos; //���¼�¼
        bagGrid.ItemPreview(this, gridPos, true);
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

}