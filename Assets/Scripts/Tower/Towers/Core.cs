using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// ���غ���
/// </summary>
public class Core : BaseTower
{
    //protected override void OnTriggerEnter2D(Collider2D collision)
    //{
    //    base.OnTriggerEnter2D(collision);

    //    if (collision.CompareTag("Enemy"))
    //    {
    //        //��Ч
    //        GameObject effObj = PoolMgr.Instance.GetObj("Effect/ExplosionEffect");
    //        effObj.transform.position = transform.position;

    //        //��Ѫ
    //        Enemy enemy = collision.GetComponent<Enemy>();
    //        nowHp -= enemy.data.atk;
    //        PoolMgr.Instance.PushObj(enemy.gameObject);
    //    }
    //}

    public override void SetHpBarPos(Vector2 pos)
    {
        //����Ҫ�ƶ�
    }

    public override void CreateHpBar()
    {
        //����Ѫ��
        HealthBar hpBar = UIManager.Instance.GetPanel<TowerPanel>().hpBar;
        hpBar.Init(nowHp, data.hp, Color.green, false);
        this.hpBar = hpBar;
    }

    public override void Wound(int dmg)
    {
        nowHp -= dmg;
        //��Ч
        GameObject effObj = PoolMgr.Instance.GetObj("Effect/ExplosionEffect");
        effObj.transform.position = transform.position;
        //����Ѫ��
        hpBar.UpdateHp(nowHp, data.hp);
        //����
        if (nowHp < 0)
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
            UIManager.Instance.ShowGameOverPanel(false);
    }
}