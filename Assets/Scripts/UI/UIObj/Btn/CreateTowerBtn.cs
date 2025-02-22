using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreateTowerBtn : MonoBehaviour
{
    public Button btn; //°´Å¥
    public Image icon; //Í¼Æ¬
    public TextMeshProUGUI costText; //»¨·Ñ
    public void SetTower(TowerData data)
    {
        icon.sprite = data.towerIcon;
        costText.text = data.cost.ToString();
        btn.onClick.AddListener(() =>
        {
            TowerManager.Instance.CreateTower(data.towerName);
        });
    }
}
