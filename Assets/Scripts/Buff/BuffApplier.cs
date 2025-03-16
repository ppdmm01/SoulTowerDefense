using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// buff施加器
/// </summary>
public class BuffApplier
{
    public List<BuffData> buffDatas; //施加的buff

    public BuffApplier(List<BuffData> buffDatas)
    {
        this.buffDatas = buffDatas;
    }

    //每次攻击都需要调用这个
    public void TryApplyBuff(Enemy target)
    {
        foreach (BuffData data in buffDatas)
        {
            float randomNum = Random.Range(0f, 100f);
            if (randomNum <= data.triggerChance)
            {
                ApplyBuff(target,data);
            }
        }
    }

    //施加buff
    private void ApplyBuff(Enemy target,BuffData data)
    {
        //如果buff可以堆叠，则直接添加即可
        if (data.isStack)
        {
            Buff buff = target.gameObject.AddComponent<Buff>(); //可优化，暂时不搞，目前一个敌人的脚本量不会太多
            buff.Init(data, target);
        }
        //否则需要判断
        else
        {
            Buff[] buffs = target.GetComponents<Buff>();
            if (buffs.Any(b => b.data.buffName == data.buffName))
            {
                //已经存在该脚本了
                Buff buff = buffs.FirstOrDefault(b => b.data.buffName == data.buffName);
                //延长buff时间
                buff.ResetTime();
            }
            else
            {
                Buff buff = target.gameObject.AddComponent<Buff>();
                buff.Init(data, target);
            }
        }
    }
}
