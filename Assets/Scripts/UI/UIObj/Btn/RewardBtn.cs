using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardBtn : MonoBehaviour
{
    public Button btn;
    public TextMeshProUGUI rewardText;

    public void SetBtn(RewardData reward)
    {
        switch (reward.rewardType)
        {
            case RewardType.Taixu:
                int randomValue = Random.Range(reward.minValue, reward.maxValue+1);
                rewardText.text = "<sprite=8>��" + randomValue;
                btn.onClick.AddListener(() =>
                {
                    GameResManager.Instance.AddTaixuNum(randomValue);
                    Destroy(this.gameObject);
                });
                break;
            case RewardType.Item:
                rewardText.text = "���߽�����";
                btn.onClick.AddListener(() =>
                {
                    UIManager.Instance.ShowPanel<SelectPanel>()?.UpdateItem();
                    Destroy(this.gameObject);
                });
                break;
            case RewardType.Tower:
                rewardText.text = "������������";
                btn.onClick.AddListener(() =>
                {
                    UIManager.Instance.ShowPanel<SelectPanel>()?.UpdateTowerItem();
                    Destroy(this.gameObject);
                });
                break;
        }
    }
}
