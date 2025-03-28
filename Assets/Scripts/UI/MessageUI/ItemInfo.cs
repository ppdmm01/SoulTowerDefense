using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// ��Ʒ������Ϣ
/// </summary>
public class ItemInfo : MonoBehaviour
{
    public TextMeshProUGUI itemName; //��Ʒ����
    private Dictionary<string, AttributeInfo> itemAttributes = new Dictionary<string, AttributeInfo>(); //��Ʒ������Ϣ
    public float nowHeight; //��¼��ǰ��Ϣ�߶�

    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="data">����������</param>
    public void SetInfo(Item item)
    {
        ItemSO data = item.data;
        List<ItemAttribute> nowAttributes = item.nowAttributes;
        List<BuffType> nowBuffTypes = item.nowItemBuffs;

        nowHeight = itemName.rectTransform.sizeDelta.y;
        itemName.text = data.itemChineseName;
        //����
        string typeInfo = $"<color={Defines.blueColor}>��Ʒ���ͣ�</color>";
        if (data.itemTags.Contains(ItemTag.Tower)) typeInfo += ColorTextTools.ColorText("����������", "#FF3030");
        else typeInfo += ColorTextTools.ColorText("�����ߡ�", "#00B6FF");
        CreateAttributeInfo("Type", typeInfo);
        //��
        if (data.itemTags.Contains(ItemTag.Tower))
        {
            string towerName = TowerManager.Instance.GetTowerSO_ByName(data.itemName).towerChineseName;
            string info =
                $"�����ڱ����У���ʹ��{ColorTextTools.ColorTextWithBrackets(towerName, Defines.redColor)}\n" +
                $"���ظ����ã����Բ����ӣ�������봥������Ч��";
            CreateAttributeInfo("TowerItem", info);
            string towerBuffInfo = $"<color={Defines.blueColor}>buff��ǩ��</color>\n";
            foreach (BuffType buffType in nowBuffTypes)
            {
                switch (buffType)
                {
                    case BuffType.None:
                        break;
                    case BuffType.Burn:
                        towerBuffInfo += $"<color={Defines.redColor}>�����ա�</color>\n";
                        break;
                    case BuffType.Slow:
                        towerBuffInfo += $"<color={Defines.cyanColor}>��������</color>\n";
                        break;
                    case BuffType.Stun:
                        towerBuffInfo += $"<color={Defines.greenColor}>������</color>\n";
                        break;
                    case BuffType.Mark:
                        towerBuffInfo += $"<color={Defines.grayColor}>��ӡ�ǡ�</color>\n";
                        break;
                }
            }
            if (towerBuffInfo != $"<color={Defines.blueColor}>buff��ǩ��</color>\n")
                CreateAttributeInfo("TowerBuff", towerBuffInfo);

        }
        //��Ʒ����
        if (data.itemAttributes.Count > 0)
        {
            string itemAttributeInfo = $"<color={Defines.blueColor}>�������ԣ�</color>\n";
            string itemAttributeInfo2 = $"<color={Defines.blueColor}>�������ԣ�</color>\n";
            //��ȡ���Ե�����
            if (nowAttributes == null) nowAttributes = data.itemAttributes;
            foreach (ItemAttribute attribute in nowAttributes)
            {
                if (attribute.attributeType == ItemAttribute.AttributeType.Global)
                    itemAttributeInfo += AttributeManager.Instance.GetAttributeDescription(attribute) + "\n";
                else
                    itemAttributeInfo2 += AttributeManager.Instance.GetAttributeDescription(attribute) + "\n";
            }
            //�п�
            if (itemAttributeInfo != $"<color={Defines.blueColor}>�������ԣ�</color>\n")
                CreateAttributeInfo("GlobalAttribute", itemAttributeInfo);
            if (itemAttributeInfo2 != $"<color={Defines.blueColor}>�������ԣ�</color>\n")
                CreateAttributeInfo("LinkAttribute", itemAttributeInfo2);
        }
        //��ǩ
        string tagInfo = $"<color={Defines.blueColor}>��ǩ��</color>";
        foreach (ItemTag tag in data.itemTags)
        {
            switch (tag)
            {
                case ItemTag.Force:
                    tagInfo += "������";
                    break;
                case ItemTag.Heat:
                    tagInfo += "���ȡ�";
                    break;
                case ItemTag.Light:
                    tagInfo += "���⡿";
                    break;
                case ItemTag.Sound:
                    tagInfo += "������";
                    break;
                case ItemTag.Book:
                    tagInfo += "���顿";
                    break;
                case ItemTag.Tower:
                    tagInfo += "������";
                    break;
            }
        }
        CreateAttributeInfo("itemTags", tagInfo);
        //���±����߶�
        (transform as RectTransform).sizeDelta = new Vector2((transform as RectTransform).sizeDelta.x, nowHeight + 50);
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="info">������Ϣ</param>
    private void CreateAttributeInfo(string name, string info)
    {
        AttributeInfo attributeInfo;
        if (!itemAttributes.ContainsKey(name))
        {
            //�������Զ������ò㼶
            GameObject attributeObj = Instantiate(Resources.Load<GameObject>("UI/UIObj/AttributeInfo"));
            attributeObj.transform.SetParent(transform, false);
            //����λ��
            RectTransform attributeTrans = attributeObj.transform as RectTransform;
            attributeTrans.anchoredPosition = new Vector2(attributeTrans.anchoredPosition.x, -nowHeight);
            //��ȡ������Ϣ��
            attributeInfo = attributeObj.GetComponent<AttributeInfo>();
            //��¼
            itemAttributes.Add(name, attributeInfo);
        }
        else
        {
            //��ȡ������Ϣ��
            attributeInfo = itemAttributes[name];
        }
        //������Ϣ
        attributeInfo.SetInfo(info);
        //����߶�
        nowHeight += attributeInfo.GetHeight();
    }

    /// <summary>
    /// �Ƴ���������
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
