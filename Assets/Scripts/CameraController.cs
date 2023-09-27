using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float interpSpeed = 2.0f;
    [SerializeField] private float interpRotationSpeed = 2.0f;
    [SerializeField] private Vector3 defaultPosition;
    [SerializeField] private Vector3 defaultRotation;
    [SerializeField] private Vector3 aimPosition;
    [SerializeField] private Vector3 aimRotation;

    [Header("References")]
    [SerializeField] private GameObject cameraObject;

    private bool isAiming = false;
    private Camera cameraComponent => cameraObject.GetComponent<Camera>();
    private bool wasInactive = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator InterpCamera(Vector3 position, Vector3 rotation, bool aimSwitch)
    {
        while (Vector3.Distance(transform.localPosition, position) > 0.01f)
        {
            if (aimSwitch && !isAiming)
            {
                yield break;
            }
            if (!aimSwitch && isAiming)
            {
                yield break;
            }
            transform.localPosition = Vector3.Lerp(transform.localPosition, position, interpSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotation), interpRotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void Aim()
    {
        isAiming = true;
        StartCoroutine(InterpCamera(aimPosition, aimRotation, true));
    }

    public void Unaim()
    {
        isAiming = false;
        StartCoroutine(InterpCamera(defaultPosition, defaultRotation, false));
    }

    public void TakePhoto()
    {
        if (!cameraObject.activeSelf)
        {
            cameraObject.SetActive(true);
            wasInactive = true;
        }
        else
        {
            wasInactive = false;
        }
        RenderTexture oldRT = cameraComponent.targetTexture;
        RenderTexture tempRT = RenderTexture.GetTemporary(2560, 1920, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 2);
        cameraComponent.targetTexture = tempRT;
        cameraComponent.Render();
        RenderTexture.active = tempRT;
        Texture2D photo = new Texture2D(2560, 1920, TextureFormat.RGB24, false);
        photo.ReadPixels(new Rect(0, 0, 2560, 1920), 0, 0);
        photo.Apply();
        cameraComponent.targetTexture = oldRT;
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(tempRT);
        if (wasInactive)
        {
            cameraObject.SetActive(false);
        }
        string uuid = System.Guid.NewGuid().ToString();
        byte[] bytes = photo.EncodeToPNG();
        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/Photos"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Photos");
        }
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/Photos/" + uuid + ".png", bytes);
        Debug.Log("Saved photo to " + Application.persistentDataPath + "/Photos/" + uuid + ".png");
        string photos = PlayerPrefs.GetString("photos", "");
        photos += uuid + ",";
        PlayerPrefs.SetString("photos", photos);
        ScorePhoto(uuid);
    }

    private void ScorePhoto(string uuid)
    {
        
    }
}
