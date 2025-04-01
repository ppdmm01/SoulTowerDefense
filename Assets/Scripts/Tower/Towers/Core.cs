using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// 基地核心
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
        //更新血条
        ShowHpBar();
        UpdateHpBar();
        GameResManager.Instance.UpdateCoreHp(nowHp);
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