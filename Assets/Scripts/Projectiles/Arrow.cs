using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    /// <summary>
    /// ����
    /// </summary>
    public void Attack(Enemy enemy)
    {
        enemy.Wound(damage);
        //ɾ���Լ�
        PoolMgr.Instance.PushObj(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Attack(collision.GetComponent<Enemy>());
        }
    }
}
