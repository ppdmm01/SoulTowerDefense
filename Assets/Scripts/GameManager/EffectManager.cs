using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EffectManager : Singleton<EffectManager>
{
    private EffectManager() 
    {
        effects = new List<Effect>();
    }

    private List<Effect> effects;

    public void PlayEffect(string effectName,Vector2 pos,UnityAction callback = null)
    {
        GameObject effObj = PoolMgr.Instance.GetObj("Effect/" + effectName);
        effObj.transform.position = pos;
        Effect effect = effObj.GetComponent<Effect>();
        effect.AddCallBack(callback);
        AddEffect(effect);
    }

    public void PlayUIEffect(string effectName, Vector2 pos, UnityAction callback = null)
    {
        GameObject effObj = PoolMgr.Instance.GetUIObj("Effect/" + effectName);
        effObj.transform.position = pos;
        Effect effect = effObj.GetComponent<Effect>();
        effect.AddCallBack(callback);
        AddEffect(effect);
    }

    public void AddEffect(Effect eff)
    {
        if (!effects.Contains(eff))
            effects.Add(eff);
    }

    public void RemoveEffect(Effect eff)
    {
        if (effects.Contains(eff))
            effects.Remove(eff);
    }

    public void ClearEffects()
    {
        foreach (Effect eff in effects)
            GameObject.Destroy(eff.gameObject);
        effects.Clear();
    }
}
