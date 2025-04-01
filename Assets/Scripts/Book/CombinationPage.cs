using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombinationPage : Page
{
    public CombinationSO combinationData; //×éºÏ
    public TextMeshProUGUI description; //ÃèÊö

    private void Start()
    {
        description.text = AttributeManager.Instance.GetAttributeDescription(combinationData.activeAttribute);
    }
}
