using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class TrashBin : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"{eventData.pointerDrag.GetComponent<Item>().data.itemName}±»¶ªÈëÀ¬»øÍ°");
        if (eventData.pointerDrag != null)
            eventData.pointerDrag.GetComponent<Item>().DeleteMe();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
            eventData.pointerDrag.GetComponent<Item>().isDelete = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
            eventData.pointerDrag.GetComponent<Item>().isDelete = false;
    }
}
