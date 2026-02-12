using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImagePropertySetter : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI timestampText;
    public TextMeshProUGUI scoreText;
    private string _imageGuid;

    public void SetImageProperties(string imageGuid)
    {
        // load the photo from Application.persistentDataPath + "/Photos" + imageGuid + ".png"
        string imagePath = System.IO.Path.Combine(Application.persistentDataPath, "Photos", imageGuid + ".png");
        if (!System.IO.File.Exists(imagePath))
        {
            Destroy(gameObject);
            return;
        }

        byte[] imageData = System.IO.File.ReadAllBytes(imagePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        // set the timestamp text to the file creation time of the photo if it exists, formatted as an ISO 8601 string (e.g. "2024-06-01T12:34:56.789Z")
        // otherwise set it to 2023
        if (System.IO.File.Exists(imagePath))
        {
            System.DateTime creationTime = System.IO.File.GetCreationTimeUtc(imagePath);
            timestampText.text = creationTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
        else
        {
            timestampText.text = "Circa 2023";
        }
        // set the score text to the combined score of the photo from the playerprefs key imageGuid
        // the value is stored as comma separated values of score weights
        string scoreString = PlayerPrefs.GetString(imageGuid, "0,0,0,0,0");
        string[] scoreValues = scoreString.Split(',');
        float visibilityScore = float.Parse(scoreValues[0]);
        float objectVisibilityScore = float.Parse(scoreValues[1]);
        float centeringScore = float.Parse(scoreValues[3]);
        float ruleOfThirdsScore = float.Parse(scoreValues[4]);
        float combinedScore = visibilityScore * GameManager.Instance.photoScoringManager.visibilityWeight +
                              objectVisibilityScore * GameManager.Instance.photoScoringManager.objectVisibilityWeight +
                              centeringScore * GameManager.Instance.photoScoringManager.centeringWeight +
                              ruleOfThirdsScore * GameManager.Instance.photoScoringManager.ruleOfThirdsWeight;
        scoreText.text = $"Score: {Mathf.CeilToInt(combinedScore)}";
        _imageGuid = imageGuid;
    }

    // called on cursor enter event
    public void OnCursorEnter()
    {
        image.color = Color.lightGray;
    }

    // called on cursor exit event
    public void OnCursorExit()
    {
        image.color = Color.white;
    }

    public void OnClicked()
    {
        // open the photo in the default image viewer application
        string imagePath = System.IO.Path.Combine(Application.persistentDataPath, "Photos", _imageGuid + ".png");
        if (System.IO.File.Exists(imagePath))
        {
            Application.OpenURL(imagePath);
        }
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            byte[] imageData = System.IO.File.ReadAllBytes(imagePath);
            string base64 = System.Convert.ToBase64String(imageData);
            string url = "data:image/png;base64," + base64;
            Application.OpenURL(url);
        }
    }
}
