using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : BasePanel
{
    public TextMeshProUGUI title;
    public Button sureBtn;
    public override void Init()
    {
        sureBtn.onClick.AddListener(() =>
        {
            //�ص����˵�
            UIManager.Instance.LoadScene("BeginScene", () =>
            {
                UIManager.Instance.HidePanel<GameOverPanel>();
                UIManager.Instance.ShowPanel<BeginPanel>();
            });
            //����浵
            GameDataManager.Instance.ClearGameData();
        });
    }

    public void SetTitle(bool isWin)
    {
        title.text = isWin ? "ʤ��������" : "ʧ��!"; 
    }
}
