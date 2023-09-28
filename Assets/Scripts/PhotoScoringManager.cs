using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoScoringManager : MonoBehaviour
{
    [Header("Weights")]
    public float visibilityWeight = 1.0f;
    public float objectVisibilityWeight = 400.0f;
    public float objectSizeWeight = 1.0f;
    public float centeringWeight = 350.0f;
    public float ruleOfThirdsWeight = 450.0f;

    [Header("References")]
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private CameraController cameraController;
    private Camera cameraComponent => cameraObject.GetComponent<Camera>();

    /// <summary>
    /// Calculate the score for a photo object based on the metrics
    /// </summary>
    /// <param name="photoObject">Object to calculate for</param>
    /// <returns>List of unweighted scores</returns>
    public List<float> ScorePhoto(GameObject photoObject)
    {
        float visibilityScore = CalculateVisibility(photoObject);
        float objectVisibilityScore = CalculateObjectVisibility(photoObject);
        float objectSizeScore = CalculateObjectSize(photoObject);
        float angleScore = CalculateAngle(photoObject);
        float ruleOfThirdsScore = CalculateRuleOfThirds(photoObject);

        return new List<float> { visibilityScore, objectVisibilityScore, objectSizeScore, angleScore, ruleOfThirdsScore };
    }

    /// <summary>
    /// Metric 1: if the object is visible at all within the camera's view frustum
    /// </summary>
    /// <param name="photoObject">Object to calculate for</param>
    /// <returns></returns>
    private float CalculateVisibility(GameObject photoObject)
    {
        Renderer meshRenderer = photoObject.GetComponent<Renderer>();
        if (meshRenderer == null)
        {
            meshRenderer = photoObject.GetComponentInChildren<Renderer>();
        }
        // Check if the object is visible from the specified camera
        bool isInFrustum = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cameraComponent), meshRenderer.bounds);
        Debug.Log("Is in frustum: " + isInFrustum);
        // Check if the object is occluded by other objects, ignore the camera layer (redundant, this only checks the object's pivot point)
        // bool isOccluded = Physics.Linecast(cameraComponent.transform.position, photoObject.transform.position, ~(1 << LayerMask.NameToLayer("Camera")));
        // Debug.Log("Is occluded: " + isOccluded);
        // Return 1 if the object is visible, or 0 if its not visible
        return isInFrustum ? 1.0f : 0.0f;
    }

    /// <summary>
    /// Metric 2: how much of the object is visible
    /// </summary>
    /// <param name="photoObject">Object to calculate for</param>
    /// <returns></returns>
    private float CalculateObjectVisibility(GameObject photoObject)
    {
        Renderer objectRenderer = photoObject.GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            objectRenderer = photoObject.GetComponentInChildren<Renderer>();
        }
        Bounds bounds = objectRenderer.bounds;
        int gridSize = 10;
        float stepX = bounds.size.x / gridSize;
        float stepY = bounds.size.y / gridSize;
        float stepZ = bounds.size.z / gridSize;
        int numHits = 0;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    Vector3 gridPoint = new Vector3(
                        bounds.min.x + x * stepX,
                        bounds.min.y + y * stepY,
                        bounds.min.z + z * stepZ
                    );
                    RaycastHit hit;
                    if (Physics.Linecast(cameraObject.transform.position, gridPoint, out hit, ~(1 << LayerMask.NameToLayer("Camera"))))
                    {
                        // Check if the raycast hit the object
                        if (hit.collider.gameObject == photoObject)
                        {
                            numHits++;
                            Debug.DrawLine(cameraObject.transform.position, gridPoint, Color.red, 10.0f);
                        }
                        else
                        {
                            Debug.DrawLine(cameraObject.transform.position, gridPoint, Color.green, 10.0f);
                        }
                    }
                }
            }
        }

        return (float)numHits / (float)(gridSize * gridSize * gridSize);
    }

    /// <summary>
    /// Metric 3: supposed to calculate how large the object is on the screen
    /// </summary>
    /// <param name="photoObject">Object to calculate for</param>
    /// <returns></returns>
    private float CalculateObjectSize(GameObject photoObject)
    {
        // get the top left corner screen coordinates of the bounding box
        //Vector3 screenPosition = cameraComponent.WorldToScreenPoint(photoObject.GetComponent<Renderer>().bounds.min);
        // get the bottom right corner screen coordinates of the bounding box
        //Vector3 screenPosition2 = cameraComponent.WorldToScreenPoint(photoObject.GetComponent<Renderer>().bounds.max);

        //float screenSize = Mathf.Abs(Mathf.Max((screenPosition2.x - screenPosition.x), (screenPosition2.z - screenPosition.z)) * (screenPosition2.y - screenPosition.y) / (Screen.width * Screen.height)) * 2;

        return 1f;
    }

    /// <summary>
    /// Metric 4: determine how centred the object is in the camera's view
    /// </summary>
    /// <param name="photoObject">Object to calculate for</param>
    /// <returns></returns>
    private float CalculateAngle(GameObject photoObject)
    {
        Vector3 toObject = photoObject.transform.position - cameraComponent.transform.position;
        float angle = Vector3.Angle(cameraComponent.transform.forward, toObject);
        float normalizedAngle = Mathf.Clamp01(angle / 90.0f);
        float angleScore = 1.0f - normalizedAngle;
        angleScore = Mathf.Pow(angleScore, 4);
        return angleScore;
    }

    /// <summary>
    /// Metric 5: calculate how well the photo sticks to the rule of thirds
    /// </summary>
    /// <param name="photoObject">Object to calculate for</param>
    /// <returns></returns>
    private float CalculateRuleOfThirds(GameObject photoObject)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        Renderer objectRenderer = photoObject.GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            objectRenderer = photoObject.GetComponentInChildren<Renderer>();
        }
        Vector3 screenPosition = cameraComponent.WorldToScreenPoint(objectRenderer.bounds.center);

        // calculate the 4 points of the rule of thirds grid
        Vector3 ruleOfThirdsTopLeft = new Vector3(screenWidth / 3, screenHeight / 3, 0);
        Vector3 ruleOfThirdsTopRight = new Vector3(screenWidth * 2 / 3, screenHeight / 3, 0);
        Vector3 ruleOfThirdsBottomLeft = new Vector3(screenWidth / 3, screenHeight * 2 / 3, 0);
        Vector3 ruleOfThirdsBottomRight = new Vector3(screenWidth * 2 / 3, screenHeight * 2 / 3, 0);

        // calculate the distance from the object to each of the 4 points
        float distanceToTopLeft = Vector3.Distance(screenPosition, ruleOfThirdsTopLeft);
        float distanceToTopRight = Vector3.Distance(screenPosition, ruleOfThirdsTopRight);
        float distanceToBottomLeft = Vector3.Distance(screenPosition, ruleOfThirdsBottomLeft);
        float distanceToBottomRight = Vector3.Distance(screenPosition, ruleOfThirdsBottomRight);

        // calculate the minimum distance to any of the 4 points
        float minDistance = Mathf.Min(distanceToTopLeft, distanceToTopRight, distanceToBottomLeft, distanceToBottomRight);

        // calculate the score based on the minimum distance
        float ruleOfThirdsScore = 1.0f - minDistance / (screenWidth / 3);

        return ruleOfThirdsScore;
    }
}

