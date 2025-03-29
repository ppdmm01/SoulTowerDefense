using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    //场景上的Canvas
    public Transform canvasTrans;
    //放置物品的容器
    public Transform topCanvasTrans;
    public Transform itemTrans;

    //是否锁住UI操作
    //public bool Lock;

    //存储面板的容器
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    
    //存储UI物体的容器
    private List<GameObject> UIObjList = new List<GameObject>();

    private UIManager()
    {
        //Lock = false;
        //创建Canvas
        GameObject canvasObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas"));
        canvasTrans = canvasObj.transform;
        GameObject topCanvasObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/TopCanvas"));
        topCanvasTrans = topCanvasObj.transform;
        itemTrans = topCanvasTrans.Find("ItemTrans");
        //过场景不删除
        GameObject.DontDestroyOnLoad(canvasObj);
        GameObject.DontDestroyOnLoad(topCanvasObj);
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <returns>对应面板脚本</returns>
    public T ShowPanel<T>() where T : BasePanel
    {
        //面板名字
        string panelName = typeof(T).Name;
        //如果已经创建了，就直接返回面板
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        //加载面板对象
        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/Panel/" + panelName));
        //获取面板脚本
        T panel = panelObj.GetComponent<T>();
        //设置父对象
        if (panel.isOnTop) panelObj.transform.SetParent(topCanvasTrans, false);
        else panelObj.transform.SetParent(canvasTrans, false);
        //执行显示方法
        panel.ShowMe();
        //添加进容器中
        panelDic.Add(panelName, panel);
        //返回面板对象
        return panel;
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <param name="isFade">是否淡出（默认为true,淡出）</param>
    /// <param name="callback">回调函数</param>
    public void HidePanel<T>(bool isFade = true, UnityAction callback = null) where T : BasePanel
    {
        //面板名字
        string panelName = typeof(T).Name;

        if (panelDic.ContainsKey(panelName))
        {
            //获取面板
            T panel = panelDic[panelName] as T;
            if (isFade)
            {
                panel.HideMe(() =>
                {
                    callback?.Invoke();
                    //移除自己
                    GameObject.Destroy(panel.gameObject);
                    //移除容器中的数据
                    panelDic.Remove(panelName);
                });
            }
            else
            {
                callback?.Invoke();
                //直接移除自己
                GameObject.Destroy(panel.gameObject);
                //移除容器中的数据
                panelDic.Remove(panelName);
            }
        }
    }

    /// <summary>
    /// 得到面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <returns>对应面板</returns>
    public T GetPanel<T>() where T : BasePanel
    {
        //面板名字
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
    /// 通过对象池创建UI物体
    /// </summary>
    /// <param name="resName">资源路径名</param>
    /// <returns>物体</returns>
    public GameObject CreateUIObjByPoolMgr(string resName)
    {
        GameObject UIObj = PoolMgr.Instance.GetUIObj(resName); //从对象池中获取对象
        UIObjList.Add(UIObj);
        return UIObj;
    }

    /// <summary>
    /// 创建UI物体
    /// </summary>
    /// <param name="resName">资源路径名</param>
    /// <returns>物体</returns>
    public GameObject CreateUIObj(string resName,Transform parent)
    {
        GameObject UIObj = GameObject.Instantiate(Resources.Load<GameObject>(resName),parent); //从对象池中获取对象
        UIObj.transform.SetAsLastSibling();
        UIObjList.Add(UIObj);
        return UIObj;
    }

    /// <summary>
    /// 通过对象池删除UI物体
    /// </summary>
    /// <param name="UIObj">要删除的UI物体</param>
    public void DestroyUIObjByPoolMgr(GameObject UIObj)
    {
        if (UIObjList.Contains(UIObj))
        {
            UIObjList.Remove(UIObj);
            PoolMgr.Instance.PushUIObj(UIObj);
        }
    }

    /// <summary>
    /// 通过对象池删除UI物体
    /// </summary>
    /// <param name="UIObj">要删除的UI物体</param>
    public void DestroyUIObj(GameObject UIObj)
    {
        if (UIObjList.Contains(UIObj))
        {
            UIObjList.Remove(UIObj);
            GameObject.Destroy(UIObj);
        }
    }

    /// <summary>
    /// 显示跳字文本
    /// </summary>
    /// <param name="txt">文本</param>
    /// <param name="color">颜色</param>
    /// <param name="pos">位置</param>
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
    /// 显示提示信息
    /// </summary>
    /// <param name="info">信息</param>
    public void ShowTipInfo(string info)
    {
        GameObject obj = CreateUIObj("UI/UIObj/TipInfo", topCanvasTrans);
        obj.GetComponent<TipInfo>().Init(info);
    }
    /// <summary>
    /// 清理所有UI物体（过场景时使用）
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
    ///// 游戏结束，显示面板
    ///// </summary>
    ///// <param name="isWin"></param>
    //public void ShowGameOverPanel(bool isWin)
    //{
    //    //清理战场
    //    LevelManager.Instance.isInLevel = false;
    //    LevelManager.Instance.Clear();
    //    TowerManager.Instance.Clear();
    //    EnemyManager.Instance.Clear();
    //    PoolMgr.Instance.ClearPool(); //切换场景前先清除对象池

    //    //显示面板
    //    TipPanel panel = ShowPanel<TipPanel>();
    //    string info = isWin ? "胜利！" : "失败！";
    //    panel.SetInfo(info, () =>
    //    {
    //        //返回地图面板（TODO:跳到胜利选奖励面板）
    //        LoadScene("MapScene", () =>
    //        {
    //            HidePanel<TowerPanel>();
    //            ShowPanel<RewardPanel>(); //显示奖励
    //        });
    //    });
    //    panel.AddCancelBtnCallBack(() =>
    //    {
    //        //返回地图面板，和确认按钮逻辑一样
    //        LoadScene("MapScene", () =>
    //        {
    //            HidePanel<TowerPanel>();
    //            ShowPanel<MapPanel>();
    //        });
    //    });
    //}
}
