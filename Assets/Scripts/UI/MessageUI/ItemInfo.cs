using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 物品属性信息
/// </summary>
public class ItemInfo : MonoBehaviour
{
    public TextMeshProUGUI itemName; //物品名字
    private Dictionary<string, AttributeInfo> itemAttributes = new Dictionary<string, AttributeInfo>(); //物品属性信息
    public float nowHeight; //记录当前信息高度

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="data">防御塔数据</param>
    public void SetInfo(ItemSO data)
    {
        nowHeight = itemName.rectTransform.sizeDelta.y;
        itemName.text = data.itemChineseName;
        //类型
        string typeInfo;
        if (data.itemTags.Contains(ItemTag.Tower)) typeInfo = ColorTextTools.ColorText("【防御塔】", "#FF3030");
        else typeInfo = ColorTextTools.ColorText("【道具】", "yellow");
        CreateAttributeInfo("Type", "物品类型：" + typeInfo);
        //描述
        CreateAttributeInfo("Description", "物品描述：" + data.description);
        //塔
        if (data.itemTags.Contains(ItemTag.Tower))
        {
            string towerName = TowerManager.Instance.GetTowerSO_ByName(data.itemName).towerChineseName;
            string info =
                $"放置在背包中：可使用{ColorTextTools.ColorTextWithBrackets(towerName, "red")}\n" +
                $"若重复放置：属性不叠加，但会参与触发联动效果";
            CreateAttributeInfo("TowerItem", info);
        }
        //物品属性
        if (data.itemAttributes.Count > 0)
        {
            string itemAttributeInfo = "基础属性：\n";
            string itemAttributeInfo2 = "联动属性：\n";
            foreach (ItemAttribute attribute in data.itemAttributes)
            {
                if (attribute.attributeType == ItemAttribute.AttributeType.Global)
                    itemAttributeInfo += attribute.description + "\n";
                else
                    itemAttributeInfo2 += attribute.description + "\n";
            }
            CreateAttributeInfo("GlobalAttribute", itemAttributeInfo);
            CreateAttributeInfo("LinkAttribute", itemAttributeInfo2);
        }
        //标签
        string tagInfo = "标签：";
        foreach (ItemTag tag in data.itemTags)
        {
            switch (tag)
            {
                case ItemTag.Force:
                    tagInfo += "【力】";
                    break;
                case ItemTag.Heat:
                    tagInfo += "【热】";
                    break;
                case ItemTag.Light:
                    tagInfo += "【光】";
                    break;
                case ItemTag.Sound:
                    tagInfo += "【声】";
                    break;
                case ItemTag.Book:
                    tagInfo += "【书】";
                    break;
                case ItemTag.Tower:
                    tagInfo += "【塔】";
                    break;
            }
        }
        CreateAttributeInfo("itemTags", tagInfo);
        //更新背景高度
        (transform as RectTransform).sizeDelta = new Vector2((transform as RectTransform).sizeDelta.x, nowHeight + 50);
    }

    /// <summary>
    /// 创建属性
    /// </summary>
    /// <param name="info">属性信息</param>
    private void CreateAttributeInfo(string name, string info)
    {
        AttributeInfo attributeInfo;
        if (!itemAttributes.ContainsKey(name))
        {
            //创建属性对象并设置层级
            GameObject attributeObj = Instantiate(Resources.Load<GameObject>("UI/UIObj/AttributeInfo"));
            attributeObj.transform.SetParent(transform, false);
            //设置位置
            RectTransform attributeTrans = attributeObj.transform as RectTransform;
            attributeTrans.anchoredPosition = new Vector2(attributeTrans.anchoredPosition.x, -nowHeight);
            //获取属性信息条
            attributeInfo = attributeObj.GetComponent<AttributeInfo>();
            //记录
            itemAttributes.Add(name, attributeInfo);
        }
        else
        {
            //获取属性信息条
            attributeInfo = itemAttributes[name];
        }
        //设置信息
        attributeInfo.SetInfo(info, 36);
        //计算高度
        nowHeight += attributeInfo.GetHeight();
    }

    /// <summary>
    /// 移除所有属性
    /// </summary>
    public void RemoveAllAttributeInfo()
    {
        nowHeight = itemName.rectTransform.sizeDelta.y;
        foreach (var attributeInfo in itemAttributes.Values)
        {
            Destroy(attributeInfo.gameObject);
        }
        itemAttributes.Clear();
    }
}
