using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowTower : BaseTower
{
    [Header("Ͷ������Դ·��")]
    public string projectilePath = "Projectiles/Arrow";
    [Header("�����")]
    public Transform firePos; //�����

    /// <summary>
    /// ����Ͷ����ڶ����¼��е��ã�
    /// </summary>
    public void CreateProjectile()
    {
        AudioManager.Instance.PlaySound("SoundEffect/BowAttack");
        GameObject arrowObj = PoolMgr.Instance.GetObj(projectilePath);
        Arrow arrow = arrowObj.GetComponent<Arrow>();
        arrow.Init(this.buffApplier,data.damage);
        arrow.transform.position = firePos.position;
        arrow.transform.rotation = firePos.rotation;
    }
}
