using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ��Դ������
/// </summary>
public class GameResManager : Singleton<GameResManager>
{
    private GameResManager()
    {
        ResetQiNum();
    }

    private int qiResNum; //����Դ����

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
