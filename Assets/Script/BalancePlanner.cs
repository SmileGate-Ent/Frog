using UnityEngine;

public class BalancePlanner : MonoBehaviour
{
    public static BalancePlanner Instance;
    
    [SerializeField] BalancePlan[] planList;

    int curPlanIndex;
    float gameTime;

    public BalancePlan CurrentPlan => planList[curPlanIndex];
    
    BalancePlan NextPlan => curPlanIndex + 1 < planList.Length ? planList[curPlanIndex + 1] : null;

    public float GameTime => gameTime;

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
        gameTime += Time.deltaTime;

        if (NextPlan != null && NextPlan.ApplyTime <= gameTime)
        {
            curPlanIndex++;
            Debug.Log($"Current plan: {CurrentPlan}", CurrentPlan);
        }
    }
}