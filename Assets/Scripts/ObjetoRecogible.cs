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
                UnityEngine.Debug.Log("Objeto recogido: " + nombreObjeto + ". Intentando reproducir sonido..."); // Log para confirmar que llegamos aquí

                // --- AÑADE ESTAS LÍNEAS PARA EL SONIDO ---
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlayRecogerObjeto();
                }
                else
                {
                    UnityEngine.Debug.LogError("AudioManager.instance es NULO al intentar recoger " + nombreObjeto + ". No se puede reproducir sonido.");
                }
                // --- FIN DE LÍNEAS PARA EL SONIDO ---

                Destroy(gameObject);
            }
            else
            {
                // Es buena práctica saber si el inventario no se encontró
                UnityEngine.Debug.LogWarning("Componente Inventario no encontrado en el objeto con tag 'Player': " + other.name);
            }
        }
    }
}