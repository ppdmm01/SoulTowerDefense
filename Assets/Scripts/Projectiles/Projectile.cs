using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float moveSpeed; //�ƶ��ٶ�
    protected int damage; //�˺�
    protected float explosionRange; //��ը��Χ

    private void OnEnable()
    {
        StartCoroutine(DestroyMe(5f));
    }

    protected virtual void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime * moveSpeed);
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    public void Init(int damage,float explosionRange = 0f)
    {
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
