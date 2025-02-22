using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����������
/// </summary>
[CreateAssetMenu(fileName = "TowerSO",menuName = "ScriptableObject/TowerSO")]
public class TowerSO : ScriptableObject
{
    [Header("����������")]
    public string towerName; //����������
    public string towerChineseName; //��������������
    [TextArea]
    public string description; //����
    public Sprite towerIcon; //������ͼƬ

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
}
