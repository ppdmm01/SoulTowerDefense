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

    [Header("�������")]
    public bool isAttacker; //�Ƿ��ǹ�����
    public int damage; //�˺�
    public float range; //������Χ
    public float interval; //�������

    [Header("������Դ���")]
    public bool isProducer; //�Ƿ�������Դ
    public int output; //ÿ�β���
    public float cooldown; //������ȴʱ��

    [Header("��ǩ")]
    public List<ItemTag> itemTags; //��������Ʒ��������

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
        damage = towerSO.damage;
        range = towerSO.range;
        interval = towerSO.interval;
        isProducer = towerSO.isProducer;
        output = towerSO.output;
        cooldown = towerSO.cooldown;
        itemTags = towerSO.itemTags;
    }
}
