using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] LayerMask tongueLayer;
    [SerializeField] LayerMask frogBodyLayer;

    // 먹었을 때 채워지는 HP양
    [SerializeField] int deltaHp = 5;
    
    // 먹었을 때 증가하는 점수
    [SerializeField] int deltaScore = 1;
    
    // 먹었을 때 변경되는 외형
    [SerializeField] CharacterPreset preset;

    /*
    [SerializeField] Sprite[] spriteList;
    [SerializeField] SpriteRenderer spriteRenderer;
    
    void Awake()
    {
        spriteRenderer.sprite = spriteList[Random.Range(0, spriteList.Length)];
    }
    */

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
                Frog.Instance.Score += deltaScore;
                Frog.Instance.Hp += deltaHp;
                if (deltaHp < 0)
                {
                    Frog.Instance.PlayDamageClip();
                }
                else
                {
                    Frog.Instance.PlayScoreClip();
                }

                Destroy(gameObject);

                // 외형 변경!!!
                if (preset != null)
                {
                    Frog.Instance.Preset = preset;
                }
            }
        }
        else
        {
            Debug.LogError("Unknown collision layer");
        }
    }
}