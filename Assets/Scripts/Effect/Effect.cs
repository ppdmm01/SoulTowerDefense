using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 特效生成
/// </summary>
public class Effect : MonoBehaviour
{
    public float DestroyTime = 3f;
    private UnityAction callback;
    private void OnEnable()
    {
        DestroyMe(DestroyTime);
    }

    private void OnDisable()
    {
        transform.localScale = Vector3.one;
        StopAllCoroutines();
        callback = null;
    }

    public void AddCallBack(UnityAction callback)
    {
        this.callback = callback;
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
        callback?.Invoke();
        callback = null;
        PoolMgr.Instance.PushObj(gameObject);
    }

    /// <summary>
    /// 立即删除自己
    /// </summary>
    public virtual void DestroyMeImmediate()
    {
        StopAllCoroutines();
        callback?.Invoke();
        callback = null;
        PoolMgr.Instance.PushObj(gameObject);
        EffectManager.Instance.RemoveEffect(this);
    }
}
