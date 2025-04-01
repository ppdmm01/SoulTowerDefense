using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BronzeMirrorTower : BaseTower
{
    protected override void Update()
    {
        if (!isUsed) return;

        //更新血条位置
        if (hpBar != null && hpBar.gameObject.activeSelf)
            SetHpBarPos(transform.position + Vector3.up);
    }

    public override void Init(TowerData data)
    {
        this.data = data;
        isDead = false;

        enemyList = new List<Transform>();
        ani = GetComponent<Animator>();

        //获取防御塔的buff
        buffApplier = new BuffApplier(data.buffDatas);

        //闪白材质
        Material material = Resources.Load<Material>("Material/FlashMaterial");
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.material != material)
                renderer.material = material;
        }

        nowHp = data.hp;

        target = null;

        //创建血条
        if (data.hp > 0)
            CreateHpBar();
    }
    public override void Wound(int dmg, Enemy enemy)
    {
        if (enemy != null)
        {
            buffApplier.TryApplyBuff(enemy);
        }
        base.Wound(dmg);

    }
}
