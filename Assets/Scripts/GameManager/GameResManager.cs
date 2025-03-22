using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

/// <summary>
/// 游戏资源管理器
/// </summary>
public class GameResManager : Singleton<GameResManager>
{

    private int soulResNum; //气资源数量
    private int taixuResNum; //太虚资源数量

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
    /// 创建一个气
    /// </summary>
    /// <param name="soulNum">资源数量</param>
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
