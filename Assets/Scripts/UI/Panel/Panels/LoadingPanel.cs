using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : BasePanel
{
    public float duration = 1f;
    private float timer;
    private UnityAction completedCallback; //场景加载完成后的回调
    private UnityAction fadeInCallback; //淡入完后的回调（黑屏时）
    private string sceneName;
    private AsyncOperation operation;

    public override void Init()
    {
        timer = 0;
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="fadeInCallback">淡入完成后的回调</param>
    /// <param name="completedCallback">场景加载完成后的回调</param>
    public void LoadScene(string sceneName,UnityAction fadeInCallback, UnityAction completedCallback)
    {
        this.sceneName = sceneName;
        this.completedCallback = completedCallback;
        this.fadeInCallback = fadeInCallback;

        operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        operation.completed += (obj) =>
        {
            this.completedCallback?.Invoke(); //加载完成，调用回调
            this.completedCallback = null;
            UIManager.Instance.HidePanel<LoadingPanel>();
        };

        timer = 0;
    }

    protected override void Update()
    {
        base.Update();
        //淡入完成才开始计时
        if (canvasGroup.alpha == 1)
        {
            fadeInCallback?.Invoke(); //淡入完成，调用回调
            fadeInCallback = null;
            timer += Time.deltaTime;
            if (timer > duration && operation.progress >= 0.9f)
            {
                timer = 0;
                operation.allowSceneActivation = true;
            }
        }
    }
}
