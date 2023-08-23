using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Sprite[] tutorialSprite;
    
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;
    
    [SerializeField] private int nowPage;
    [SerializeField] Image image;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Back()
    {
        nowPage--;
        if(nowPage == 0) leftButton.SetActive(false);
        rightButton.SetActive(true);

        image.sprite = tutorialSprite[nowPage]; 
    }
    public void Close()
    {
       gameObject.SetActive(false);
    }
    public void Next()
    {
        nowPage++;
        if(nowPage == tutorialSprite.Length-1) rightButton.SetActive(false);
        leftButton.SetActive(true);

        image.sprite = tutorialSprite[nowPage];
    }
}
