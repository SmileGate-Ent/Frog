using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("TitleScene");
    }
}
