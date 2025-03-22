using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YangSuiTower : BaseTower
{
    [Header("投射物资源路径")]
    public string projectilePath = "Projectiles/FireBall";
    [Header("发射口")]
    public Transform firePos; //发射口

    //目前没动画，直接发射即可
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
            //寻找目标
            if (target == null && enemyList.Count > 0)
            {
                attackTimer -= Time.deltaTime;
                target = FindTarget();
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
        //设置方向
        Vector2 dir = target.transform.position - firePos.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        fireBall.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}
