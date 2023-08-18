using UnityEngine;

public class Frog : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    
    void Update()
    {
        var dx = Input.GetAxisRaw("Horizontal");
        var dy = Input.GetAxisRaw("Vertical");
        if (dx == 0 && dy == 0)
        {
            return;
        }

        var d = new Vector2(dx, dy).normalized;

        transform.Translate(d * (moveSpeed * Time.deltaTime), Space.Self); 
    }
}
