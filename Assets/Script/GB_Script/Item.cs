using UnityEngine;

public class Item : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(gameObject);

        Frog.Instance.Score++;
        Frog.Instance.PlayScoreClip();
    }
}
