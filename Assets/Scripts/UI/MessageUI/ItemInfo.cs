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
    public void SetInfo(ItemSO data,List<ItemAttribute> nowAttributes)
    {
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
                $"�����ڱ����У���ʹ��{ColorTextTools.ColorTextWithBrackets(towerName, "red")}\n" +
                $"���ظ����ã����Բ����ӣ�������봥������Ч��";
            CreateAttributeInfo("TowerItem", info);
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
                    itemAttributeInfo += GetAttributeDescription(attribute) + "\n";
                else
                    itemAttributeInfo2 += GetAttributeDescription(attribute) + "\n";
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

    /// <summary>
    /// ��ȡ����Ч��������
    /// </summary>
    public string GetAttributeDescription(ItemAttribute attribute)
    {
        string description = "";
        ItemActiveCondition condition = attribute.condition;
        //��������
        switch (attribute.attributeType)
        {
            case ItemAttribute.AttributeType.Global:
                description += "����";
                break;
            case ItemAttribute.AttributeType.Link:
                switch (condition.pointType)
                {
                    case DetectionPoint.PointType.Star:
                        description += "���Ǵ�����";
                        break;
                    case DetectionPoint.PointType.Fire:
                        description += "���津����";
                        break;
                }
                break;
        }
        //�����ı�ǩ������
        ItemTag[] tags = condition.tags;
        switch (condition.conditionType)
        {
            case ItemActiveCondition.ConditionType.Tag:
                //Ŀǰֻ���Ƿ�����Ч��
                if (tags.Contains(ItemTag.Tower))
                {
                    if (tags.Contains(ItemTag.Force))
                    {
                        description += "��ѧ����";
                    }
                    else if (tags.Contains(ItemTag.Heat))
                    {
                        description += "��ѧ����";
                    }
                    else if (tags.Contains(ItemTag.Light))
                    {
                        description += "��ѧ����";
                    }
                }
                break;
            case ItemActiveCondition.ConditionType.Name:
                description += condition.name + "��";
                break;
        }
        //����Ч��
        string growStr = ""; //��¼�ɳ�Ч��
        for (int i = 0;i < attribute.activeEffects.Count();i++)
        {
            ItemActiveEffect effect = attribute.activeEffects[i];
            switch (effect.effectType)
            {
                case ItemActiveEffect.EffectType.Hp:
                    description += $"<color={Defines.purpleColor}>Ѫ��+{Mathf.RoundToInt(effect.value)}</color>";                   
                    growStr += $"<color={Defines.purpleColor}>Ѫ��+{Mathf.RoundToInt(effect.growValue)}</color>";
                    break;
                case ItemActiveEffect.EffectType.Cost:
                    description += $"<color={Defines.purpleColor}>����-{Mathf.RoundToInt(effect.value)}</color>";
                    growStr += $"<color={Defines.purpleColor}>����-{Mathf.RoundToInt(effect.growValue)}</color>";
                    break;
                case ItemActiveEffect.EffectType.Output:
                    description += $"<color={Defines.purpleColor}>����+{Mathf.RoundToInt(effect.value)}</color>";
                    growStr += $"<color={Defines.purpleColor}>����+{Mathf.RoundToInt(effect.growValue)}</color>";
                    break;
                case ItemActiveEffect.EffectType.Cooldown:
                    description += $"<color={Defines.purpleColor}>������ȴ-{Mathf.RoundToInt(effect.value)}s</color>";
                    growStr += $"<color={Defines.purpleColor}>������ȴ-{Mathf.RoundToInt(effect.growValue)}s</color>";
                    break;
                case ItemActiveEffect.EffectType.DamageMultiplier:
                    description += $"<color={Defines.purpleColor}>�����˺�+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.purpleColor}>�����˺�+{Mathf.RoundToInt(effect.growValue * 100)}%</color>";
                    break;
                case ItemActiveEffect.EffectType.RangeMultiplier:
                    description += $"<color={Defines.purpleColor}>���+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.purpleColor}>���+{Mathf.RoundToInt(effect.growValue * 100)}%</color>";
                    break;
                case ItemActiveEffect.EffectType.IntervalMultiplier:
                    description += $"<color={Defines.purpleColor}>������ȴ-{Mathf.Abs(Mathf.RoundToInt(effect.value * 100))}%</color>";
                    growStr += $"<color={Defines.purpleColor}>������ȴ-{Mathf.Abs(Mathf.RoundToInt(effect.growValue * 100))}%</color>";
                    break;
                case ItemActiveEffect.EffectType.BurnBuff_Duration:
                    description += $"<color={Defines.redColor}>�����ա�</color>" +
                        $"<color={Defines.purpleColor}>����ʱ��+{effect.value.ToString("F1")}s</color>";
                    growStr += $"<color={Defines.redColor}>�����ա�</color>" +
                        $"<color={Defines.purpleColor}>����ʱ��+{effect.growValue.ToString("F1")}s</color>";
                    break;
                case ItemActiveEffect.EffectType.BurnBuff_Damage:
                    description += $"<color={Defines.redColor}>�����ա�</color>" +
                        $"<color={Defines.purpleColor}>�˺�+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.redColor}>�����ա�</color>" +
                        $"<color={Defines.purpleColor}>�˺�+{Mathf.RoundToInt(effect.growValue * 100)}%</color>";
                    break;
                case ItemActiveEffect.EffectType.BurnBuff_TriggerChance:
                    description += $"<color={Defines.redColor}>�����ա�</color>" +
                        $"<color={Defines.purpleColor}>��������+{effect.value.ToString("F1")}%</color>";
                    growStr += $"<color={Defines.redColor}>�����ա�</color>" +
                        $"<color={Defines.purpleColor}>��������+{effect.growValue.ToString("F1")}%</color>";
                    break;
                case ItemActiveEffect.EffectType.SlowBuff_Duration:
                    description += $"<color={Defines.greenColor}>��������</color>" +
                        $"<color={Defines.purpleColor}>����ʱ��+{effect.value.ToString("F1")}s</color>";
                    growStr += $"<color={Defines.greenColor}>��������</color>" +
                        $"<color={Defines.purpleColor}>����ʱ��+{effect.growValue.ToString("F1")}s</color>";
                    break;
                case ItemActiveEffect.EffectType.SlowBuff_TriggerChance:
                    description += $"<color={Defines.greenColor}>��������</color>" +
                        $"<color={Defines.purpleColor}>��������+{Mathf.RoundToInt(effect.value * 100)}%</color>";
                    growStr += $"<color={Defines.greenColor}>��������</color>" +
                        $"<color={Defines.purpleColor}>��������+{Mathf.RoundToInt(effect.growValue * 100)}%</color>";
                    break;
                default:
                    break;
            }
            if (i != attribute.activeEffects.Count() - 1)
            {
                description += "��";
                growStr += "��";
            }
            if (i == attribute.activeEffects.Count() - 1 && attribute.isGrow)
            {
                switch (attribute.growType)
                {
                    case ItemAttribute.GrowType.All:
                        description += $"\n(ÿ����һ����ͼ�ڵ㣺{growStr})" +
                            $"<color={Defines.purpleColor}> <�ѳɳ�{attribute.growTime}��></color>";
                        break;
                    case ItemAttribute.GrowType.Random:
                        description += $"\n(ÿ����һ����ͼ�ڵ㣬�������������һЧ����{growStr})" +
                            $"<color={Defines.purpleColor}> <�ѳɳ�{attribute.growTime}��></color>";
                        break;
                    default:
                        break;
                }
            }
        }
        return description;
    }
}
