using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI FPSText = null;

    void Update()
    {
        FPSText.text = (1 / Time.deltaTime).ToString("000") + " FPS";
    }
}
