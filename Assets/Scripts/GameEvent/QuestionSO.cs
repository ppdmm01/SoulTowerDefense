using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 问答小游戏数据
/// </summary>
[CreateAssetMenu(fileName = "QuestionSO", menuName = "ScriptableObject/QuestionSO")]
public class QuestionSO : ScriptableObject
{
    [Header("问题")]
    [TextArea(3,5)]
    public string questionContent;
    public string ansA;
    public string ansB;
    public string ansC;
    public string ansD;
    [Header("答案（1：A | 2：B | 3：C | 4：D）")]
    public int ans; //最终答案
    [Header("奖励")]
    public ItemSO reward;
}
