using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 属性信息
/// </summary>
public class AttributeInfo : MonoBehaviour
{
    public TextMeshProUGUI infoTxt;
    public RectTransform rectTransform;

    /// <summary>
    /// 设置文本信息
    /// </summary>
    /// <param name="info">信息</param>
    public void SetInfo(string info,float fontSize = 48f)
    {
        infoTxt.text = info;
        infoTxt.fontSize = fontSize;

        //计算文本高度
        float height = infoTxt.preferredHeight; //获取宽度受限时的首先高度
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height + 20); //20是文本框上下离父物体范围的距离
    }

    /// <summary>
    /// 获取高度
    /// </summary>
    public float GetHeight()
    {
        return rectTransform.sizeDelta.y;
    }
}
