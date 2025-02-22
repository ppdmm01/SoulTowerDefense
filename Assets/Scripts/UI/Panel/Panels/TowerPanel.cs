using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanel : BasePanel
{
    public TextMeshProUGUI qiNumText; //“气”资源数量文本
    public HealthBar hpBar; //基地血量
    public Transform towerBtnContent; //放置按钮的位置
    private List<CreateTowerBtn> towerBtnList = new List<CreateTowerBtn>(); //存储放置防御塔的按钮
    public override void Init()
    {
        ChangeQiNum(0);
    }

    /// <summary>
    /// 初始化防御塔放置按钮
    /// </summary>
    public void InitTowerBtn()
    {
        foreach (TowerData towerData in TowerManager.Instance.towers.Values)
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("UI/UIObj/CreateTowerBtn"));
            obj.transform.SetParent(towerBtnContent, false);
            CreateTowerBtn btn = obj.GetComponent<CreateTowerBtn>();
            btn.SetTower(towerData);
            towerBtnList.Add(btn);
        }
    }

    /// <summary>
    /// 摧毁所有放置防御塔按钮
    /// </summary>
    public void DestroyAllTowerBtn()
    {
        foreach (CreateTowerBtn btn in towerBtnList)
            Destroy(btn);
        towerBtnList.Clear();
    }

    public void ChangeQiNum(int num)
    {
        qiNumText.text = num.ToString();    
    }
}
