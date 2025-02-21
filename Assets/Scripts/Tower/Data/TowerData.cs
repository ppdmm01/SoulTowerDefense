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
    public string description;

    [Header("��������")]
    public int hp; //Ѫ��
    public int cost; //����

    [Header("�������")]
    public int damage; //�˺�
    public float range; //������Χ
    public float interval; //�������

    [Header("������Դ���")]
    public int output; //ÿ�β���
    public float cooldown; //������ȴʱ��

    /// <summary>
    /// ��ʼ����
    /// </summary>
    /// <param name="towerSO"></param>
    public void Init(TowerSO towerSO)
    {
        towerName = towerSO.name;
        description = towerSO.description;
        hp = towerSO.hp;
        cost = towerSO.cost;
        damage = towerSO.damage;
        range = towerSO.range;
        interval = towerSO.interval;
        output = towerSO.output;
        cooldown = towerSO.cooldown;
    }
}
