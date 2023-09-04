using UnityEngine;

public class BalancePlanner : MonoBehaviour
{
    public static BalancePlanner Instance;

    [SerializeField] BalancePlan[] planList;
    [SerializeField] float balanceTimeScale = 1;

    int curPlanIndex;
    float gameTime;

    public BalancePlan CurrentPlan => planList[curPlanIndex];

    BalancePlan NextPlan => curPlanIndex + 1 < planList.Length ? planList[curPlanIndex + 1] : null;

    public float GameTime => gameTime;

    public float BalanceTimeScale => balanceTimeScale;
    
    public float BalanceDeltaTime => Time.deltaTime * BalanceTimeScale;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log($"Current plan: {CurrentPlan}", CurrentPlan);
    }

    void Update()
    {
        gameTime += BalanceDeltaTime;

        if (NextPlan != null && NextPlan.ApplyTime <= gameTime)
        {
            curPlanIndex++;
            Debug.Log($"Current plan: {CurrentPlan}", CurrentPlan);
        }
    }
}