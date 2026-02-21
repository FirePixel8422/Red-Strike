using Fire_Pixel.Utility;
using UnityEngine;


public class ScreenBlurUpdater : MonoBehaviour
{
    [SerializeField] private Material blurMaterial;

    private RenderTexture renderTex;
    private Camera mainCam;



    private void Awake()
    {
        mainCam = Camera.main;
    }
    private void OnEnable()
    {
        UpdateScheduler.RegisterLateUpdate(OnLateUpdate);
    }
    private void OnDisable()
    {
        if (renderTex != null)
        {
            renderTex.Release();
            renderTex = null;
        }
        UpdateScheduler.UnRegisterLateUpdate(OnLateUpdate);
    }

    private void OnLateUpdate()
    {
        // Create render texture if needed
        if (renderTex == null || renderTex.width != Screen.width || renderTex.height != Screen.height)
        {
            if (renderTex != null)
                renderTex.Release();

            renderTex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.DefaultHDR);
            renderTex.Create();
        }

        // Render the camera into the render texture
        mainCam.targetTexture = renderTex;
        mainCam.Render();
        mainCam.targetTexture = null;

        // Assign to the blur material
        blurMaterial.SetTexture("_MainTex", renderTex);
    }
}