using UnityEngine;

[CreateAssetMenu(menuName = "Frog/Character Preset")]
public class CharacterPreset : ScriptableObject
{
    [SerializeField] Sprite frogIdleSprite;
    [SerializeField] Sprite frogJumpSprite;
    [SerializeField] Sprite frogAttackSprite;
}
