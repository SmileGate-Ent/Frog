using TMPro;
using UnityEngine;

public class FrogCanvas : MonoBehaviour
{
    public static FrogCanvas Instance;
    
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] ConfirmPopup confirmPopupPrefab;
    [SerializeField] Sprite exitBgSprite;
    [SerializeField] Sprite btn1Sprite;
    [SerializeField] Sprite btn2Sprite;

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

    public ConfirmPopup InstantiateTextConfirmPopup()
    {
        return Instantiate(confirmPopupPrefab, transform);
    }

    public ConfirmPopup InstantiateExitPopup()
    {
        var popup = Instantiate(confirmPopupPrefab, transform);
        popup.BgSprite = exitBgSprite;
        popup.Btn1Sprite = btn1Sprite;
        popup.Btn2Sprite = btn2Sprite;
        return popup;
    }
}
