using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDragonTower : BaseTower
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
        GameObject obj = PoolMgr.Instance.GetObj(projectilePath);
        FireDragon fireDragon = obj.GetComponent<FireDragon>();
        fireDragon.Init(this.buffApplier,data.damage,1f);
        fireDragon.transform.position = firePos.position;
        fireDragon.transform.rotation = firePos.rotation;
    }
}
