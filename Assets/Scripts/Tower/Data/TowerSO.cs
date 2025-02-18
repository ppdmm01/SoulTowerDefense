using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerSO",menuName = "ScriptableObject/TowerSO")]
public class TowerSO : ScriptableObject
{
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
}
