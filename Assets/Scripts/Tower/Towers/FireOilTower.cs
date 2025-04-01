using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireOilTower : BaseTower
{
    [Header("火焰范围")]
    public float angle = 60f; //整个扇形角度
    public GameObject fireObj; //火焰物体
    private bool isFire; //是否在喷火


    protected override void Update()
    {
        base.Update();
        if (isFire) attackTimer = 0; //喷火时不计时
    }

    public void AttackStart()
    {
        isFire = true;
        AudioManager.Instance.PlaySound("SoundEffect/BowAttack"); //火焰的声音
    }

    public void AttackEnd()
    {
        isFire = false;
    }

    public override void Init(TowerData data)
    {
        base.Init(data);
        //初始化火焰扇形大小（目前火焰半径为1个scale单位对应2.5格）
        float scaleMultiplier = data.range / 2.5f;
        fireObj.transform.localScale = scaleMultiplier * Vector3.one;
    }
    /// <summary>
    /// 伤害
    /// </summary>
    public void Hit()
    {
        List<Enemy> enemies = FindWoundEnemies();
        if (enemies == null) return;
        foreach (Enemy enemy in enemies)
        {
            buffApplier.TryApplyBuff(enemy);
            enemy.Wound(data.damage,Color.yellow);
        }
    }

    /// <summary>
    /// 寻找火焰范围内的敌人
    /// </summary>
    public List<Enemy> FindWoundEnemies()
    {
        if (target == null) FindTarget();
        if (target == null) return null;
        List<Enemy> enemies = new List<Enemy>();
        Vector2 fireDir = (target.transform.position - transform.position).normalized; //火焰喷射方向
        foreach (Transform enemyTrans in enemyList)
        {
            Vector2 enemyDir = (enemyTrans.position - transform.position).normalized; //敌人方向
            float angle = Vector2.Angle(enemyDir, fireDir);
            if (angle <= (this.angle/2f)) enemies.Add(enemyTrans.GetComponent<Enemy>());
        }
        return enemies;
        
    }
}
