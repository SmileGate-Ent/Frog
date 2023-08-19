using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject credit;
    [SerializeField] private GameObject setting;

    public void StartButton()
    {
        AudioManager.Instance.PlayBtnClick();
        SceneManager.LoadScene("InGame");
    }

    public void SettingButton()
    {
        AudioManager.Instance.PlayBtnClick();
        setting.SetActive(true);
    }

    public void CreditButton()
    {
        AudioManager.Instance.PlayBtnClick();
        credit.SetActive(true);
    }

    public void ExitButton()
    {
        var popup = FrogCanvas.Instance.InstantiateConfirmPopup();
        popup.Text = "정말 떠나실 거예요? ㅜ.ㅠ";
        popup.Btn1Text = "예";
        popup.Btn2Text = "아니요";
        popup.OnBtn1 = () =>
        {
            AudioManager.Instance.PlayBtnClick();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        };
        popup.OnBtn2 = () => { Destroy(popup.gameObject); };
    }

    public void GotoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void ReStart()
    {
        SceneManager.LoadScene("InGame");
    }

    public void CloseSetting()
    {
        setting.SetActive(false);
    }

    public void CloseCreditButton()
    {
        credit.SetActive(false);
    }
}