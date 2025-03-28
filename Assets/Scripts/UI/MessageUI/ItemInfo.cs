using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public void SetInfo(Item item)
    {
        ItemSO data = item.data;
        List<ItemAttribute> nowAttributes = item.nowAttributes;
        List<BuffType> nowBuffTypes = item.nowItemBuffs;

        nowHeight = itemName.rectTransform.sizeDelta.y;
        itemName.text = data.itemChineseName;
        //类型
        string typeInfo = $"<color={Defines.blueColor}>物品类型：</color>";
        if (data.itemTags.Contains(ItemTag.Tower)) typeInfo += ColorTextTools.ColorText("【防御塔】", "#FF3030");
        else typeInfo += ColorTextTools.ColorText("【道具】", "#00B6FF");
        CreateAttributeInfo("Type", typeInfo);
        //塔
        if (data.itemTags.Contains(ItemTag.Tower))
        {
            string towerName = TowerManager.Instance.GetTowerSO_ByName(data.itemName).towerChineseName;
            string info =
                $"放置在背包中：可使用{ColorTextTools.ColorTextWithBrackets(towerName, Defines.redColor)}\n" +
                $"若重复放置：属性不叠加，但会参与触发联动效果";
            CreateAttributeInfo("TowerItem", info);
            string towerBuffInfo = $"<color={Defines.blueColor}>buff标签：</color>\n";
            foreach (BuffType buffType in nowBuffTypes)
            {
                switch (buffType)
                {
                    case BuffType.None:
                        break;
                    case BuffType.Burn:
                        towerBuffInfo += $"<color={Defines.redColor}>「灼烧」</color>\n";
                        break;
                    case BuffType.Slow:
                        towerBuffInfo += $"<color={Defines.cyanColor}>「缓慢」</color>\n";
                        break;
                    case BuffType.Stun:
                        towerBuffInfo += $"<color={Defines.greenColor}>「音震」</color>\n";
                        break;
                    case BuffType.Mark:
                        towerBuffInfo += $"<color={Defines.grayColor}>「印记」</color>\n";
                        break;
                }
            }
            if (towerBuffInfo != $"<color={Defines.blueColor}>buff标签：</color>\n")
                CreateAttributeInfo("TowerBuff", towerBuffInfo);

        }
        //物品属性
        if (data.itemAttributes.Count > 0)
        {
            string itemAttributeInfo = $"<color={Defines.blueColor}>基础属性：</color>\n";
            string itemAttributeInfo2 = $"<color={Defines.blueColor}>联动属性：</color>\n";
            //获取属性的描述
            if (nowAttributes == null) nowAttributes = data.itemAttributes;
            foreach (ItemAttribute attribute in nowAttributes)
            {
                if (attribute.attributeType == ItemAttribute.AttributeType.Global)
                    itemAttributeInfo += AttributeManager.Instance.GetAttributeDescription(attribute) + "\n";
                else
                    itemAttributeInfo2 += AttributeManager.Instance.GetAttributeDescription(attribute) + "\n";
            }
            //判空
            if (itemAttributeInfo != $"<color={Defines.blueColor}>基础属性：</color>\n")
                CreateAttributeInfo("GlobalAttribute", itemAttributeInfo);
            if (itemAttributeInfo2 != $"<color={Defines.blueColor}>联动属性：</color>\n")
                CreateAttributeInfo("LinkAttribute", itemAttributeInfo2);
        }
        //标签
        string tagInfo = $"<color={Defines.blueColor}>标签：</color>";
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
        attributeInfo.SetInfo(info);
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
