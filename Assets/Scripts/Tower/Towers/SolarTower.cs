using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarTower : BaseTower
{
    [Header("投射物资源路径")]
    public string projectilePath = "Projectiles/FireBall";
    [Header("发射口")]
    public Transform firePos; //发射口

    protected override void Update()
    {
        if (!isUsed) return;

        attackTimer += Time.deltaTime;
        produceTimer += Time.deltaTime;

        CheckEnemy();

        if (data.isAttacker)
        {
            //寻找目标
            if (target == null && enemyList.Count > 0)
            {
                attackTimer -= Time.deltaTime;
                target = FindTarget();
            }

            //转向
            if (target != null && launcher != null)
            {
                Vector2 dir = target.position - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                launcher.rotation = Quaternion.Slerp(launcher.rotation, Quaternion.Euler(0, 0, angle - 90), Time.deltaTime * 10);
            }

            //攻击
            if (target != null && attackTimer > data.interval)
            {
                attackTimer = 0;
                Attack();
            }
        }

        //更新血条位置
        if (hpBar != null && hpBar.gameObject.activeSelf)
            SetHpBarPos(transform.position + Vector3.up);
    }

    /// <summary>
    /// 创建投射物（在动画事件中调用）
    /// </summary>
    public void CreateProjectile()
    {
        AudioManager.Instance.PlaySound("SoundEffect/BowAttack");
        GameObject fireBallObj = PoolMgr.Instance.GetObj(projectilePath);
        FireBall fireBall = fireBallObj.GetComponent<FireBall>();
        fireBall.Init(this.buffApplier, data.damage);
        fireBall.transform.position = firePos.position;
        fireBall.transform.rotation = launcher.rotation;
    }
}
