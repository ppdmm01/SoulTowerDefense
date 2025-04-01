using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理所有问答小游戏数据
/// </summary>
[CreateAssetMenu(fileName = "QuestionManagerSO", menuName = "ScriptableObject/QuestionManagerSO")]
public class QuestionManagerSO : ScriptableObject
{
    public List<QuestionSO> questions;
}
