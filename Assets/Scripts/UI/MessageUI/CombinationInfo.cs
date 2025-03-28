using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombinationInfo : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    public void SetInfo(string title,ItemAttribute itemAttribute)
    {
        this.title.text = title;
        this.description.text = AttributeManager.Instance.GetAttributeDescription(itemAttribute);
    }
}
