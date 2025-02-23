using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// 基地核心
/// </summary>
public class Core : BaseTower
{
    //protected override void OnTriggerEnter2D(Collider2D collision)
    //{
    //    base.OnTriggerEnter2D(collision);

    //    if (collision.CompareTag("Enemy"))
    //    {
    //        //特效
    //        GameObject effObj = PoolMgr.Instance.GetObj("Effect/ExplosionEffect");
    //        effObj.transform.position = transform.position;

    //        //扣血
    //        Enemy enemy = collision.GetComponent<Enemy>();
    //        nowHp -= enemy.data.atk;
    //        PoolMgr.Instance.PushObj(enemy.gameObject);
    //    }
    //}

    public override void SetHpBarPos(Vector2 pos)
    {
        //不需要移动
    }

    public override void CreateHpBar()
    {
        //创建血条
        HealthBar hpBar = UIManager.Instance.GetPanel<TowerPanel>().hpBar;
        hpBar.Init(nowHp, data.hp, Color.green, false);
        this.hpBar = hpBar;
    }

    public override void Wound(int dmg)
    {
        nowHp -= dmg;
        //特效
        GameObject effObj = PoolMgr.Instance.GetObj("Effect/ExplosionEffect");
        effObj.transform.position = transform.position;
        //更新血条
        hpBar.UpdateHp(nowHp, data.hp);
        //死亡
        if (nowHp < 0)
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
            UIManager.Instance.ShowGameOverPanel(false);
    }
}