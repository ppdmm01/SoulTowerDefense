using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    //�����ϵ�Canvas
    public Transform canvasTrans;
    //������Ʒ������
    public Transform topCanvasTrans;
    public Transform itemTrans;

    //�Ƿ���סUI����
    //public bool Lock;

    //�洢��������
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    
    //�洢UI���������
    private List<GameObject> UIObjList = new List<GameObject>();

    private UIManager()
    {
        //Lock = false;
        //����Canvas
        GameObject canvasObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas"));
        canvasTrans = canvasObj.transform;
        GameObject topCanvasObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/TopCanvas"));
        topCanvasTrans = topCanvasObj.transform;
        itemTrans = topCanvasTrans.Find("ItemTrans");
        //��������ɾ��
        GameObject.DontDestroyOnLoad(canvasObj);
        GameObject.DontDestroyOnLoad(topCanvasObj);
    }

    /// <summary>
    /// ��ʾ���
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
    /// <returns>��Ӧ���ű�</returns>
    public T ShowPanel<T>() where T : BasePanel
    {
        //�������
        string panelName = typeof(T).Name;
        //����Ѿ������ˣ���ֱ�ӷ������
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        //����������
        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/Panel/" + panelName));
        //��ȡ���ű�
        T panel = panelObj.GetComponent<T>();
        //���ø�����
        if (panel.isOnTop) panelObj.transform.SetParent(topCanvasTrans, false);
        else panelObj.transform.SetParent(canvasTrans, false);
        //ִ����ʾ����
        panel.ShowMe();
        //��ӽ�������
        panelDic.Add(panelName, panel);
        //����������
        return panel;
    }

    /// <summary>
    /// �������
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
    /// <param name="isFade">�Ƿ񵭳���Ĭ��Ϊtrue,������</param>
    /// <param name="callback">�ص�����</param>
    public void HidePanel<T>(bool isFade = true, UnityAction callback = null) where T : BasePanel
    {
        //�������
        string panelName = typeof(T).Name;

        if (panelDic.ContainsKey(panelName))
        {
            //��ȡ���
            T panel = panelDic[panelName] as T;
            if (isFade)
            {
                panel.HideMe(() =>
                {
                    callback?.Invoke();
                    //�Ƴ��Լ�
                    GameObject.Destroy(panel.gameObject);
                    //�Ƴ������е�����
                    panelDic.Remove(panelName);
                });
            }
            else
            {
                callback?.Invoke();
                //ֱ���Ƴ��Լ�
                GameObject.Destroy(panel.gameObject);
                //�Ƴ������е�����
                panelDic.Remove(panelName);
            }
        }
    }

    /// <summary>
    /// �õ����
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
    /// <returns>��Ӧ���</returns>
    public T GetPanel<T>() where T : BasePanel
    {
        //�������
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        return null;
    }

    public void LoadScene(string sceneName,UnityAction fadeInCallback = null, UnityAction completedCallback = null)
    {
        LoadingPanel panel = ShowPanel<LoadingPanel>();
        panel.LoadScene(sceneName, fadeInCallback, completedCallback);
    }

    /// <summary>
    /// ͨ������ش���UI����
    /// </summary>
    /// <param name="resName">��Դ·����</param>
    /// <returns>����</returns>
    public GameObject CreateUIObjByPoolMgr(string resName)
    {
        GameObject UIObj = PoolMgr.Instance.GetUIObj(resName); //�Ӷ�����л�ȡ����
        UIObjList.Add(UIObj);
        return UIObj;
    }

    /// <summary>
    /// ����UI����
    /// </summary>
    /// <param name="resName">��Դ·����</param>
    /// <returns>����</returns>
    public GameObject CreateUIObj(string resName,Transform parent)
    {
        GameObject UIObj = GameObject.Instantiate(Resources.Load<GameObject>(resName),parent); //�Ӷ�����л�ȡ����
        UIObj.transform.SetAsLastSibling();
        UIObjList.Add(UIObj);
        return UIObj;
    }

    /// <summary>
    /// ͨ�������ɾ��UI����
    /// </summary>
    /// <param name="UIObj">Ҫɾ����UI����</param>
    public void DestroyUIObjByPoolMgr(GameObject UIObj)
    {
        if (UIObjList.Contains(UIObj))
        {
            UIObjList.Remove(UIObj);
            PoolMgr.Instance.PushUIObj(UIObj);
        }
    }

    /// <summary>
    /// ͨ�������ɾ��UI����
    /// </summary>
    /// <param name="UIObj">Ҫɾ����UI����</param>
    public void DestroyUIObj(GameObject UIObj)
    {
        if (UIObjList.Contains(UIObj))
        {
            UIObjList.Remove(UIObj);
            GameObject.Destroy(UIObj);
        }
    }

    /// <summary>
    /// ��ʾ�����ı�
    /// </summary>
    /// <param name="txt">�ı�</param>
    /// <param name="color">��ɫ</param>
    /// <param name="pos">λ��</param>
    public void ShowTxtPopup(string txt,Color color, float size, Vector2 pos,bool isUI = false)
    {
        GameObject obj = CreateUIObjByPoolMgr("UI/UIObj/TxtPopup");
        UIPopup txtPopup = obj.GetComponent<UIPopup>();
        if (isUI)
            txtPopup.InitUI(txt, color, size, pos);
        else
            txtPopup.Init(txt, color, size, pos);
    }

    /// <summary>
    /// ��ʾ��ʾ��Ϣ
    /// </summary>
    /// <param name="info">��Ϣ</param>
    public void ShowTipInfo(string info)
    {
        GameObject obj = CreateUIObj("UI/UIObj/TipInfo", topCanvasTrans);
        obj.GetComponent<TipInfo>().Init(info);
    }
    /// <summary>
    /// ��������UI���壨������ʱʹ�ã�
    /// </summary>
    public void ClearAllUIObj()
    {
        foreach (GameObject UIObj in UIObjList)
        {
            GameObject.Destroy(UIObj);
        }
        UIObjList.Clear();
    }

    ///// <summary>
    ///// ��Ϸ��������ʾ���
    ///// </summary>
    ///// <param name="isWin"></param>
    //public void ShowGameOverPanel(bool isWin)
    //{
    //    //����ս��
    //    LevelManager.Instance.isInLevel = false;
    //    LevelManager.Instance.Clear();
    //    TowerManager.Instance.Clear();
    //    EnemyManager.Instance.Clear();
    //    PoolMgr.Instance.ClearPool(); //�л�����ǰ����������

    //    //��ʾ���
    //    TipPanel panel = ShowPanel<TipPanel>();
    //    string info = isWin ? "ʤ����" : "ʧ�ܣ�";
    //    panel.SetInfo(info, () =>
    //    {
    //        //���ص�ͼ��壨TODO:����ʤ��ѡ������壩
    //        LoadScene("MapScene", () =>
    //        {
    //            HidePanel<TowerPanel>();
    //            ShowPanel<RewardPanel>(); //��ʾ����
    //        });
    //    });
    //    panel.AddCancelBtnCallBack(() =>
    //    {
    //        //���ص�ͼ��壬��ȷ�ϰ�ť�߼�һ��
    //        LoadScene("MapScene", () =>
    //        {
    //            HidePanel<TowerPanel>();
    //            ShowPanel<MapPanel>();
    //        });
    //    });
    //}
}
