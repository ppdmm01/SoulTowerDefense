using System.Collections;
using System.Collections.Generic;
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
    public void SetInfo(ItemSO data)
    {
        nowHeight = itemName.rectTransform.sizeDelta.y;
        itemName.text = data.itemChineseName;
        //����
        string typeInfo;
        if (data.itemTags.Contains(ItemTag.Tower)) typeInfo = ColorTextTools.ColorText("����������", "#FF3030");
        else typeInfo = ColorTextTools.ColorText("�����ߡ�", "yellow");
        CreateAttributeInfo("Type", "��Ʒ���ͣ�" + typeInfo);
        //����
        CreateAttributeInfo("Description", "��Ʒ������" + data.description);
        //��
        if (data.itemTags.Contains(ItemTag.Tower))
        {
            string towerName = TowerManager.Instance.GetTowerSO_ByName(data.itemName).towerChineseName;
            string info =
                $"�����ڱ����У���ʹ��{ColorTextTools.ColorTextWithBrackets(towerName, "red")}\n" +
                $"���ظ����ã����Բ����ӣ�������봥������Ч��";
            CreateAttributeInfo("TowerItem", info);
        }
        //��Ʒ����
        if (data.itemAttributes.Count > 0)
        {
            string itemAttributeInfo = "�������ԣ�\n";
            string itemAttributeInfo2 = "�������ԣ�\n";
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
        //��ǩ
        string tagInfo = "��ǩ��";
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
        attributeInfo.SetInfo(info, 36);
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
