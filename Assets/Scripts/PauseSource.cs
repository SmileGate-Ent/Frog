using UnityEngine;

public class PauseSource : MonoBehaviour
{
    void OnEnable()
    {
        TimeScaleManager.Instance.IncrPause();
    }

    void OnDisable()
    {
        TimeScaleManager.Instance.DescPause();
    }
}