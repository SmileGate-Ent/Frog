using UnityEngine;

public class PauseSource : MonoBehaviour
{
    void OnEnable()
    {
        if (TimeScaleManager.Instance != null)
        {
            TimeScaleManager.Instance.IncrPause();
        }
    }

    void OnDisable()
    {
        if (TimeScaleManager.Instance != null)
        {
            TimeScaleManager.Instance.DescPause();
        }
    }
}