using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHighlighter : MonoBehaviour
{
    public Material highlightMaterial;
    private XRRayInteractor rayInteractor;

    private Renderer currentRenderer;
    private Material[] originalMaterials;

    void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    void OnEnable()
    {
        rayInteractor.hoverEntered.AddListener(OnHoverEntered);
        rayInteractor.hoverExited.AddListener(OnHoverExited);
    }

    void OnDisable()
    {
        rayInteractor.hoverEntered.RemoveListener(OnHoverEntered);
        rayInteractor.hoverExited.RemoveListener(OnHoverExited);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        var target = args.interactableObject.transform.GetComponent<Renderer>();
        if (target != null)
        {
            currentRenderer = target;
            originalMaterials = target.materials;

            // Crear nuevo arreglo con el mismo número de materiales pero todos de highlight
            Material[] highlightMaterials = new Material[originalMaterials.Length];
            for (int i = 0; i < highlightMaterials.Length; i++)
                highlightMaterials[i] = highlightMaterial;

            target.materials = highlightMaterials;
        }
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (currentRenderer != null)
        {
            currentRenderer.materials = originalMaterials;
            currentRenderer = null;
            originalMaterials = null;
        }
    }
}
