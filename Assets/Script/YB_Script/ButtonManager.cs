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
        SceneManager.LoadScene("YB_Scene_Main");
    }
    public void SettingButton()
    {
        setting.SetActive(true);
    }
    public void CreditButton()
    {
        credit.SetActive(true);
    }
    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
