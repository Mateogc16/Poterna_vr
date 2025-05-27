using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjetoInteractuable : MonoBehaviour
{
    public Material materialResaltado; 
    private Material materialOriginal; 
    private Renderer objetoRenderer;

    void Start()
    {
        objetoRenderer = GetComponent<Renderer>();
        if (objetoRenderer != null)
        {
            materialOriginal = objetoRenderer.material; 
        }
    }

    public void OnHoverEnter()
    {
        if (objetoRenderer != null && materialResaltado != null)
        {
            objetoRenderer.material = materialResaltado; 
        }
    }

    public void OnHoverExit()
    {
        if (objetoRenderer != null)
        {
            objetoRenderer.material = materialOriginal; 
        }
    }
}