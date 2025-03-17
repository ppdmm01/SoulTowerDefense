using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 查看防御塔信息的
/// </summary>
public class TowerBuffInfoBtn : MonoBehaviour
{
    public Button btn;
    public TextMeshProUGUI labelText;

    /// <summary>
    /// 初始化按钮
    /// </summary>
    /// <param name="data">防御塔数据</param>
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
