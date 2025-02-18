using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���غ���
/// </summary>
public class Core : BaseTower
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.CompareTag("Enemy"))
        {
            //��Ч
            GameObject effObj = PoolMgr.Instance.GetObj("Effect/ExplosionEffect");
            effObj.transform.position = transform.position;

            //��Ѫ
            Enemy enemy = collision.GetComponent<Enemy>();
            nowHp -= enemy.data.atk;
            PoolMgr.Instance.PushObj(enemy.gameObject);
        }
    }
}
