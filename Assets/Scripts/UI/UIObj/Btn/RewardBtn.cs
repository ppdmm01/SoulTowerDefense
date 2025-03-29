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
                rewardText.text = "̫���" + reward.value;
                btn.onClick.AddListener(() =>
                {
                    GameResManager.Instance.AddTaixuNum(reward.value);
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
        }
    }
}
