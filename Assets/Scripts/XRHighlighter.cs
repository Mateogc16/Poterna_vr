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
        // 🔍 Busca el Renderer en el objeto o sus hijos
        var targetRenderer = args.interactableObject.transform.GetComponentInChildren<Renderer>();
        if (targetRenderer != null)
        {
            currentRenderer = targetRenderer;
            originalMaterials = targetRenderer.materials;

            // Cambia todos los materiales por el resaltado
            Material[] highlightMaterials = new Material[originalMaterials.Length];
            for (int i = 0; i < highlightMaterials.Length; i++)
                highlightMaterials[i] = highlightMaterial;

            targetRenderer.materials = highlightMaterials;
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
