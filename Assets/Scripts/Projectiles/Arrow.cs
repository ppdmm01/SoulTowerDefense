using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    [SerializeField] private bool isAttack;
    /// <summary>
    /// ����
    /// </summary>
    public void Attack(Enemy enemy)
    {
        buffApplier.TryApplyBuff(enemy);
        enemy.Wound(damage,Color.yellow);
        //ɾ���Լ�
        PoolMgr.Instance.PushObj(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !isAttack)
        {
            isAttack = true;
            Attack(collision.GetComponent<Enemy>());
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isAttack = false;
    } 

    protected override void OnDisable()
    {
        base.OnDisable();
        isAttack = false;
    }
}
