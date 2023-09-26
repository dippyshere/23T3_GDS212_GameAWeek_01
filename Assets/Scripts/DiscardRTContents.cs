using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DiscardRTContents : MonoBehaviour
{

    private void Update()
    {
        if (GetComponent<Camera>().enabled == true)
        {
            GetComponent<Camera>().enabled = false;
        }
        else
        {
            GetComponent<Camera>().enabled = true;
        }
    }

    //void OnEnable()
    //{
    //    RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    //}

    //void OnDisable()
    //{
    //    RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
    //}

    //private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
    //{
    //    OnPostRender();
    //}

    //private void OnPostRender()
    //{
    //    //RenderTexture.active.DiscardContents();
    //    //RenderTexture.active.Release();
    //    //RenderTexture.active = null;
    //    GetComponent<Camera>().enabled = false;
    //    GetComponent<Camera>().enabled = true;
    //    //this.GetComponent<Camera>().enabled = true;
    //}
}
