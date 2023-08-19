using UnityEngine;

[CreateAssetMenu(menuName = "Frog/Character Preset")]
public class CharacterPreset : ScriptableObject
{
    [SerializeField] Sprite idleSprite;
    [SerializeField] Sprite attackSprite;
    [SerializeField] Sprite jumpSprite;
}
