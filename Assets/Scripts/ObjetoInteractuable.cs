using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjetoInteractuable : MonoBehaviour
{
    public Material materialResaltado; // Material cuando el objeto es seleccionado
    private Material materialOriginal; // Material original del objeto
    private Renderer objetoRenderer;

    void Start()
    {
        objetoRenderer = GetComponent<Renderer>();
        if (objetoRenderer != null)
        {
            materialOriginal = objetoRenderer.material; // Guarda el material original
        }
    }

    public void OnHoverEnter()
    {
        if (objetoRenderer != null && materialResaltado != null)
        {
            objetoRenderer.material = materialResaltado; // Cambia al material resaltado
        }
    }

    public void OnHoverExit()
    {
        if (objetoRenderer != null)
        {
            objetoRenderer.material = materialOriginal; // Vuelve al material original
        }
    }
}