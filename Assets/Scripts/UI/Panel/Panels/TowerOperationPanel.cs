using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerOperationPanel : BasePanel
{
    public Image bk; //����
    public Button closeBtn;
    public Button sellBtn;
    public TextMeshProUGUI towerNameTxt;
    private BaseTower targetTower; //��ѡ��ķ�����
    private int towerRes = 0; //�������õ���Դ
    private Vector2 pos = Vector2.one; //λ��
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
                targetTower.Dead(); //���ٷ�����
            UIManager.Instance.HidePanel<TowerOperationPanel>(false);
            TowerManager.Instance.isOpenPanel = false;
        });
    }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="data">����������</param>
    /// <param name="pos">λ��</param>
    public void SetInfo(BaseTower tower)
    {
        pos = tower.transform.position;
        bk.transform.position = Camera.main.WorldToScreenPoint(pos);
        towerNameTxt.text = tower.data.towerChineseName;
        towerRes = (int)(tower.data.cost * 0.5f);
        targetTower = tower;
    }
}
