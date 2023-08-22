using TMPro;
using UnityEngine;

public class TextCopier : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI original;
    [SerializeField] TextMeshProUGUI self;

    void LateUpdate()
    {
        self.text = original.text;
    }
}
