using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������ʴ�С��Ϸ����
/// </summary>
[CreateAssetMenu(fileName = "QuestionManagerSO", menuName = "ScriptableObject/QuestionManagerSO")]
public class QuestionManagerSO : ScriptableObject
{
    public List<QuestionSO> questions;
}
