using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ��Ʒ
/// </summary>
public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("��Ʒ����")]
    public ItemSO data; //��Ʒ����

    [Header("��ؼ�¼����")]
    private Vector2 oldPos; //��¼ԭ��λ�ã�ʵ������λ�ã�
    private Vector2Int gridPos; //��Ʒ������ʼ���꣨��������λ�ã���Ʒ��λ�ã�
    private Vector2Int oldGridPos; //��¼ԭ������ʼ���꣨��������λ�ã���Ʒ��λ�ã�
    private Vector2Int lastFrameGridPos; //��¼��һ֡���ӵ����꣨��������λ�ã���Ʒ�ƶ������е���һ֡λ�ã�
    private BagGrid bagGrid; //��ǰ�����ĸ�����

    [Header("��������")]
    private CanvasGroup canvasGroup; //��������Ʒ��͸�� �� ��ֹ�ڵ����߼��
    private Image Icon; //ͼƬ
    private RectTransform rectTransform; //�任���

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
        bagGrid = BagManager.Instance.BagDic["bag"]; //��¼��������
        Icon.sprite = data.icon; //����ͼƬ
        rectTransform.sizeDelta = new Vector2(data.size.x * Defines.cellSize, data.size.y * Defines.cellSize); //��������������Ʒ��С  
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        oldPos = transform.position; //��¼ԭ��λ��
        canvasGroup.alpha = 0.6f; //��͸��  
        canvasGroup.blocksRaycasts = false; //��ֹ�ڵ�����  

        //ȡ����ǰ����ռ��
        if (oldGridPos != Defines.nullValue) //���ԭ���м�¼����ȡ��
            bagGrid.RemoveItem(this, oldGridPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;

        //---����Ԥ������---
        //�������λ��
        Vector2Int centerGridPos = ScreenToGridPoint(eventData.position); //��Ʒ����������������
        gridPos = CalculateStartGridPoint(centerGridPos); //��ǰ�ĸ�����ʼ����
        //����Ԥ��
        if (lastFrameGridPos != gridPos && bagGrid.CheckBound(this, gridPos))
        {
            Debug.Log("last:" + lastFrameGridPos);
            Debug.Log("now:" + gridPos);
            bagGrid.ItemPreview(this, lastFrameGridPos, false); //ȡ����һ�ε�Ԥ������
            lastFrameGridPos = gridPos; //��¼
            bagGrid.ItemPreview(this, gridPos, true); //���µ�ǰԤ������
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //��Ʒ�ָ�ԭ����״̬
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // �������λ��  
        //Vector2Int centerGridPos = ScreenToGridPoint(eventData.position); //��Ʒ����������������
        //Vector2Int gridPos = CalculateStartGridPoint(centerGridPos); //��ǰ�ĸ�����ʼ����
        //Debug.Log("gridPos:"+gridPos);
        //Debug.Log("oldGridPos:" + oldGridPos);

        bagGrid.ItemPreview(this, gridPos, false);//ȡ��Ԥ��

        if (bagGrid.CanPlaceItem(this, gridPos))
        {
            //print("������Ʒ�ɹ�");
            bagGrid.PlaceItem(this, gridPos); //������Ʒ
            oldGridPos = gridPos; //��¼������ʼ����
        }
        else
        {
            //print("������Ʒʧ��");
            if (oldGridPos != Defines.nullValue && bagGrid.CheckBound(this,oldGridPos)) //ֻ����Ʒԭ��λ�ú�����ָܻ�
                bagGrid.PlaceItem(this, oldGridPos); //�ָ�ԭ������ռ��
            transform.position = oldPos; //�ص�  
        }
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
        Vector2Int pos = Vector2Int.zero;
        pos.x = centerPoint.x - data.size.x/2;
        pos.y = centerPoint.y - data.size.y/2;
        return pos;
    }
}
