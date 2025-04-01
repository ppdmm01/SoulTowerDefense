using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caltrop : BaseTower
{
    public override void Attack()
    {
        if (enemyList.Count == 0) return;
        ani.Play("Attack"); //�����¼���������
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

    //    //��ȡ��������buff
    //    buffApplier = new BuffApplier(data.buffDatas);

    //    //���ײ���
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

    //    //��������Χ
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
    }
}
