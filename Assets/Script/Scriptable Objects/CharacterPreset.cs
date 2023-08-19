using UnityEngine;

[CreateAssetMenu(menuName = "Frog/Character Preset")]
public class CharacterPreset : ScriptableObject
{
    [SerializeField] Sprite idleSprite;
    [SerializeField] Sprite attackSprite;
    [SerializeField] Sprite jumpSprite;

    public Sprite IdleSprite => idleSprite;
    public Sprite AttackSprite => attackSprite;
    public Sprite JumpSprite => jumpSprite;
}
