using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.MPE;
using UnityEngine;

/// <summary>
/// 属性信息
/// </summary>
public class AttributeInfo : MonoBehaviour
{
    public TextMeshProUGUI infoTxt;
    public TextMeshProUGUI changedInfoTxt; //变化的信息
    public RectTransform rectTransform;

    /// <summary>
    /// 设置文本信息
    /// </summary>
    /// <param name="info">信息</param>
    /// <param name="fontSize">字体大小</param>
    public void SetInfo(string info,float fontSize = 36f)
    {
        infoTxt.text = info;
        infoTxt.fontSize = fontSize;
        changedInfoTxt.text = "";

        //计算文本高度
        float height = infoTxt.preferredHeight; //获取宽度受限时的首先高度
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height + 20); //20是文本框上下离父物体范围的距离
    }

    /// <summary>
    /// 设置变化的文本信息
    /// </summary>
    /// <param name="info">信息</param>
    /// <param name="changedInfo">变化的信息</param>
    /// <param name="fontSize">字体大小</param>
    public void SetChangedInfo(string info,string changedInfo, float fontSize = 36f)
    {
        SetInfo(info, fontSize);
        changedInfoTxt.fontSize = fontSize;
        changedInfoTxt.text = changedInfo;
        StartCoroutine(ChangeTxt(2f));
    }

    public IEnumerator ChangeTxt(float time)
    {
        yield return new WaitForSeconds(time);
        changedInfoTxt.gameObject.SetActive(false);
    }

    /// <summary>
    /// 获取高度
    /// </summary>
    public float GetHeight()
    {
        return rectTransform.sizeDelta.y;
    }
}
