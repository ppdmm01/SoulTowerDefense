using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class GameRes
{
    public int soulResNum; //气资源数量
    public int taixuResNum; //太虚资源数量
    public int coreNowHp; //当前基地血量
    public int coreMaxHp; //当前基地最大血量
    public float gameTimeRes; //游戏时间资源
    public float startTime; //游戏开始时间

    public GameRes()
    {
        soulResNum = 0;
        taixuResNum = 0;
        coreNowHp = 0;
        coreMaxHp = 0;
        gameTimeRes = 0;
        startTime = 0;
    }
}

/// <summary>
/// 游戏资源管理器
/// </summary>
public class GameResManager : SingletonMono<GameResManager>
{
    public GameRes gameRes;
    private int oldTaixuNum; //记录上一次变化时太虚的量


    protected override void Awake()
    {
        base.Awake();
        gameRes = GameDataManager.Instance.gameResData;
        if (gameRes == null || gameRes.coreMaxHp == 0)
        {
            gameRes = new GameRes();
            ResetSoulNum();
            ResetTaixuNum();
            ResetCoreHp();
        }
        TopColumnPanel panel = UIManager.Instance.GetPanel<TopColumnPanel>();
        if (panel != null)
        {
            panel.UpdateHp(gameRes.coreNowHp,gameRes.coreMaxHp);
            panel.UpdateTaixuResNum(GetTaixuNum());
        }
        oldTaixuNum = -1;
    }

    public int GetSoulNum()
    {
        return gameRes.soulResNum;
    }

    public int GetTaixuNum()
    {
        return gameRes.taixuResNum;
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
        gameRes.soulResNum += num;
        TowerPanel panel = UIManager.Instance.GetPanel<TowerPanel>();
        if (panel != null)
            panel.UpdateSoulNum(gameRes.soulResNum);
        Save();
    }

    public void AddTaixuNum(int num)
    {
        StopAllCoroutines();
        if (oldTaixuNum == -1)
        {
            //-1说明从来没赋过值，需要记录老的值
            oldTaixuNum = gameRes.taixuResNum;
        }
        gameRes.taixuResNum += num;
        StartCoroutine(TaixuNumChangeRoutine(oldTaixuNum));
        //TopColumnPanel panel = UIManager.Instance.GetPanel<TopColumnPanel>();
        //if ( panel != null) 
        //    panel.UpdateTaixuResNum(gameRes.taixuResNum);
        Save();
    }

    public void ResetSoulNum()
    {
        gameRes.soulResNum = 0;
        TowerPanel panel = UIManager.Instance.GetPanel<TowerPanel>();
        if (panel != null)
            panel.UpdateSoulNum(gameRes.soulResNum);
        Save();
    }

    public void ResetTaixuNum()
    {
        gameRes.taixuResNum = 0;
        TopColumnPanel panel = UIManager.Instance.GetPanel<TopColumnPanel>();
        if (panel != null)
            panel.UpdateTaixuResNum(gameRes.taixuResNum);
        Save();
    }

    //重置血量
    public void ResetCoreHp()
    {
        gameRes.coreMaxHp = TowerManager.Instance.GetTowerSO_ByName("Core").hp;
        gameRes.coreNowHp = gameRes.coreMaxHp;
        Save();
    }

    public void UpdateCoreHp(int nowHp)
    {
        gameRes.coreNowHp = nowHp;
        Save();
    }

    public float GetStartTime()
    {
        return gameRes.startTime;
    }

    public float GetNowTime()
    {
        return gameRes.gameTimeRes;
    }

    public void ResetTime()
    {
        gameRes.gameTimeRes = 0;
        gameRes.startTime = 0;
    }

    public void SaveTime(float nowTime,float startTime)
    {
        gameRes.gameTimeRes = nowTime;
        gameRes.startTime = startTime;
        Save();
    }

    //重置所有资源（新游戏）
    public void ResetAllRes()
    {
        ResetCoreHp();
        ResetSoulNum();
        ResetTaixuNum();
        ResetTime();
        Save();
    }

    //保存
    public void Save()
    {
        GameDataManager.Instance.UpdateGameResData(gameRes);
        GameDataManager.Instance.SaveGameResData();
    }

    /// <summary>
    /// 太虚资源改变时的动画
    /// </summary>
    private IEnumerator TaixuNumChangeRoutine(int oldNum)
    {
        float timer = 0;
        TopColumnPanel panel = UIManager.Instance.GetPanel<TopColumnPanel>();
        while (timer <= 1f)
        {
            timer += Time.deltaTime;
            oldTaixuNum = (int)Mathf.Lerp(oldNum, gameRes.taixuResNum, timer / 1f);
            if (panel != null)
                panel.UpdateTaixuResNum(oldTaixuNum);
            yield return null;
        }
        //结束时记录为最终值
        oldTaixuNum = gameRes.taixuResNum;
        if (panel != null)
            panel.UpdateTaixuResNum(gameRes.taixuResNum);
    }
}
