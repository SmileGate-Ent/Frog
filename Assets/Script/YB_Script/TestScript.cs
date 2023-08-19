using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) GameManager.Instance.SetFrogNumber(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) GameManager.Instance.SetFrogNumber(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) GameManager.Instance.SetFrogNumber(2);
        tmpText.text = GameManager.Instance.GetFrogNumber().ToString();
    }

    public void GameStart()
    {
        SceneManager.LoadScene("YB_Scene_Main");
    }
}
