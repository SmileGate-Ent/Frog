using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject credit;
    [SerializeField] private GameObject setting;
    public GameObject tuto;
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
    public void TutoButton()
    {
        tuto.SetActive(true);
    }

    public void ExitButton()
    {
        var popup = FrogCanvas.Instance.InstantiateExitPopup();
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
    
    public void OpenAchievements()
    {
        Social.ShowAchievementsUI();
    }

    public void OpenLeaderboards()
    {
        Social.ShowLeaderboardUI();
    }
}