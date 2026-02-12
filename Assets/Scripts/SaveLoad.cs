using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour
{
    public Canvas canvas;
    public GameObject photoPrefab;
    public GameObject photoContainer;
    public GameObject loadingScreen;
    private int totalPhotosToLoad;
    public ScrollRect scrollRect;

    public void LoadImages()
    {
        canvas.enabled = true;
        string photosString = PlayerPrefs.GetString("photos", "");
        string[] photoGuids = photosString.Split(',');
        totalPhotosToLoad = photoGuids.Length;
        foreach (Transform child in photoContainer.transform)
        {
            Destroy(child.gameObject);
        }
        StartCoroutine(WaitForLoading());
        foreach (string photoGuid in photoGuids)
        {
            if (string.IsNullOrEmpty(photoGuid))
            {
                totalPhotosToLoad -= 1;
                continue;
            }

            AsyncInstantiateOperation<GameObject> operation = InstantiateAsync(photoPrefab, photoContainer.transform);
            operation.completed += (result) =>
            {
                operation.Result[0].GetComponent<ImagePropertySetter>().SetImageProperties(photoGuid);
                totalPhotosToLoad -= 1;
            };
        }
    }

    public void HideImages()
    {
        foreach (Transform child in photoContainer.transform)
        {
            Destroy(child.gameObject);
        }
        canvas.enabled = false;
    }

    private IEnumerator WaitForLoading()
    {
        loadingScreen.SetActive(true);
        while (totalPhotosToLoad > 0)
        {
            scrollRect.verticalNormalizedPosition = 1f;
            yield return null;
        }
        loadingScreen.SetActive(false);
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
