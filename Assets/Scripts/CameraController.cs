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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator InterpCamera(Vector3 position, Vector3 rotation)
    {
        while (Vector3.Distance(transform.localPosition, position) > 0.01f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, position, interpSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotation), interpRotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void Aim()
    {
        StartCoroutine(InterpCamera(aimPosition, aimRotation));
    }

    public void Unaim()
    {
        StartCoroutine(InterpCamera(defaultPosition, defaultRotation));
    }
}
