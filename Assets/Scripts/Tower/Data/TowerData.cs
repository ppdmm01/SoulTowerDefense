using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �洢�������������ݣ���������+��̬�޸ĵ����ݣ�
/// </summary>
public class TowerData
{
    public TowerSO originData; //ԭʼ����

    [Header("����������")]
    public string towerName;
    public string towerChineseName;
    public string description;
    public Sprite towerIcon;

    [Header("��������")]
    public int hp; //Ѫ��
    public int cost; //����
    public bool canBeAttack; //�Ƿ��ܱ�����

    [Header("�������")]
    public bool isAttacker; //�Ƿ��ǹ�����
    public int damage; //�˺�
    public float range; //������Χ
    public float interval; //�������

    [Header("������Դ���")]
    public bool isProducer; //�Ƿ�������Դ
    public int output; //ÿ�β���
    public float cooldown; //������ȴʱ��

    [Header("Buff���")]
    public List<BuffData> buffDatas; //Buff����

    [Header("��ǩ")]
    public List<ItemTag> itemTags; //��������Ʒ��������

    [Header("���Ա��ʼ���")]
    public float damageMultiplier = 1f;
    public float rangeMultiplier = 1f;
    public float intervalMultiplier = 1f;

    public TowerData() { }

    public TowerData(TowerSO towerSO,List<BuffType> itemBuffTypes)
    {
        Init(towerSO, itemBuffTypes);
    }

    public TowerData(TowerData other)
    {
        originData = other.originData;
        towerName = other.towerName;
        towerChineseName = other.towerChineseName;
        description = other.description;
        towerIcon = other.towerIcon;
        hp = other.hp;
        cost = other.cost;
        canBeAttack = other.canBeAttack;
        isAttacker = other.isAttacker;
        damage = other.damage;
        range = other.range;
        interval = other.interval;
        output = other.output;
        cooldown = other.cooldown;
        buffDatas = other.GetBuffDatas();
        itemTags = other.itemTags;

        damageMultiplier = other.damageMultiplier;
        rangeMultiplier = other.rangeMultiplier;
        intervalMultiplier = other.intervalMultiplier;
    }

    /// <summary>
    /// ��ʼ����
    /// </summary>
    /// <param name="towerSO"></param>
    public void Init(TowerSO towerSO, List<BuffType> itemBuffTypes)
    {
        originData = towerSO;
        towerName = towerSO.towerName;
        towerChineseName = towerSO.towerChineseName;
        description = towerSO.description;
        towerIcon = towerSO.towerIcon;
        isAttacker = towerSO.isAttacker;
        hp = towerSO.hp;
        cost = towerSO.cost;
        canBeAttack = towerSO.canBeAttack;
        damage = towerSO.damage;
        range = towerSO.range;
        interval = towerSO.interval;
        isProducer = towerSO.isProducer;
        output = towerSO.output;
        cooldown = towerSO.cooldown;
        buffDatas = towerSO.GetBuffDatas();
        itemTags = towerSO.itemTags;

        damageMultiplier = 1f;
        rangeMultiplier = 1f;
        intervalMultiplier = 1f;

        //Ϊ���������buff
        if(itemBuffTypes != null)
        {
            //�ж��Ƿ���buff��
            foreach (BuffType type in itemBuffTypes)
            {
                if (buffDatas.Any(buffData => buffData.buffType == type)) continue; //���˾Ͳ��ü���
                else buffDatas.Add(BuffConfigManager.Instance.GetBuffConfigData(type)); //û�о���ӳ�ʼֵ
            }
        }
    }

    /// <summary>
    /// ��ȡ�������buff����
    /// </summary>
    /// <returns></returns>
    public List<BuffData> GetBuffDatas()
    {
        List<BuffData> list = new List<BuffData>();
        for(int i = 0; i < buffDatas.Count; i++)
        {
            list.Add(new BuffData(buffDatas[i]));
        }
        return list;
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void UpdateAttribute()
    {
        damage = Mathf.RoundToInt(originData.damage * damageMultiplier);
        range = originData.range * rangeMultiplier;
        interval = originData.interval * intervalMultiplier;
        foreach (BuffData data in buffDatas)
        {
            BuffData originBuffData = originData.GetBuffData(data.buffType);
            //�����˺����������� * ��ǰ����
            if (originBuffData != null)
                data.damage = Mathf.RoundToInt(originData.GetBuffData(data.buffType).damage * data.damageMultiplier);
        }
    }

    /// <summary>
    /// ��ȡbuff����
    /// </summary>
    public BuffData GetBuffData(BuffType type)
    {
        if (type == BuffType.None) return null;
        if (buffDatas.Any(data => data.buffType == type))
            return buffDatas.FirstOrDefault(data => data.buffType == type);
        else
            return null;
    }
}
