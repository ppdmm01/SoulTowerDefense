using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDragon : Projectile
{
    public Transform explosionPos; //��ըλ��
    /// <summary>
    /// ����
    /// </summary>
    public void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRange, 1 << LayerMask.NameToLayer("Enemy"));
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                buffApplier.TryApplyBuff(enemy);
                enemy.Wound(damage,Color.yellow);
            }
        }
        AudioManager.Instance.PlaySound("SoundEffect/Explosion");
        //��Ч
        EffectManager.Instance.PlayEffect("Explode", explosionPos.position,1.1f);
        //ɾ���Լ�
        PoolMgr.Instance.PushObj(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Attack();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
