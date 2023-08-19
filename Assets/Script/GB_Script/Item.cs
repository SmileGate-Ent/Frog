using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] LayerMask tongueLayer;
    [SerializeField] LayerMask frogBodyLayer;
    [SerializeField] Sprite[] spriteList;
    [SerializeField] SpriteRenderer spriteRenderer;
    
    void Awake()
    {
        spriteRenderer.sprite = spriteList[Random.Range(0, spriteList.Length)];
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if ((tongueLayer.value & (1 << col.gameObject.layer)) != 0)
        {
            //if (Frog.Instance.CanCatch)
            {
                Frog.Instance.AttachItemToTongue(this);
            }
        }
        else if ((frogBodyLayer.value & (1 << col.gameObject.layer)) != 0)
        {
            //if (Frog.Instance.IsAttachedToTongue(this))
            {
                Frog.Instance.Score++;
                Frog.Instance.Hp += 5;
                Frog.Instance.PlayScoreClip();
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogError("Unknown collision layer");
        }
    }
}