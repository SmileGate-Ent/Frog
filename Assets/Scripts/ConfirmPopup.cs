using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI btn1Text;
    [SerializeField] TextMeshProUGUI btn2Text;

    [SerializeField] Image bgImage;
    [SerializeField] Image btn1Image;
    [SerializeField] Image btn2Image;

    UnityAction onBtn1;
    UnityAction onBtn2;

    public Sprite BgSprite
    {
        set
        {
            bgImage.sprite = value;
            Text = string.Empty;
        }
    }

    public Sprite Btn1Sprite
    {
        set
        {
            btn1Image.sprite = value;
            Btn1Text = string.Empty;
        }
    }

    public Sprite Btn2Sprite
    {
        set
        {
            btn2Image.sprite = value;
            Btn2Text = string.Empty;
        }
    }

    public UnityAction OnBtn1
    {
        set => onBtn1 = value;
    }

    public UnityAction OnBtn2
    {
        set => onBtn2 = value;
    }

    public string Text
    {
        set => text.text = value;
    }

    public string Btn1Text
    {
        set => btn1Text.text = value;
    }

    public string Btn2Text
    {
        set => btn2Text.text = value;
    }

    public void OnBtn1Click()
    {
        onBtn1.Invoke();
    }

    public void OnBtn2Click()
    {
        onBtn2.Invoke();
    }
}