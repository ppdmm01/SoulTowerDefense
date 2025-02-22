using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerInfo : MonoBehaviour
{
    public RectTransform towerBaseInfoTrans; //获取防御塔基础信息高度
    public Image towerIcon; //防御塔图片
    public TextMeshProUGUI towerName; //防御塔名字
    public TextMeshProUGUI towerDescription; //防御塔描述
    private Dictionary<string,AttributeInfo> towerAttributes = new Dictionary<string, AttributeInfo>(); //防御塔属性信息
    public float nowHeight; //记录当前信息高度

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="data">防御塔数据</param>
    /// <param name="startHeight">信息面板起始高度</param>
    public void SetInfo(TowerData data, float startHeight)
    {
        (transform as RectTransform).anchoredPosition = new Vector2((transform as RectTransform).anchoredPosition.x, startHeight);

        towerIcon.sprite = data.towerIcon;
        towerName.text = data.towerChineseName;
        towerDescription.text = data.description;
        nowHeight = towerBaseInfoTrans.sizeDelta.y;
        //创建属性
        CreateAttributeInfo(nameof(data.hp),"血量：" + data.hp);
        CreateAttributeInfo(nameof(data.cost), "花费：" + data.cost);
        if (data.isAttacker)
        {
            CreateAttributeInfo(nameof(data.damage), "伤害：" + data.damage);
            CreateAttributeInfo(nameof(data.range), "攻击范围：" + data.range + "m");
            CreateAttributeInfo(nameof(data.interval), "攻击间隔：" + data.interval + "s");
        }
        if (data.isProducer)
        {
            CreateAttributeInfo(nameof(data.output), "产量：" + data.output + "/次");
            CreateAttributeInfo(nameof(data.cooldown), "冷却时间：" + data.cooldown + "s");
        }
        //更新背景高度
        (transform as RectTransform).sizeDelta = new Vector2((transform as RectTransform).sizeDelta.x, nowHeight+50);
    }

    /// <summary>
    /// 创建属性
    /// </summary>
    /// <param name="info">属性信息</param>
    private void CreateAttributeInfo(string name, string info)
    {
        AttributeInfo attributeInfo;
        if (!towerAttributes.ContainsKey(name))
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
            towerAttributes.Add(name, attributeInfo);
        }
        else
        {
            //获取属性信息条
            attributeInfo = towerAttributes[name];
        }
        //设置信息
        attributeInfo.SetInfo(info,48);
        //计算高度
        nowHeight += attributeInfo.GetHeight();
    }

    /// <summary>
    /// 移除所有属性
    /// </summary>
    public void RemoveAllAttributeInfo()
    {
        nowHeight = towerBaseInfoTrans.sizeDelta.y;
        foreach (var attributeInfo in towerAttributes.Values)
        {
            Destroy(attributeInfo.gameObject);
        }
        towerAttributes.Clear();
    }

    /// <summary>
    /// 获取防御塔信息高度
    /// </summary>
    /// <returns></returns>
    public float GetHeight()
    {
        return (transform as RectTransform).sizeDelta.y;
    }
}
