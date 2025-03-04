using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PostProcessing : MonoBehaviour
{
    public Material highlightMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (highlightMaterial != null)
        {
            highlightMaterial.SetTexture("_MainTex", src);
            Graphics.Blit(src, dest, highlightMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}