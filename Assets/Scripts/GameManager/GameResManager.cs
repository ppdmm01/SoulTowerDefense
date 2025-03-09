using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

/// <summary>
/// 游戏资源管理器
/// </summary>
public class GameResManager : Singleton<GameResManager>
{

    private int qiResNum; //气资源数量
    private int taixuResNum; //太虚资源数量

    private GameResManager()
    {
        ResetQiNum();
    }

    public int GetQiNum()
    {
        return qiResNum;
    }

    public int GetTaixuNum()
    {
        return taixuResNum;
    }

    public void AddQiNum(int num)
    {
        qiResNum += num;
        TowerPanel panel = UIManager.Instance.GetPanel<TowerPanel>();
        if (panel != null)
            panel.UpdateQiNum(qiResNum);
    }

    public void AddTaixuNum(int num)
    {
        taixuResNum += num;
        TopColumnPanel panel = UIManager.Instance.GetPanel<TopColumnPanel>();
        if ( panel != null) 
            panel.UpdateTaixuResNum(taixuResNum);
    }

    public void ResetQiNum()
    {
        qiResNum = 0;
        TowerPanel panel = UIManager.Instance.GetPanel<TowerPanel>();
        if (panel != null)
            panel.UpdateQiNum(qiResNum);
    }

    public void ResetTaixuNum()
    {
        taixuResNum = 0;
        TopColumnPanel panel = UIManager.Instance.GetPanel<TopColumnPanel>();
        if (panel != null)
            panel.UpdateTaixuResNum(taixuResNum);
    }
}
