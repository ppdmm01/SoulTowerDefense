using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

/// <summary>
/// ��Ϸ��Դ������
/// </summary>
public class GameResManager : Singleton<GameResManager>
{

    private int qiResNum; //����Դ����
    private int taixuResNum; //̫����Դ����

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
