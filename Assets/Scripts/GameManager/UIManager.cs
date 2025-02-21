using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    //场景上的Canvas
    public Transform canvasTrans;

    //存储面板的容器
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    
    //存储UI物体的容器
    private List<GameObject> UIObjList = new List<GameObject>();

    private UIManager()
    {
        //创建Canvas
        GameObject canvasObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas"));
        canvasTrans = canvasObj.transform;
        //过场景不删除
        GameObject.DontDestroyOnLoad(canvasObj);
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
        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName));
        //设置父对象
        panelObj.transform.SetParent(canvasTrans, false);
        //获取面板脚本
        T panel = panelObj.GetComponent<T>();
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
    public void ShowTxtPopup(string txt,Color color, Vector2 pos)
    {
        GameObject obj = CreateUIObjByPoolMgr("UI/Popup/TxtPopup");
        UIPopup txtPopup = obj.GetComponent<UIPopup>();
        txtPopup.Init(txt, color, pos);
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
}
