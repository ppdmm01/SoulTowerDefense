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
            //回到主菜单
            UIManager.Instance.LoadScene("BeginScene", () =>
            {
                UIManager.Instance.HidePanel<GameOverPanel>();
                UIManager.Instance.ShowPanel<BeginPanel>();
            });
            //清除存档
            GameDataManager.Instance.ClearGameData();
        });
    }

    public void SetTitle(bool isWin)
    {
        title.text = isWin ? "胜利！！！" : "失败!"; 
    }
}
