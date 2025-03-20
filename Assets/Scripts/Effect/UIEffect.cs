using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEffect : Effect
{

    protected override IEnumerator ReallyDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        PoolMgr.Instance.PushUIObj(gameObject);
    }

    public override void DestroyMeImmediate()
    {
        StopAllCoroutines();
        PoolMgr.Instance.PushUIObj(gameObject);
        EffectManager.Instance.RemoveEffect(this);
    }
}
