using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特效生成
/// </summary>
public class Effect : MonoBehaviour
{
    private void OnEnable()
    {
        DestroyMe(3f);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 删除自己
    /// </summary>
    /// <param name="time"></param>
    protected virtual void DestroyMe(float time = 0f)
    {
        StopAllCoroutines();
        StartCoroutine(ReallyDestroy(time));
    }

    protected virtual IEnumerator ReallyDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        PoolMgr.Instance.PushObj(gameObject);
    }

    /// <summary>
    /// 立即删除自己
    /// </summary>
    public virtual void DestroyMeImmediate()
    {
        StopAllCoroutines();
        PoolMgr.Instance.PushObj(gameObject);
        EffectManager.Instance.RemoveEffect(this);
    }
}
