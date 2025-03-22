using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO",menuName = "ScriptableObject/EnemySO")]
public class EnemySO : ScriptableObject
{
    [Header("��������")]
    public int hp; //Ѫ��
    public float moveSpeed; //�ƶ��ٶ�
    public int atk; //������
    public float interval; //�������
    public int soulNum; //�������õ���Դ
}
