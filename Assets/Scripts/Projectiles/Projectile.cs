using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float moveSpeed; //移动速度
    protected int damage; //伤害
    protected float explosionRange; //爆炸范围

    protected BuffApplier buffApplier; //可以施加的buff

    private void OnEnable()
    {
        StartCoroutine(DestroyMe(5f));
    }

    protected virtual void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime * moveSpeed);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(BuffApplier buffApplier, int damage,float explosionRange = 0f)
    {
        this.buffApplier = buffApplier;
        this.damage = damage;
        this.explosionRange = explosionRange;
    }

    private IEnumerator DestroyMe(float time)
    {
        yield return new WaitForSeconds(time);
        PoolMgr.Instance.PushObj(gameObject);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
