using UnityEngine;

/// <summary>
/// Buff效果（目前只会添加给敌人，若后续需修改，可以把Tower和Enemy重构下，新建一个类作为它们的父类）
/// </summary>
public class Buff : MonoBehaviour
{
    public Enemy target; //目标
    public BuffData data; //数据
    private float timer; //buff触发计时器
    private float totalTimer; //buff持续时间计时器

    protected virtual void Start()
    {
        timer = 0;
        totalTimer = 0;
        OnApply();
    }

    protected virtual void Update()
    {
        timer += Time.deltaTime;
        totalTimer += Time.deltaTime;
        if (totalTimer < data.duration)
        {
            if (timer >= data.triggerInterval)
            {
                timer = 0;
                OnTrigger(); //触发一次效果
            }
        }
        else
        {
            Destroy(this); //销毁自己
        }
    }

    protected virtual void OnDestroy()
    {
        OnExit();
    }

    //初始化
    public void Init(BuffData data,Enemy target)
    {
        this.data = data;
        this.target = target;
    }

    //获得时触发
    public void OnApply()
    {
        target.speedMultiplier = data.speedMultiplier;
        target.damageMultiplier = data.woundMultiplier;
    }

    //每隔一段时间触发
    public void OnTrigger()
    {
        if (data.isTriggerOverTime)
        {
            if (data.damage != 0)
                target.Wound(data.damage,Color.red);
        }
    }

    //结束时触发
    public void OnExit()
    {
        target.speedMultiplier = 1;
        target.damageMultiplier = 1;
    }

    //重置计时器（用于不堆叠的buff，重新计算时间）
    public void ResetTime()
    {
        totalTimer = 0;
    }
}
