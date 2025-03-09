using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 防御塔信息按钮
/// </summary>
public class TowerInfoBtn : MonoBehaviour
{
    public Button btn;
    public Image icon;
    private TowerData data;

    /// <summary>
    /// 初始化按钮
    /// </summary>
    /// <param name="data">防御塔数据</param>
    public void InitInfo(TowerData data)
    {
        btn.onClick.RemoveAllListeners();
        this.data = data;
        icon.sprite = data.towerIcon;
        btn.onClick.AddListener(() =>
        {
            PreFightPanel panel = UIManager.Instance.GetPanel<PreFightPanel>();
            if (panel != null)
            {
                panel.UpdateTowerInfo(data.towerName); //更新面板信息
                panel.nowTowerInfoName = data.towerName; //更新当前展示的防御塔名
            }
        });
    }

    /// <summary>
    /// 获取宽度
    /// </summary>
    public float GetWeight()
    {
        return btn.image.rectTransform.sizeDelta.x;
    }
}
