using UnityEngine;

public class PinchosTrigger : MonoBehaviour
{
    public GameObject pinchos; 

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Player"))
        {
            
            Rigidbody rb = pinchos.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = false; 
                rb.useGravity = true;   
            }
            else
            {
                Debug.LogWarning("El objeto 'pinchos' no tiene un Rigidbody asignado.");
            }
        }
    }
}

