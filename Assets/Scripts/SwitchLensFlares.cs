using UnityEngine;
using UnityEngine.Rendering;

public class SwitchLensFlares : MonoBehaviour
{
    [SerializeField] Camera targetCamera;
    public LensFlareComponentSRP reqLensFlare;

    void OnCameraEnd(ScriptableRenderContext _, Camera currentCamera)
    {
        if (targetCamera == null || currentCamera == targetCamera) reqLensFlare.enabled = false;
    }

    void OnCameraStart(ScriptableRenderContext _, Camera currentCamera)
    {
        if (targetCamera == null || currentCamera == targetCamera) reqLensFlare.enabled = true;
    }

    void Awake()
    {
        RenderPipelineManager.beginCameraRendering += OnCameraStart;
        RenderPipelineManager.endCameraRendering += OnCameraEnd;
    }
    void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= OnCameraStart;
        RenderPipelineManager.endCameraRendering -= OnCameraEnd;
    }
}
