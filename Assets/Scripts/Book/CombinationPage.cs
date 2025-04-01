using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombinationPage : Page
{
    public CombinationSO combinationData; //���
    public TextMeshProUGUI description; //����

    private void Start()
    {
        description.text = AttributeManager.Instance.GetAttributeDescription(combinationData.activeAttribute);
    }
}
