using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID
using GooglePlayGames;
#endif

public class LogoScene : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
        
#if UNITY_ANDROID
        PlayGamesPlatform.Activate();
#endif
        Social.localUser.Authenticate(ProcessAuthentication);
    }
    
    static void ProcessAuthentication(bool result)
    {
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("TitleScene");
    }
}
