using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] LayerMask tongueLayer;
    [SerializeField] LayerMask frogBodyLayer;

    void OnTriggerEnter2D(Collider2D col)
    {
        if ((tongueLayer.value & (1 << col.gameObject.layer)) != 0)
        {
            Frog.Instance.AttachItemToTongue(this);
        }
        else if ((frogBodyLayer.value & (1 << col.gameObject.layer)) != 0)
        {
            Frog.Instance.Score++;
            Frog.Instance.PlayScoreClip();
            Destroy(gameObject);
            
        }
        else
        {
            Debug.LogError("Unknown collision layer");
        }
    }
}