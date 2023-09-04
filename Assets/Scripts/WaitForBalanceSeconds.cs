using UnityEngine;

public class WaitForBalanceSeconds : CustomYieldInstruction
{
    float sec;

    public WaitForBalanceSeconds(float inSec)
    {
        sec = inSec;
    }

    public override bool keepWaiting
    {
        get
        {
            sec -= BalancePlanner.Instance.BalanceDeltaTime;
            return sec > 0;
        }
    }
}