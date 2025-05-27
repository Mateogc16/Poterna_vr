using UnityEngine;

public class PinchosTrigger : MonoBehaviour
{
    public GameObject pinchos; // Asigna el objeto "pinchos" desde el inspector

    private void OnTriggerEnter(Collider other)
    {
        // Asegúrate de que sea el jugador quien activa el trigger
        if (other.CompareTag("Player"))
        {
            // Activar físicas en el objeto pinchos
            Rigidbody rb = pinchos.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = false; // Permitir que la física afecte al objeto
                rb.useGravity = true;   // Asegurar que la gravedad esté activada
            }
            else
            {
                Debug.LogWarning("El objeto 'pinchos' no tiene un Rigidbody asignado.");
            }
        }
    }
}

