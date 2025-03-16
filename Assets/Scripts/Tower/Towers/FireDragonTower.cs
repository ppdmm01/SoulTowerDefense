using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDragonTower : BaseTower
{
    [Header("投射物资源路径")]
    public string projectilePath = "Projectiles/Arrow";
    [Header("发射口")]
    public Transform firePos; //发射口

    /// <summary>
    /// 创建投射物（在动画事件中调用）
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
