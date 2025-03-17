using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �鿴��������Ϣ��
/// </summary>
public class TowerBuffInfoBtn : MonoBehaviour
{
    public Button btn;
    public TextMeshProUGUI labelText;

    /// <summary>
    /// ��ʼ����ť
    /// </summary>
    /// <param name="data">����������</param>
    public void InitInfo(BuffData data)
    {
        labelText.text = data.buffName;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            PreFightPanel panel = UIManager.Instance.GetPanel<PreFightPanel>();
            if (panel != null)
            {
                panel.buffInfo.SetActive(true);
                panel.UpdateBuffInfo(data);
                panel.nowBuffType = data.buffType;
            }
        });
    }
}
