using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// ���غ���
/// </summary>
public class Core : BaseTower
{
    public override void Wound(int dmg)
    {
        nowHp -= dmg;
        //��Ч
        //GameObject effObj = PoolMgr.Instance.GetObj("Effect/ExplosionEffect");
        //effObj.transform.position = transform.position;
        if (nowHp < 0) nowHp = 0;
        //����Ѫ��
        ShowHpBar();
        UpdateHpBar();
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