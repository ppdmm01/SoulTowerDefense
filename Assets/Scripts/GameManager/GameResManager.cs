using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

/// <summary>
/// ��Ϸ��Դ������
/// </summary>
public class GameResManager : Singleton<GameResManager>
{

    private int soulResNum; //����Դ����
    private int taixuResNum; //̫����Դ����

    private GameResManager()
    {
        ResetSoulNum();
    }

    public int GetSoulNum()
    {
        return soulResNum;
    }

    public int GetTaixuNum()
    {
        return taixuResNum;
    }

    /// <summary>
    /// ����һ����
    /// </summary>
    /// <param name="soulNum">��Դ����</param>
    public void CreateOneSoul(int soulNum,Vector2 pos)
    {
        GameObject obj = PoolMgr.Instance.GetObj("GameRes/Soul");
        if (obj != null)
        {
            obj.transform.position = pos;
            obj.GetComponent<Soul>().Init(soulNum);
        }
    }

    public void AddSoulNum(int num)
    {
        soulResNum += num;
        TowerPanel panel = UIManager.Instance.GetPanel<TowerPanel>();
        if (panel != null)
            panel.UpdateSoulNum(soulResNum);
    }

    public void AddTaixuNum(int num)
    {
        taixuResNum += num;
        TopColumnPanel panel = UIManager.Instance.GetPanel<TopColumnPanel>();
        if ( panel != null) 
            panel.UpdateTaixuResNum(taixuResNum);
    }

    public void ResetSoulNum()
    {
        soulResNum = 0;
        TowerPanel panel = UIManager.Instance.GetPanel<TowerPanel>();
        if (panel != null)
            panel.UpdateSoulNum(soulResNum);
    }

    public void ResetTaixuNum()
    {
        taixuResNum = 0;
        TopColumnPanel panel = UIManager.Instance.GetPanel<TopColumnPanel>();
        if (panel != null)
            panel.UpdateTaixuResNum(taixuResNum);
    }
}
