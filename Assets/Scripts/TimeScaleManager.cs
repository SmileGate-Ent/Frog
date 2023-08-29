using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    public static TimeScaleManager Instance;

    [SerializeField] int pauseCounter;

    void Awake()
    {
        Instance = this;
    }

    public void IncrPause()
    {
        pauseCounter++;
        UpdatePauseState();
    }

    public void DescPause()
    {
        pauseCounter--;
        UpdatePauseState();
    }

    void UpdatePauseState()
    {
        Time.timeScale = pauseCounter > 0 ? 0 : 1;
    }
}