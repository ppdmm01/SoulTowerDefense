using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanel : BasePanel
{
    public TextMeshProUGUI qiNumText; //“气”资源数量文本
    public HealthBar hpBar; //基地血量
    public Button createTowerBtn; //创建塔防按钮
    public Button createTowerBtn2; 
    public override void Init()
    {
        ChangeQiNum(0);

        createTowerBtn.onClick.AddListener(() =>
        {
            //创建防御塔
            TowerManager.Instance.CreateTower("CrossbowTower");
        });

        createTowerBtn2.onClick.AddListener(() =>
        {
            //创建防御塔
            TowerManager.Instance.CreateTower("FireDragonTower");
        });
    }

    public void ChangeQiNum(int num)
    {
        qiNumText.text = num.ToString();    
    }
}
