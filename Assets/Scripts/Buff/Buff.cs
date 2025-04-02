using UnityEngine;

/// <summary>
/// BuffЧ����Ŀǰֻ����Ӹ����ˣ����������޸ģ����԰�Tower��Enemy�ع��£��½�һ������Ϊ���ǵĸ��ࣩ
/// </summary>
public class Buff : MonoBehaviour
{
    public Enemy target; //Ŀ��
    public BuffData data; //����
    private float timer; //buff������ʱ��
    private float totalTimer; //buff����ʱ���ʱ��

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
                OnTrigger(); //����һ��Ч��
            }
        }
        else
        {
            Destroy(this); //�����Լ�
        }
    }

    protected virtual void OnDestroy()
    {
        OnExit();
    }

    //��ʼ��
    public void Init(BuffData data,Enemy target)
    {
        this.data = data;
        this.target = target;
    }

    //���ʱ����
    public void OnApply()
    {
        target.speedMultiplier = data.speedMultiplier;
        target.damageMultiplier = data.woundMultiplier;
    }

    //ÿ��һ��ʱ�䴥��
    public void OnTrigger()
    {
        if (data.isTriggerOverTime)
        {
            if (data.damage != 0)
                target.Wound(data.damage,Color.red);
        }
    }

    //����ʱ����
    public void OnExit()
    {
        target.speedMultiplier = 1;
        target.damageMultiplier = 1;
    }

    //���ü�ʱ�������ڲ��ѵ���buff�����¼���ʱ�䣩
    public void ResetTime()
    {
        totalTimer = 0;
    }
}
