using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowTower : BaseTower
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
        AudioManager.Instance.PlaySound("SoundEffect/BowAttack");
        GameObject arrowObj = PoolMgr.Instance.GetObj(projectilePath);
        Arrow arrow = arrowObj.GetComponent<Arrow>();
        arrow.Init(this.buffApplier,data.damage);
        arrow.transform.position = firePos.position;
        arrow.transform.rotation = firePos.rotation;
    }
}
