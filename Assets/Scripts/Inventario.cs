using System.Collections.Generic;
using UnityEngine;

public class Inventario : MonoBehaviour
{
    private List<string> items = new List<string>();

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        UnityEngine.Debug.Log(itemName + " ha sido añadido al inventario.");
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Recogible")) // Verifica si el objeto es recogible
        {
            ObjetoRecogible objeto = other.GetComponent<ObjetoRecogible>();

            if (objeto != null)
            {
                AddItem(objeto.nombreObjeto);
                UnityEngine.Debug.Log("Objeto recogido: " + objeto.nombreObjeto);
                Destroy(other.gameObject); // Eliminar el objeto de la escena
            }
        }
    }
}