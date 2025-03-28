using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 物品格
/// </summary>
public class ItemSlot : MonoBehaviour
{
    [HideInInspector] public bool isUsed; //是否被占用
    private BaseGrid grid; //物品格属于哪个网格
    [HideInInspector] public Item nowItem; //当前物品格的物品
    private float flashTime = 0.3f;
    private Color nowColor;

    private Image slotImg;

    void Awake()
    {
        nowItem = null;
        slotImg = GetComponent<Image>();
        SetStatus(false);
        Material material = new Material(Resources.Load<Material>("Material/FlashMaterial"));
        if (slotImg.material != material)
            slotImg.material = material;
        nowColor = slotImg.color;
    }

    /// <summary>
    /// 往格子里添加物品
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        nowItem = item;
        SetStatus(true);
    }

    /// <summary>
    /// 移除当前物品
    /// </summary>
    public void RemoveItem()
    {
        nowItem = null;
        SetStatus(false);
    }

    /// <summary>
    /// 设置所属背包
    /// </summary>
    /// <param name="bagGrid">背包</param>
    public void SetBelongs(BaseGrid grid)
    {
        this.grid = grid;
    }

    /// <summary>
    /// 设置使用状态
    /// </summary>
    /// <param name="isUsed">是否被使用</param>
    public void SetStatus(bool isUsed)
    {
        this.isUsed = isUsed;
        if (isUsed)
            SetColor(Defines.GetSlotColor(nowItem.data.slotColorType));
        else
            SetColor(Defines.validColor);
    }

    /// <summary>
    /// 设置物品格颜色
    /// </summary>
    /// <param name="color">颜色</param>
    public void SetColor(Color color)
    {
        nowColor = color;
        slotImg.color = color;
    }

    /// <summary>
    /// 隐藏图片
    /// </summary>
    public void HideImg()
    {
        slotImg.enabled = false;
    }

    /// <summary>
    /// 闪烁效果
    /// </summary>
    public void Flash()
    {
        StopAllCoroutines();
        if (gameObject.activeSelf)
            StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        slotImg.material.SetFloat("_FlashAmount", 1);
        slotImg.material.SetColor("_FlashColor", Color.yellow);
        yield return new WaitForSeconds(flashTime);
        slotImg.material.SetColor("_FlashColor", nowColor);
        slotImg.material.SetFloat("_FlashAmount", 0);
    }
}