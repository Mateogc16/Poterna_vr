using UnityEngine;

public class ObjetoRecogible : MonoBehaviour
{
    public string nombreObjeto; // Nombre del objeto (definir en el Inspector)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegurarse de que el jugador está interactuando
        {
            Inventario inventario = other.GetComponent<Inventario>();

            if (inventario != null)
            {
                inventario.AddItem(nombreObjeto);
                UnityEngine.Debug.Log("Objeto recogido: " + nombreObjeto);
                Destroy(gameObject); // Eliminar el objeto de la escena
            }
        }
    }
}