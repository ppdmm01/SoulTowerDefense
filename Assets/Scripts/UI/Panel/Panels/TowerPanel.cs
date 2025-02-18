using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanel : BasePanel
{
    public TextMeshProUGUI qiNumText; //��������Դ�����ı�
    public Button createTowerBtn; //����������ť
    public override void Init()
    {
        ChangeQiNum(0);

        createTowerBtn.onClick.AddListener(() =>
        {
            //����������
            TowerManager.Instance.CreateTower("CrossbowTower");
        });
    }

    public void ChangeQiNum(int num)
    {
        qiNumText.text = num.ToString();    
    }
}
