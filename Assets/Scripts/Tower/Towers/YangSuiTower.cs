using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YangSuiTower : BaseTower
{
    [Header("Ͷ������Դ·��")]
    public string projectilePath = "Projectiles/FireBall";
    [Header("�����")]
    public Transform firePos; //�����

    //Ŀǰû������ֱ�ӷ��伴��
    public override void Attack()
    {
        CreateProjectile();
    }

    protected override void Update()
    {
        if (!isUsed) return;

        attackTimer += Time.deltaTime;
        produceTimer += Time.deltaTime;

        CheckEnemy();

        if (data.isAttacker)
        {
            //Ѱ��Ŀ��
            if (target == null && enemyList.Count > 0)
            {
                attackTimer -= Time.deltaTime;
                target = FindTarget();
            }

            //����
            if (target != null && attackTimer > data.interval)
            {
                attackTimer = 0;
                Attack();
            }
        }

        //����Ѫ��λ��
        if (hpBar != null && hpBar.gameObject.activeSelf)
            SetHpBarPos(transform.position + Vector3.up);
    }

    /// <summary>
    /// ����Ͷ����ڶ����¼��е��ã�
    /// </summary>
    public void CreateProjectile()
    {
        AudioManager.Instance.PlaySound("SoundEffect/BowAttack");
        GameObject fireBallObj = PoolMgr.Instance.GetObj(projectilePath);
        FireBall fireBall = fireBallObj.GetComponent<FireBall>();

        fireBall.Init(this.buffApplier, data.damage);
        fireBall.transform.position = firePos.position;
        //���÷���
        Vector2 dir = target.transform.position - firePos.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        fireBall.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}
