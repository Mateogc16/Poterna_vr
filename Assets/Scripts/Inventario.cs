using System.Collections.Generic;
using UnityEngine;

public class Inventario : MonoBehaviour
{
    private List<string> items = new List<string>();

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        UnityEngine.Debug.Log(itemName + " ha sido añadido al inventario.");
        // Aquí podrías añadir lógica para actualizar una UI de inventario si la tienes.
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    // El OnTriggerEnter original que tenías para recoger objetos directamente.
    // Puedes mantenerlo si también tienes objetos que se recogen de esa forma.
    // Si los objetos solo se recogen mediante interacción (como las notas o este corazón de estatua),
    // esta parte podría no ser necesaria o necesitar ajustes.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Recogible")) // Asegúrate de que los objetos directamente recogibles tengan este tag
        {
            ObjetoRecogible objeto = other.GetComponent<ObjetoRecogible>();

            if (objeto != null)
            {
                AddItem(objeto.nombreObjeto); // Asume que ObjetoRecogible tiene una variable pública 'nombreObjeto'
                UnityEngine.Debug.Log("Objeto recogido directamente: " + objeto.nombreObjeto);
                Destroy(other.gameObject);
            }
        }
    }

    // Método de ejemplo para mostrar el inventario en la consola (para depuración)
    public void MostrarInventario()
    {
        if (items.Count == 0)
        {
            UnityEngine.Debug.Log("El inventario está vacío.");
            return;
        }
        UnityEngine.Debug.Log("Contenido del inventario:");
        foreach (string item in items)
        {
            UnityEngine.Debug.Log("- " + item);
        }
    }
}