using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ConfirmPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI btn1Text;
    [SerializeField] TextMeshProUGUI btn2Text;

    UnityAction onBtn1;
    UnityAction onBtn2;
    
    public UnityAction OnBtn1
    {
        set => onBtn1 = value;
    }
    
    public UnityAction OnBtn2
    {
        set => onBtn2 = value;
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