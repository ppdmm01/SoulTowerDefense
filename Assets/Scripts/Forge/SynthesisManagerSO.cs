using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SynthesisManagerSO", menuName = "ScriptableObject/SynthesisManagerSO")]
public class SynthesisManagerSO : ScriptableObject
{
    public List<SynthesisSO> synthesisSOList; //合成列表
}
