using TMPro;
using UnityEngine;

public class FrogCanvas : MonoBehaviour
{
    public static FrogCanvas Instance;
    
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] ConfirmPopup confirmPopupPrefab;

    public string HpText
    {
        set => hpText.text = value;
    }

    public string ScoreText
    {
        set => scoreText.text = value;
    }
    
    void Awake()
    {
        Instance = this;
    }

    public ConfirmPopup InstantiateConfirmPopup()
    {
        return Instantiate(confirmPopupPrefab, transform);
    }
}
