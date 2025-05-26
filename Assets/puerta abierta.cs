using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeyPickupDoorTrigger : MonoBehaviour
{
    [Header("Asignar el XR Grab Interactable del objeto llave")]
    public XRGrabInteractable keyObject;

    [Header("Puerta que se eliminará u ocultará")]
    public GameObject doorObject;

    private bool doorOpened = false;

    void OnEnable()
    {
        if (keyObject != null)
            keyObject.selectEntered.AddListener(OnKeyPickedUp);
    }

    void OnDisable()
    {
        if (keyObject != null)
            keyObject.selectEntered.RemoveListener(OnKeyPickedUp);
    }

    private void OnKeyPickedUp(SelectEnterEventArgs args)
    {
        if (doorOpened) return;

        // ✅ Desactiva la puerta
        if (doorObject != null)
        {
            doorObject.SetActive(false);  // Esto la oculta completamente
        }

        doorOpened = true;
    }
}
