using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ������Ϣ
/// </summary>
public class AttributeInfo : MonoBehaviour
{
    public TextMeshProUGUI infoTxt;
    public RectTransform rectTransform;

    /// <summary>
    /// �����ı���Ϣ
    /// </summary>
    /// <param name="info">��Ϣ</param>
    public void SetInfo(string info,float fontSize = 48f)
    {
        infoTxt.text = info;
        infoTxt.fontSize = fontSize;

        //�����ı��߶�
        float height = infoTxt.preferredHeight; //��ȡ�������ʱ�����ȸ߶�
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height + 20); //20���ı��������븸���巶Χ�ľ���
    }

    /// <summary>
    /// ��ȡ�߶�
    /// </summary>
    public float GetHeight()
    {
        return rectTransform.sizeDelta.y;
    }
}
