using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 顶部栏按钮类型
/// </summary>
public enum TopColumnBtnType
{
    Book, //书籍
    Map, //地图
    Menu, //菜单
    Crystal, //水晶
}

/// <summary>
/// 顶部栏面板
/// </summary>
public class TopColumnPanel : BasePanel
{
    [Header("文本相关")]
    public TextMeshProUGUI titleText; //标题文本
    public TextMeshProUGUI timerText; //计时器
    public TextMeshProUGUI taixuText; //太虚资源文本

    [Header("按钮相关")]
    public Transform btnContainer; //按钮容器
    public Button bookBtn; //图鉴按钮
    public Button mapBtn; //地图按钮
    public Button menuBtn; //菜单按钮
    public Button crystalBtn; //元气水晶按钮

    [Header("血条相关")]
    public HealthBar HpBar; //血条

    private float startTime; //计时器（到时放进管理器里）

    public override void Init()
    {
        startTime = Time.time;
        bookBtn.onClick.AddListener(() =>
        {
            //打开图鉴面板
        });
        mapBtn.onClick.AddListener(() =>
        {
            //打开地图面板
        });
        menuBtn.onClick.AddListener(() =>
        {
            //打开菜单面板
        });
        crystalBtn.onClick.AddListener(() =>
        {
            //打开元气水晶面板
        });
    }

    protected override void Update()
    {
        base.Update();
        UpdateTime();
    }

    //更新血条
    public void UpdateHp(int nowHp,int maxHp)
    {
        HpBar.UpdateHp(nowHp, maxHp);
    }

    //更新太虚数量文本
    public void UpdateTaixuResNum(int num)
    {
        taixuText.text = num.ToString();
    }

    //设置标题
    public void SetTitle(string title)
    {
        titleText.text = title;
    }

    //显示按钮
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

    //隐藏按钮
    public void HideBtn(params TopColumnBtnType[] btnTypes)
    {
        foreach (var type in btnTypes)
        {
            switch (type)
            {
                case TopColumnBtnType.Book:
                    bookBtn.gameObject.SetActive(false);
                    bookBtn.transform.SetParent(transform); //将父对象设为面板子对象，断开原来与容器的父子关系
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

    //更新时间
    private void UpdateTime()
    {
        int duration = (int)(Time.time - startTime);
        int hour = duration / 3600;
        int min = duration / 60;
        int sec = duration % 60;
        string txt = "";

        //时
        if (hour > 0)
        {
            if (hour < 10) txt += $"0{hour}:";
            else txt += $"{hour}:";
        }
        //分
        if (min > 0)
        {
            if (min < 10) txt += $"0{min}:";
            else txt += $"{min}:";
        }
        else
        {
            txt += "00:";
        }
        //秒
        if (sec < 10) txt += $"0{sec}";
        else txt += sec;

        timerText.text = txt;
    }
}
