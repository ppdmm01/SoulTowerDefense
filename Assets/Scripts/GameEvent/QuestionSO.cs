using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ʴ�С��Ϸ����
/// </summary>
[CreateAssetMenu(fileName = "QuestionSO", menuName = "ScriptableObject/QuestionSO")]
public class QuestionSO : ScriptableObject
{
    [Header("����")]
    [TextArea(3,5)]
    public string questionContent;
    public string ansA;
    public string ansB;
    public string ansC;
    public string ansD;
    [Header("�𰸣�1��A | 2��B | 3��C | 4��D��")]
    public int ans; //���մ�
    [Header("����")]
    public ItemSO reward;
}
