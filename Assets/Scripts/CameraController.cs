using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    static readonly int Photo = Animator.StringToHash("TakePhoto");

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
    [SerializeField] private TextMeshProUGUI photoScores;

    [Header("Debug")]
    [SerializeField] private GameObject debugPhotoObject;

    private bool isAiming = false;
    private Camera cameraComponent => cameraObject.GetComponent<Camera>();
    private bool wasInactive = false;
    private GameObject captureObject;

    private string[] compliments = {
        "Great shot!",
        "Absolutely stunning!",
        "Impressive composition!",
        "Wonderful capture!",
        "Incredible work!",
        "You nailed it!",
        "Bravo!",
        "A work of art!",
        "This is picture-perfect!"
    };

    private string[] catCompliments =
    {
        "You've got purr-fect photography skills!",
        "This photo is the cat's meow!",
        "Your photography skills are claw-some!",
        "This shot is absolutely fur-tastic!",
        "Meowgnificent work!",
        "Your photography is the cat-ch of the day!",
        "Pawsitively impressive!"
    };

    private string[] dissapointments =
    {
        "While the object may be missing, your photography skills still shine!",
        "Even without the object, your photo is a standout.",
        "You've captured a different kind of beauty in this shot.",
        "Your creativity knows no bounds, even without the object.",
        "The absence of the object has its own unique charm.",
        "Your photography is versatile and always impressive.",
        "Sometimes the unexpected can lead to extraordinary results.",
        "Even without the object, your photo is a masterpiece.",
        "Your talent transcends the need for the object."
    };

    private string[] catDissapointments =
    {
        "Oops, looks like the cat's taking a nap elsewhere!",
        "The purr-fect shot is still up for grabs!",
        "No sign of whiskers in this photo!",
        "The cat must be on a secret mission.",
        "Looks like the cat is playing hide and seek!",
        "The cat's invisibility cloak must be on!",
        "Cat-astrophe! The feline vanished from the frame.",
        "The cat has gone incognito in this one.",
        "No cat-tastic cameo in this shot!"
    };

    IEnumerator InterpCamera(Vector3 position, Vector3 rotation, bool aimSwitch)
    {
        while (Vector3.Distance(transform.localPosition, position) > 0.01f)
        {
            switch (aimSwitch)
            {
                case true when !isAiming:
                case false when isAiming:
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
        photoPreviewAnimator.SetTrigger(Photo);
        photoFlashAnimator.SetTrigger(Photo);
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
        if (GameManager.Instance.currentObjectIndex >= 2)
        {
            captureObject = debugPhotoObject;
        }
        else
        {
            captureObject = GameManager.Instance.objectsToCapture[GameManager.Instance.currentObjectIndex];
        }
        photoPreviewText.text = captureObject.name;
        List<float> photoScore = photoScoringManager.ScorePhoto(captureObject);
        Debug.Log("Photo score: Visibility: " + photoScore[0] + " Object Visibility: " + photoScore[1] + " Centering: " + photoScore[3] + " Rule of Thirds: " + photoScore[4]);
        PlayerPrefs.SetString(uuid, string.Join(",", photoScore));
        if (photoScore[0] > 0)
        {
            photoScores.text = $"\nVisibility: {Mathf.CeilToInt(photoScore[1] * photoScoringManager.objectVisibilityWeight)}";
            if (photoScore[3] >= photoScore[4])
            {
                photoScores.text += $"\nCentering: {Mathf.CeilToInt(photoScore[3] * photoScoringManager.centeringWeight)}\n\n";
            }
            else
            {
                photoScores.text += $"\nRule of Thirds: {Mathf.CeilToInt(photoScore[4] * photoScoringManager.ruleOfThirdsWeight)}\n\n";
            }
            if (captureObject.CompareTag("Cat"))
            {
                photoScores.text += catCompliments[Random.Range(0, catCompliments.Length)];
            }
            else
            {
                photoScores.text += compliments[Random.Range(0, compliments.Length)];
            }
            GameManager.Instance.IncrementObjectIndex();
        }
        else
        {
            photoScores.text = "\nVisibility: 0\n\n";
            if (captureObject.CompareTag("Cat"))
            {
                photoScores.text += catDissapointments[Random.Range(0, catDissapointments.Length)];
            }
            else
            {
                photoScores.text += dissapointments[Random.Range(0, dissapointments.Length)];
            }
        }
    }
}
