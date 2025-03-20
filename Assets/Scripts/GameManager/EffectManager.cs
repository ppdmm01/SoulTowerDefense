using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    private EffectManager() 
    {
        effects = new List<Effect>();
    }

    private List<Effect> effects;

    public void PlayEffect(string effectName,Vector2 pos)
    {
        GameObject effObj = PoolMgr.Instance.GetObj("Effect/" + effectName);
        effObj.transform.position = pos;
        AddEffect(effObj.GetComponent<Effect>());
    }

    public void PlayUIEffect(string effectName, Vector2 pos)
    {
        GameObject effObj = PoolMgr.Instance.GetUIObj("Effect/" + effectName);
        effObj.transform.position = pos;
        AddEffect(effObj.GetComponent<Effect>());
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
