using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerInfo : MonoBehaviour
{
    public RectTransform towerBaseInfoTrans; //��ȡ������������Ϣ�߶�
    public Image towerIcon; //������ͼƬ
    public TextMeshProUGUI towerName; //����������
    public TextMeshProUGUI towerDescription; //����������
    private Dictionary<string,AttributeInfo> towerAttributes = new Dictionary<string, AttributeInfo>(); //������������Ϣ
    public float nowHeight; //��¼��ǰ��Ϣ�߶�

    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="data">����������</param>
    /// <param name="startHeight">��Ϣ�����ʼ�߶�</param>
    public void SetInfo(TowerData data, float startHeight)
    {
        (transform as RectTransform).anchoredPosition = new Vector2((transform as RectTransform).anchoredPosition.x, startHeight);

        towerIcon.sprite = data.towerIcon;
        towerName.text = data.towerChineseName;
        towerDescription.text = data.description;
        nowHeight = towerBaseInfoTrans.sizeDelta.y;
        //��������
        CreateAttributeInfo(nameof(data.hp),"Ѫ����" + data.hp);
        CreateAttributeInfo(nameof(data.cost), "���ѣ�" + data.cost);
        if (data.isAttacker)
        {
            CreateAttributeInfo(nameof(data.damage), "�˺���" + data.damage);
            CreateAttributeInfo(nameof(data.range), "������Χ��" + data.range + "m");
            CreateAttributeInfo(nameof(data.interval), "���������" + data.interval + "s");
        }
        if (data.isProducer)
        {
            CreateAttributeInfo(nameof(data.output), "������" + data.output + "/��");
            CreateAttributeInfo(nameof(data.cooldown), "��ȴʱ�䣺" + data.cooldown + "s");
        }
        //���±����߶�
        (transform as RectTransform).sizeDelta = new Vector2((transform as RectTransform).sizeDelta.x, nowHeight+50);
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="info">������Ϣ</param>
    private void CreateAttributeInfo(string name, string info)
    {
        AttributeInfo attributeInfo;
        if (!towerAttributes.ContainsKey(name))
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
            towerAttributes.Add(name, attributeInfo);
        }
        else
        {
            //��ȡ������Ϣ��
            attributeInfo = towerAttributes[name];
        }
        //������Ϣ
        attributeInfo.SetInfo(info,48);
        //����߶�
        nowHeight += attributeInfo.GetHeight();
    }

    /// <summary>
    /// �Ƴ���������
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
    /// ��ȡ��������Ϣ�߶�
    /// </summary>
    /// <returns></returns>
    public float GetHeight()
    {
        return (transform as RectTransform).sizeDelta.y;
    }
}
