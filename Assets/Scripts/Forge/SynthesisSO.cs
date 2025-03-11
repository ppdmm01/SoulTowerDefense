using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SynthesisSO",menuName = "ScriptableObject/SynthesisSO")]
public class SynthesisSO : ScriptableObject
{
    public int id; //id
    public List<ItemSO> recipe; //≈‰∑Ω
    public ItemSO product; //≥…∆∑
}
