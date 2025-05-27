using UnityEngine;

public class ObjetoRecogible : MonoBehaviour
{
    public string nombreObjeto; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            Inventario inventario = other.GetComponent<Inventario>();

            if (inventario != null)
            {
                inventario.AddItem(nombreObjeto);
                UnityEngine.Debug.Log("Objeto recogido: " + nombreObjeto);
                Destroy(gameObject); 
            }
        }
    }
}