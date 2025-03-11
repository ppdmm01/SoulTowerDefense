using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// ±Í«©∞¥≈•
/// </summary>
public class LabelBtn : MonoBehaviour
{
    public int page;
    public Button btn;

    private void Awake()
    {
        btn.onClick.AddListener(() =>
        {
            BookPanel panel = UIManager.Instance.GetPanel<BookPanel>();
            if (panel != null)
            {
                panel.UpdatePage(page);
            }
        });
    }
}
