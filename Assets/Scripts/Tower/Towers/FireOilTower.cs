using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireOilTower : BaseTower
{
    [Header("���淶Χ")]
    public float angle = 60f; //�������νǶ�
    public GameObject fireObj; //��������
    private bool isFire; //�Ƿ������


    protected override void Update()
    {
        base.Update();
        if (isFire) attackTimer = 0; //���ʱ����ʱ
    }

    public void AttackStart()
    {
        isFire = true;
        AudioManager.Instance.PlaySound("SoundEffect/BowAttack"); //���������
    }

    public void AttackEnd()
    {
        isFire = false;
    }

    public override void Init(TowerData data)
    {
        base.Init(data);
        //��ʼ���������δ�С��Ŀǰ����뾶Ϊ1��scale��λ��Ӧ2.5��
        float scaleMultiplier = data.range / 2.5f;
        fireObj.transform.localScale = scaleMultiplier * Vector3.one;
    }
    /// <summary>
    /// �˺�
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
    /// Ѱ�һ��淶Χ�ڵĵ���
    /// </summary>
    public List<Enemy> FindWoundEnemies()
    {
        if (target == null) FindTarget();
        if (target == null) return null;
        List<Enemy> enemies = new List<Enemy>();
        Vector2 fireDir = (target.transform.position - transform.position).normalized; //�������䷽��
        foreach (Transform enemyTrans in enemyList)
        {
            Vector2 enemyDir = (enemyTrans.position - transform.position).normalized; //���˷���
            float angle = Vector2.Angle(enemyDir, fireDir);
            if (angle <= (this.angle/2f)) enemies.Add(enemyTrans.GetComponent<Enemy>());
        }
        return enemies;
        
    }
}
