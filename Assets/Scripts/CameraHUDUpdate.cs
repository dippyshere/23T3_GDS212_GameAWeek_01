using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraHUDUpdate : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraController CameraController;
    [SerializeField] private TextMeshProUGUI hudTimetsamp;
    [SerializeField] private TextMeshProUGUI hudInfo;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateTimestampCoroutine());
        StartCoroutine(UpdateInfoCoroutine());
    }

    IEnumerator UpdateTimestampCoroutine()
    {
        while (true)
        {
            hudTimetsamp.text = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            yield return new WaitForSeconds(0.08f);
        }
    }

    IEnumerator UpdateInfoCoroutine()
    {
        while (true)
        {
            hudInfo.text = $"2560 x 1920 @ {(int)(1f / Time.deltaTime)}fps";
            yield return new WaitForSeconds(1f);
        }
    }
}
