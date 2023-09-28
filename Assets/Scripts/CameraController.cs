using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Animator photoPreviewAnimator;
    [SerializeField] private Animator photoFlashAnimator;
    [SerializeField] private RawImage photoPreview;
    [SerializeField] private TextMeshProUGUI photoPreviewText;
    [SerializeField] private PhotoScoringManager photoScoringManager;

    [Header("Debug")]
    [SerializeField] private GameObject debugPhotoObject;

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
        photoPreviewAnimator.SetTrigger("TakePhoto");
        photoFlashAnimator.SetTrigger("TakePhoto");
        StartCoroutine(CapturePhoto());
    }

    IEnumerator CapturePhoto()
    {
        // Waits for the capture animations to start (hides hitch)
        yield return null;
        // related to hack to avoid allocation vram leak bug
        if (!cameraObject.activeSelf)
        {
            cameraObject.SetActive(true);
            wasInactive = true;
        }
        else
        {
            wasInactive = false;
        }
        // tempararily create a new RT for capturing at a higher resolution with AA
        RenderTexture oldRT = cameraComponent.targetTexture;
        RenderTexture tempRT = RenderTexture.GetTemporary(2560, 1920, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 2);
        cameraComponent.targetTexture = tempRT;
        cameraComponent.Render();
        RenderTexture.active = tempRT;
        // convert the RT to a texture
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
        photoPreview.texture = photo;
        photoPreviewText.text = debugPhotoObject.name;
        if (Random.Range(0, 2) == 0)
        {
            photoPreview.transform.parent.rotation = Quaternion.Euler(0, 0, Random.Range(-12f, -5f));
        }
        else
        {
            photoPreview.transform.parent.rotation = Quaternion.Euler(0, 0, Random.Range(5f, 12f));
        }
        // assign a unique identifier (uuidv4) to the photo, append it to the list of photos, and save it to disk
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
        List<float> photoScore = photoScoringManager.ScorePhoto(debugPhotoObject);
        Debug.Log("Photo score: Visibility: " + photoScore[0] + " Object Visibility: " + photoScore[1] + " Object Size: " + photoScore[2] + " Centering: " + photoScore[3] + " Rule of Thirds: " + photoScore[4]);
        PlayerPrefs.SetString(uuid, string.Join(",", photoScore));
    }
}
