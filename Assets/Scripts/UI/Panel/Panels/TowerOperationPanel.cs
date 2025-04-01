using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerOperationPanel : BasePanel
{
    public Image bk; //背景
    public Button closeBtn;
    public Button sellBtn;
    public TextMeshProUGUI towerNameTxt;
    private BaseTower targetTower; //被选择的防御塔
    private int towerRes = 0; //售卖后获得的资源
    private Vector2 pos = Vector2.one; //位置
    public override void Init()
    {
        closeBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<TowerOperationPanel>(false);
            TowerManager.Instance.isOpenPanel = false;
        });
        sellBtn.onClick.AddListener(() =>
        {
            GameResManager.Instance.AddSoulNum(towerRes);
            UIManager.Instance.ShowTxtPopup($"<sprite=5><color=purple>{towerRes}</color>", Color.white, 36, pos);
            if (targetTower != null)
                targetTower.Dead(); //销毁防御塔
            UIManager.Instance.HidePanel<TowerOperationPanel>(false);
            TowerManager.Instance.isOpenPanel = false;
        });
    }

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="data">防御塔数据</param>
    /// <param name="pos">位置</param>
    public void SetInfo(BaseTower tower)
    {
        pos = tower.transform.position;
        bk.transform.position = Camera.main.WorldToScreenPoint(pos);
        towerNameTxt.text = tower.data.towerChineseName;
        towerRes = (int)(tower.data.cost * 0.5f);
        targetTower = tower;
    }
}
