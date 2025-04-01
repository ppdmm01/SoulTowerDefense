using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class GameRes
{
    public int soulResNum; //����Դ����
    public int taixuResNum; //̫����Դ����
    public int coreNowHp; //��ǰ����Ѫ��
    public int coreMaxHp; //��ǰ�������Ѫ��
    public float gameTimeRes; //��Ϸʱ����Դ
    public float startTime; //��Ϸ��ʼʱ��

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
/// ��Ϸ��Դ������
/// </summary>
public class GameResManager : SingletonMono<GameResManager>
{
    public GameRes gameRes;
    private int oldTaixuNum; //��¼��һ�α仯ʱ̫�����


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
            //-1˵������û����ֵ����Ҫ��¼�ϵ�ֵ
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

    //����Ѫ��
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

    //����������Դ������Ϸ��
    public void ResetAllRes()
    {
        ResetCoreHp();
        ResetSoulNum();
        ResetTaixuNum();
        ResetTime();
        Save();
    }

    //����
    public void Save()
    {
        GameDataManager.Instance.UpdateGameResData(gameRes);
        GameDataManager.Instance.SaveGameResData();
    }

    /// <summary>
    /// ̫����Դ�ı�ʱ�Ķ���
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
        //����ʱ��¼Ϊ����ֵ
        oldTaixuNum = gameRes.taixuResNum;
        if (panel != null)
            panel.UpdateTaixuResNum(gameRes.taixuResNum);
    }
}
