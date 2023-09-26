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

    private bool isAiming = false;


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
}
