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
        switch (data.buffType)
        {
            case BuffType.None:
                labelText.text = "";
                break;
            case BuffType.Burn:
                labelText.text = "<sprite=1>";
                break;
            case BuffType.Slow:
                labelText.text = "<sprite=4>";
                break;
            case BuffType.Stun:
                labelText.text = "<sprite=6>";
                break;
            case BuffType.Mark:
                labelText.text = "<sprite=9>";
                break;
            default:
                labelText.text = "";
                break;
        }
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
