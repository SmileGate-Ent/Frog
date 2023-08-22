using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private int _frogNumber;

    public void SetFrogNumber(int n)
    {
        _frogNumber = n;
    }

    public int GetFrogNumber()
    {
        return _frogNumber;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
