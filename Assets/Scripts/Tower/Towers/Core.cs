using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// 基地核心
/// </summary>
public class Core : BaseTower
{
    public override void Wound(int dmg)
    {
        nowHp -= dmg;
        //特效
        //GameObject effObj = PoolMgr.Instance.GetObj("Effect/ExplosionEffect");
        //effObj.transform.position = transform.position;
        if (nowHp < 0) nowHp = 0;
        //更新血条
        ShowHpBar();
        UpdateHpBar();
        UIManager.Instance.GetPanel<TopColumnPanel>()?.UpdateHp(nowHp,data.hp);
        //死亡
        if (nowHp <= 0)
        {
            Dead(); //游戏结束
        }
        //闪白
        Flash(0.1f, Color.white);
    }

    public override void Dead()
    {
        base.Dead();
        TowerManager.Instance.core = null;
        if (LevelManager.Instance.isInLevel == true) //游戏还未结束时，游戏失败，已经结束了就跳过该步骤
            LevelManager.Instance.LoseGame();
    }
}