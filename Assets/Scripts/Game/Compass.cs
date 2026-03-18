using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private RectTransform needleImage; // Assign your needle/rose UI Image here
    [SerializeField] private TextMeshProUGUI directionText;

    private readonly string[] directions =
   {
        "N", "NE", "E", "SE", "S", "SW", "W", "NW"
    };

    void Update()
    {
        float yaw = player.eulerAngles.y;

        // Rotate needle
        needleImage.localRotation = Quaternion.Euler(0f, 0f, yaw);

        directionText.text = GetDirection(yaw);
    }

    private string GetDirection(float yaw)
    {
        int index = Mathf.RoundToInt(yaw / 45f) % 8;
        int angle = Mathf.RoundToInt(yaw);

        return $"{angle}° {directions[index]}";
    }
}