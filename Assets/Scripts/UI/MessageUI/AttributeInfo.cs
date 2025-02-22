using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.MPE;
using UnityEngine;

/// <summary>
/// ������Ϣ
/// </summary>
public class AttributeInfo : MonoBehaviour
{
    public TextMeshProUGUI infoTxt;
    public TextMeshProUGUI changedInfoTxt; //�仯����Ϣ
    public RectTransform rectTransform;

    /// <summary>
    /// �����ı���Ϣ
    /// </summary>
    /// <param name="info">��Ϣ</param>
    /// <param name="fontSize">�����С</param>
    public void SetInfo(string info,float fontSize = 36f)
    {
        infoTxt.text = info;
        infoTxt.fontSize = fontSize;
        changedInfoTxt.text = "";

        //�����ı��߶�
        float height = infoTxt.preferredHeight; //��ȡ�������ʱ�����ȸ߶�
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height + 20); //20���ı��������븸���巶Χ�ľ���
    }

    /// <summary>
    /// ���ñ仯���ı���Ϣ
    /// </summary>
    /// <param name="info">��Ϣ</param>
    /// <param name="changedInfo">�仯����Ϣ</param>
    /// <param name="fontSize">�����С</param>
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
    /// ��ȡ�߶�
    /// </summary>
    public float GetHeight()
    {
        return rectTransform.sizeDelta.y;
    }
}
