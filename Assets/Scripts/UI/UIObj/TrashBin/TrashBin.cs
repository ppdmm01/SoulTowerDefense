using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrashBin : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Item item = eventData.pointerDrag.GetComponent<Item>();
        Debug.Log($"{item.data.itemName}±»¶ªÈëÀ¬»øÍ°");
        if (eventData.pointerDrag != null)
            item.DeleteMe();
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
