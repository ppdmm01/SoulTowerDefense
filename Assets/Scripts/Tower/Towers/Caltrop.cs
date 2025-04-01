using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caltrop : BaseTower
{
    public override void Attack()
    {
        if (enemyList.Count == 0) return;
        ani.Play("Attack"); //¶¯»­ÊÂ¼ş´¥·¢¹¥»÷
    }

    public void CaltropAttack()
    {
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            Enemy enemy = enemyList[i].GetComponent<Enemy>();
            if (enemy != null)
            {
                buffApplier.TryApplyBuff(enemy);
                enemy.Wound(data.damage, Color.yellow);
            }
        }
        AudioManager.Instance.PlaySound("SoundEffect/CaltropAttack");
    }

    //public override void Init(TowerData data)
    //{
    //    this.data = data;
    //    isDead = false;

    //    enemyList = new List<Transform>();
    //    ani = GetComponent<Animator>();

    //    //»ñÈ¡·ÀÓùËşµÄbuff
    //    buffApplier = new BuffApplier(data.buffDatas);

    //    //ÉÁ°×²ÄÖÊ
    //    Material material = Resources.Load<Material>("Material/FlashMaterial");
    //    foreach (SpriteRenderer renderer in renderers)
    //    {
    //        if (renderer.material != material)
    //            renderer.material = material;
    //    }

    //    attackTimer = 0;
    //    produceTimer = 0;
    //    nowHp = data.hp;

    //    target = null;

    //    //·ÀÓùËş·¶Î§
    //    rangeTrigger.radius = data.range;
    //    rangeObj.transform.localScale = Vector3.one * (data.range * 2);
    //    rangeObj.SetActive(false);
    //}

    protected override void Update()
    {
        if (!isUsed) return;

        attackTimer += Time.deltaTime;
        produceTimer += Time.deltaTime;

        CheckEnemy();

        if (data.isAttacker)
        {
            //Ñ°ÕÒÄ¿±ê
            if (target == null && enemyList.Count > 0)
            {
                attackTimer -= Time.deltaTime;
                target = FindTarget();
            }

            //¹¥»÷
            if (target != null && attackTimer > data.interval)
            {
                attackTimer = 0;
                Attack();
            }
        }
    }
}
