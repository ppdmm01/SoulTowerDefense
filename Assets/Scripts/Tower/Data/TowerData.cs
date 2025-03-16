using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �洢�������������ݣ���������+��̬�޸ĵ����ݣ�
/// </summary>
public class TowerData
{
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



    public TowerData(TowerSO towerSO)
    {
        Init(towerSO);
    }

    public TowerData(TowerData other)
    {
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
    }

    /// <summary>
    /// ��ʼ����
    /// </summary>
    /// <param name="towerSO"></param>
    public void Init(TowerSO towerSO)
    {
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
            list.Add(buffDatas[i]);
        }
        return list;
    }
}
