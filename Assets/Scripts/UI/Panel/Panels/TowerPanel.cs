using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanel : BasePanel
{
    public TextMeshProUGUI qiNumText; //��������Դ�����ı�
    public HealthBar hpBar; //����Ѫ��
    public Button createTowerBtn; //����������ť
    public Button createTowerBtn2; 
    public override void Init()
    {
        ChangeQiNum(0);

        createTowerBtn.onClick.AddListener(() =>
        {
            //����������
            TowerManager.Instance.CreateTower("CrossbowTower");
        });

        createTowerBtn2.onClick.AddListener(() =>
        {
            //����������
            TowerManager.Instance.CreateTower("FireDragonTower");
        });
    }

    public void ChangeQiNum(int num)
    {
        qiNumText.text = num.ToString();    
    }
}
