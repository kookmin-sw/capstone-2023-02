using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTransparent : MonoBehaviour
{
    [Range(0f, 1f)]
    public float transparency = 0.2f;
    private float prevTransparency = 1.0f;
    private Renderer[] renderers;

    // Start is called before the first frame update
    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        //Initialize
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                // Set the rendering mode to "Transparent"
                material.SetFloat("_Mode", 3);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                // Set the alpha value of the material's color
                Color color = material.color;
                color.a = transparency; // Set the alpha value to 0.5f for 50% transparency
                material.color = color;
            }
        }
    }

    void Update()
    {
        if (transparency != prevTransparency) adjustTransparent();
    }

    void adjustTransparent()
    {
        foreach (Renderer renderer in renderers)
        {

            foreach (Material material in renderer.materials)
            {
                // Set the alpha value of the material's color
                Color color = material.color;
                color.a = transparency; // Set the alpha value to 0.5f for 50% transparency
                material.color = color;
            }
        }
    }

}
