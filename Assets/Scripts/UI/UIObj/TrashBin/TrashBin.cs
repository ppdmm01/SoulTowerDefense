using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ������
/// </summary>
public class TrashBin : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Item item = eventData.pointerDrag.GetComponent<Item>();
        if (item == null) return;
        if (item.oldGrid.gridName == "StoreGrid")
        {
            item.RecoverItem();
            return;
        }
        item.currentRotation = item.lastCurrentRotation; //�ָ���ԭ���ĽǶ�
        if (eventData.pointerDrag != null)
        {
            item.SetIsUpdateInfo(true);
            item.DeleteMe();
            EffectManager.Instance.PlayUIEffect("SmokeUIEffect",item.transform.position);
        }
        //TODO:��ȡ��Դ
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<Item>() != null)
            eventData.pointerDrag.GetComponent<Item>().isDelete = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<Item>() != null)
            eventData.pointerDrag.GetComponent<Item>().isDelete = false;
    }
}
