using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��������ť����
/// </summary>
public enum TopColumnBtnType
{
    Book, //�鼮
    Map, //��ͼ
    Menu, //�˵�
    Crystal, //ˮ��
}

/// <summary>
/// ���������
/// </summary>
public class TopColumnPanel : BasePanel
{
    [Header("�ı����")]
    public TextMeshProUGUI titleText; //�����ı�
    public TextMeshProUGUI timerText; //��ʱ��
    public TextMeshProUGUI taixuText; //̫����Դ�ı�

    [Header("��ť���")]
    public Transform btnContainer; //��ť����
    public Button bookBtn; //ͼ����ť
    public Button mapBtn; //��ͼ��ť
    public Button menuBtn; //�˵���ť
    public Button crystalBtn; //Ԫ��ˮ����ť

    [Header("Ѫ�����")]
    public HealthBar HpBar; //Ѫ��

    private float startTime; //��ʱ������ʱ�Ž��������

    public override void Init()
    {
        startTime = Time.time;
        bookBtn.onClick.AddListener(() =>
        {
            //��ͼ�����
        });
        mapBtn.onClick.AddListener(() =>
        {
            //�򿪵�ͼ���
        });
        menuBtn.onClick.AddListener(() =>
        {
            //�򿪲˵����
        });
        crystalBtn.onClick.AddListener(() =>
        {
            //��Ԫ��ˮ�����
        });
    }

    protected override void Update()
    {
        base.Update();
        UpdateTime();
    }

    //����Ѫ��
    public void UpdateHp(int nowHp,int maxHp)
    {
        HpBar.UpdateHp(nowHp, maxHp);
    }

    //����̫�������ı�
    public void UpdateTaixuResNum(int num)
    {
        taixuText.text = num.ToString();
    }

    //���ñ���
    public void SetTitle(string title)
    {
        titleText.text = title;
    }

    //��ʾ��ť
    public void ShowBtn(params TopColumnBtnType[] btnTypes)
    {
        HideBtn(TopColumnBtnType.Book, TopColumnBtnType.Crystal, TopColumnBtnType.Menu, TopColumnBtnType.Map);
        foreach (var type in btnTypes)
        {
            switch (type)
            {
                case TopColumnBtnType.Book:
                    bookBtn.gameObject.SetActive(true);
                    bookBtn.transform.SetParent(btnContainer);
                    break;
                case TopColumnBtnType.Map:
                    mapBtn.gameObject.SetActive(true);
                    mapBtn.transform.SetParent(btnContainer);
                    break;
                case TopColumnBtnType.Menu:
                    menuBtn.gameObject.SetActive(true);
                    menuBtn.transform.SetParent(btnContainer);
                    break;
                case TopColumnBtnType.Crystal:
                    crystalBtn.gameObject.SetActive(true);
                    crystalBtn.transform.SetParent(btnContainer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    //���ذ�ť
    public void HideBtn(params TopColumnBtnType[] btnTypes)
    {
        foreach (var type in btnTypes)
        {
            switch (type)
            {
                case TopColumnBtnType.Book:
                    bookBtn.gameObject.SetActive(false);
                    bookBtn.transform.SetParent(transform); //����������Ϊ����Ӷ��󣬶Ͽ�ԭ���������ĸ��ӹ�ϵ
                    break;
                case TopColumnBtnType.Map:
                    mapBtn.gameObject.SetActive(false);
                    mapBtn.transform.SetParent(transform);
                    break;
                case TopColumnBtnType.Menu:
                    menuBtn.gameObject.SetActive(false);
                    menuBtn.transform.SetParent(transform);
                    break;
                case TopColumnBtnType.Crystal:
                    crystalBtn.gameObject.SetActive(false);
                    crystalBtn.transform.SetParent(transform);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    //����ʱ��
    private void UpdateTime()
    {
        int duration = (int)(Time.time - startTime);
        int hour = duration / 3600;
        int min = duration / 60;
        int sec = duration % 60;
        string txt = "";

        //ʱ
        if (hour > 0)
        {
            if (hour < 10) txt += $"0{hour}:";
            else txt += $"{hour}:";
        }
        //��
        if (min > 0)
        {
            if (min < 10) txt += $"0{min}:";
            else txt += $"{min}:";
        }
        else
        {
            txt += "00:";
        }
        //��
        if (sec < 10) txt += $"0{sec}";
        else txt += sec;

        timerText.text = txt;
    }
}
