using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基地核心
/// </summary>
public class Core : BaseTower
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.CompareTag("Enemy"))
        {
            //特效
            GameObject effObj = PoolMgr.Instance.GetObj("Effect/ExplosionEffect");
            effObj.transform.position = transform.position;

            //扣血
            Enemy enemy = collision.GetComponent<Enemy>();
            nowHp -= enemy.data.atk;
            PoolMgr.Instance.PushObj(enemy.gameObject);
        }
    }
}
