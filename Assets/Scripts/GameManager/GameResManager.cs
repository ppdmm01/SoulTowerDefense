using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏资源管理器
/// </summary>
public class GameResManager : Singleton<GameResManager>
{
    private GameResManager()
    {
        ResetQiNum();
    }

    private int qiResNum; //气资源数量

    public int GetQiNum()
    {
        return qiResNum;
    }

    public void AddQiNum(int num)
    {
        qiResNum += num;
        TowerPanel panel = UIManager.Instance.GetPanel<TowerPanel>();
        if (panel != null)
            panel.UpdateQiNum(qiResNum);
    }

    public void ResetQiNum()
    {
        qiResNum = 0;
        TowerPanel panel = UIManager.Instance.GetPanel<TowerPanel>();
        if (panel != null)
            panel.UpdateQiNum(qiResNum);
    }
}
