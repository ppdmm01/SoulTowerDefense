using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// buffʩ����
/// </summary>
public class BuffApplier
{
    public List<BuffData> buffDatas; //ʩ�ӵ�buff

    public BuffApplier(List<BuffData> buffDatas)
    {
        this.buffDatas = buffDatas;
    }

    //ÿ�ι�������Ҫ�������
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

    //ʩ��buff
    private void ApplyBuff(Enemy target,BuffData data)
    {
        //���buff���Զѵ�����ֱ����Ӽ���
        if (data.isStack)
        {
            Buff buff = target.gameObject.AddComponent<Buff>(); //���Ż�����ʱ���㣬Ŀǰһ�����˵Ľű�������̫��
            buff.Init(data, target);
        }
        //������Ҫ�ж�
        else
        {
            Buff[] buffs = target.GetComponents<Buff>();
            if (buffs.Any(b => b.data.buffName == data.buffName))
            {
                //�Ѿ����ڸýű���
                Buff buff = buffs.FirstOrDefault(b => b.data.buffName == data.buffName);
                //�ӳ�buffʱ��
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
