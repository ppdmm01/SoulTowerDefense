using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float moveSpeed = 5f;
    protected int damage;

    protected virtual void Start()
    {
        StartCoroutine(DestroyMe(5f));
    }

    protected virtual void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime * moveSpeed);
    }

    /// <summary>
    /// …Ë÷√…À∫¶
    /// </summary>
    /// <param name="dmg">…À∫¶÷µ</param>
    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    private IEnumerator DestroyMe(float time)
    {
        yield return new WaitForSeconds(time);
        PoolMgr.Instance.PushObj(gameObject);
    }
}
