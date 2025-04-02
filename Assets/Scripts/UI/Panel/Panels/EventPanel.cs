using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// �¼���壨���һ��ż��ϵģ�д�ĺܸϣ����Դ��벻̫���ۣ�
/// </summary>
public class EventPanel : BasePanel
{
    public TextMeshProUGUI contentText;
    public Button OkBtn;
    public Button CancelBtn;
    public TextMeshProUGUI OkBtnText;
    public TextMeshProUGUI CancelBtnText;

    public GameObject EventBg; //�ս����¼���������ť�����
    public GameObject rewardBg; //�������
    [Header("�ش���������")]
    public GameObject QuestionBg; //�ش���������
    public GameObject questionRewardBg; //���⽱�����
    [Header("����������")]
    public GameObject treasureBg; //�������
    public GameObject enemyBg; //�������
    [Header("ˮ����Ƭ������")]
    public GameObject crystalBg; //ˮ����Ƭ���

    [Header("ȷ����ť")]
    public Button sureBtn;

    [Header("�ش�����İ�ť")]
    public Button btnA;
    public TextMeshProUGUI txtA;
    public Button btnB;
    public TextMeshProUGUI txtB;
    public Button btnC;
    public TextMeshProUGUI txtC;
    public Button btnD;
    public TextMeshProUGUI txtD;
    private int nowSelect; //��ǰѡ��

    [Header("������")]
    public BaseGrid storageBox;
    public Button arrangeBtn;

    [Header("����")]
    public SelectItemGrid rewardGrid;
    [Header("��Ʒ��Ϣ")]
    public GameObject ItemInfoObj;

    public override void Init()
    {
        HideItemInfo();
        sureBtn.gameObject.SetActive(false);
        treasureBg.SetActive(false);
        QuestionBg.SetActive(false);
        EventBg.SetActive(true);
        rewardBg.SetActive(false);
        enemyBg.SetActive(false);
        questionRewardBg.SetActive(false);
        crystalBg.SetActive(false);

        sureBtn.onClick.AddListener(() =>
        {
            canvasGroup.blocksRaycasts = false;
            UIManager.Instance.HidePanel<EventPanel>();
            UIManager.Instance.ShowPanel<MapPanel>();
        });
        arrangeBtn.onClick.AddListener(() =>
        {
            GridManager.Instance.GetGridByName(storageBox.gridName).AutoArrange();
        });
        //���һ���¼�
        RandomEvent();
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //�򱳰�����������ӱ���
        GridManager.Instance.AddGrid(storageBox);
        //��ȡ�������ݲ�����
        GridData storageBoxData = GameDataManager.Instance.GetGridData(storageBox.gridName);
        if (storageBoxData != null)
            storageBox.UpdateGrid(storageBoxData);
    }

    public override void HideMe(UnityAction action)
    {
        base.HideMe(action);
        //������������
        GridData storageBoxData = new GridData(storageBox.gridName, storageBox.items);
        GameDataManager.Instance.UpdateGridData(storageBoxData);
        GameDataManager.Instance.SaveGridData();
        //�����Ʒ
        GridManager.Instance.ClearAllItem(rewardGrid, false);
        GridManager.Instance.ClearAllItem(storageBox, false);
        //�򱳰����������Ƴ�����
        GridManager.Instance.RemoveGrid(storageBox);
    }

    //���һ���¼�
    public void RandomEvent()
    {
        int randomNum = Random.Range(0, 100);
        if (randomNum < 40)
            QuestionEvent();
        else if (randomNum < 70)
            TreasureEvent();
        else
            CrystalEvent();
    }

    //�ʺ��¼�
    private void QuestionEvent()
    {
        contentText.text = "��������һ��ͷ���ʺŵ�С���飬С����˵��\n" +
            "<color=purple>��������~�����ð���ߣ�Ҫ���ʴ���Ϸ�� �ش�����н���Ŷ����</color>��";
        OkBtn.onClick.RemoveAllListeners();
        CancelBtn.onClick.RemoveAllListeners();
        OkBtnText.text = "ͬ��";
        CancelBtnText.text = "������";
        OkBtn.onClick.AddListener(() =>
        {
            EventBg.SetActive(false);
            //����ش�����
            BeginQuestion();
        });
        CancelBtn.onClick.AddListener(() =>
        {
            contentText.text = "��ͷҲ���ص�����";
            EventBg.SetActive(false);
            sureBtn.gameObject.SetActive(true);
        });
    }

    //�����¼�
    private void TreasureEvent()
    {
        treasureBg.SetActive(true);
        contentText.text = "�㷢����һ�����䣡�����...";
        OkBtn.onClick.RemoveAllListeners();
        CancelBtn.onClick.RemoveAllListeners();
        OkBtnText.text = "��";
        CancelBtnText.text = "����";
        OkBtn.onClick.AddListener(() =>
        {
            EventBg.SetActive(false);
            //�򿪱���
            OpenChest();
        });
        CancelBtn.onClick.AddListener(() =>
        {
            contentText.text = "�������ð�����";
            EventBg.SetActive(false);
            sureBtn.gameObject.SetActive(true);
        });
    }

    //ˮ���¼�
    private void CrystalEvent()
    {
        crystalBg.SetActive(true);
        contentText.text = "�㷢����һ��ˮ����Ƭ�������...";
        OkBtn.onClick.RemoveAllListeners();
        CancelBtn.onClick.RemoveAllListeners();
        OkBtnText.text = "����";
        CancelBtnText.text = "����";
        OkBtn.onClick.AddListener(() =>
        {
            contentText.text = $"��������ˮ����Ԫ��ˮ������ֵ<color=green>+20</color>��";
            TopColumnPanel panel = UIManager.Instance.ShowPanel<TopColumnPanel>();
            int nowHp = Mathf.Clamp(GameResManager.Instance.gameRes.coreNowHp + 20,0,GameResManager.Instance.gameRes.coreMaxHp);
            GameResManager.Instance.UpdateCoreHp(nowHp);
            panel.UpdateHp(nowHp, GameResManager.Instance.gameRes.coreMaxHp);
            EventBg.SetActive(false);
            sureBtn.gameObject.SetActive(true);
        });
        CancelBtn.onClick.AddListener(() =>
        {
            int randomValue = Random.Range(50,80);
            contentText.text = $"�������ˮ�������<sprite=8><color=purple>{randomValue}</color>��";
            GameResManager.Instance.AddTaixuNum(randomValue);
            EventBg.SetActive(false);
            crystalBg.SetActive(false);
            sureBtn.gameObject.SetActive(true);
        });
    }

    #region �ʴ��¼�
    //��ʼ�ʴ�
    public void BeginQuestion()
    {
        QuestionBg.gameObject.SetActive(true);
        QuestionSO data = QuestionManager.Instance.GetRandomQuestion();
        contentText.text = data.questionContent;
        txtA.text = data.ansA;
        txtB.text = data.ansB;
        txtC.text = data.ansC;
        txtD.text = data.ansD;
        btnA.onClick.RemoveAllListeners();
        btnB.onClick.RemoveAllListeners();
        btnC.onClick.RemoveAllListeners();
        btnD.onClick.RemoveAllListeners();
        btnA.onClick.AddListener(() =>
        {
            nowSelect = 1;
            CheckAns(data);
        });
        btnB.onClick.AddListener(() =>
        {
            nowSelect = 2;
            CheckAns(data);
        });
        btnC.onClick.AddListener(() =>
        {
            nowSelect = 3;
            CheckAns(data);
        });
        btnD.onClick.AddListener(() =>
        {
            nowSelect = 4;
            CheckAns(data);
        });
    }

    //����
    public void CheckAns(QuestionSO data)
    {
        btnA.interactable = false;
        btnB.interactable = false;
        btnC.interactable = false;
        btnD.interactable = false;
        string ans = "";
        switch (data.ans)
        {
            case 1: ans = "A"; break;
            case 2: ans = "B"; break;
            case 3: ans = "C"; break;
            case 4: ans = "D"; break;
        }
        if (nowSelect != data.ans)
        {
            //�ش����
            contentText.text = $"С���飺<color=purple>���ش���� ��ȷ����{ans}���´������ɣ�����~��</color>";
        }
        else
        {
            //�ش���ȷ����ý���
            contentText.text = "С���飺<color=purple>���ش���ȷ�� ��ȡ��Ľ����ɣ�����~��</color>";
            questionRewardBg.SetActive(true);
            QuestionBg.SetActive(false);
            rewardBg.SetActive(true);
            AddRewardItem(data.reward);
        }

        //�г��˳���ť
        sureBtn.gameObject.SetActive(true);
    }
    #endregion

    #region �����¼�
    //�򿪱���
    public void OpenChest()
    {
        treasureBg.SetActive(false);
        Debug.Log(treasureBg.activeSelf);
        int randomNum = Random.Range(0, 100);
        if (randomNum < 40)
        {
            //�ܵ�Ϯ��
            contentText.text = "�����ܵ���Ϯ����ˮ������ֵ<color=red>-5</color>";
            enemyBg.SetActive(true);
            TopColumnPanel panel = UIManager.Instance.ShowPanel<TopColumnPanel>();
            GameResManager.Instance.UpdateCoreHp(GameResManager.Instance.gameRes.coreNowHp - 5);
            panel.UpdateHp(GameResManager.Instance.gameRes.coreNowHp,GameResManager.Instance.gameRes.coreMaxHp);
            if (GameResManager.Instance.gameRes.coreNowHp <= 0)
            {
                LevelManager.Instance.LoseGame(); //��Ϸʧ��
            }
        }
        else
        {
            contentText.text = "�����˽�����";
            //����
            rewardBg.SetActive(true);
            AddRandomItem();
        }

        //�г��˳���ť
        sureBtn.gameObject.SetActive(true);
    }
    #endregion

    //��ý�����Ʒ
    public void AddRewardItem(ItemSO data)
    {
        data = ItemManager.Instance.GetItemDataById(data.id);
        rewardGrid.ForceUpdateGridLayout();
        GridManager.Instance.AddItem(data.id, rewardGrid);
    }

    //������������Ʒ
    public void AddRandomItem()
    {
        ItemSO data = ItemManager.Instance.GetRandomItemData(1)[0];
        rewardGrid.ForceUpdateGridLayout();
        GridManager.Instance.AddItem(data.id, rewardGrid);
    }

    /// <summary>
    /// ��ʾ��Ʒ��Ϣ
    /// </summary>
    public void ShowItemInfo(Item item)
    {
        ItemInfoObj.SetActive(true);
        ItemInfoObj.GetComponent<ItemInfo>().SetInfo(item);
    }

    /// <summary>
    /// ������Ʒ��Ϣ
    /// </summary>
    public void HideItemInfo()
    {
        ItemInfoObj.SetActive(false);
        ItemInfoObj.GetComponent<ItemInfo>().RemoveAllAttributeInfo();
    }
}
