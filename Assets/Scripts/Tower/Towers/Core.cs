using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// ���غ���
/// </summary>
public class Core : BaseTower
{
    public override void Init(TowerData data)
    {
        base.Init(data);
        TopColumnPanel panel = UIManager.Instance.GetPanel<TopColumnPanel>();
        nowHp = GameResManager.Instance.gameRes.coreNowHp;
        panel.UpdateHp(nowHp,data.hp);
    }
    public override void Wound(int dmg,Enemy enemy = null)
    {
        nowHp -= dmg;
        if (nowHp < 0) nowHp = 0;
        //����Ѫ��
        ShowHpBar();
        UpdateHpBar();
        GameResManager.Instance.UpdateCoreHp(nowHp);
        UIManager.Instance.GetPanel<TopColumnPanel>()?.UpdateHp(nowHp,data.hp);
        //����
        if (nowHp <= 0)
        {
            Dead(); //��Ϸ����
        }
        //����
        Flash(0.1f, Color.white);
    }

    public override void Dead()
    {
        base.Dead();
        TowerManager.Instance.core = null;
        if (LevelManager.Instance.isInLevel == true) //��Ϸ��δ����ʱ����Ϸʧ�ܣ��Ѿ������˾������ò���
            LevelManager.Instance.LoseGame();
    }
}