using UnityEngine;

public class EstatuaController : MonoBehaviour
{
    [Header("Configuración de Absorción")]
    [Tooltip("El nombre exacto del ítem que se añadirá al inventario.")]
    public string itemParaInventario = "El corazon indescifrable";

    [Tooltip("Tag del objeto que esta estatua puede absorber. Asegúrate que el objeto absorbible tenga este tag.")]
    public string tagObjetoAbsorbible = "ObjetoParaEstatua";

    private Inventario inventarioJugador;

    void Start()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            inventarioJugador = jugador.GetComponent<Inventario>();
        }

        if (inventarioJugador == null)
        {
            UnityEngine.Debug.LogError("EstatuaController (" + gameObject.name + "): No se pudo encontrar el script Inventario en el jugador.");
        }

        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            UnityEngine.Debug.LogError("EstatuaController (" + gameObject.name + "): No tiene un Collider.");
        }
        else if (!col.isTrigger)
        {
            UnityEngine.Debug.LogWarning("EstatuaController (" + gameObject.name + "): El Collider no está configurado como 'Is Trigger'.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("Estatua (" + gameObject.name + ") detectó: " + other.gameObject.name);

        if (other.CompareTag(tagObjetoAbsorbible) && other.GetComponent<ObjetoAbsorbible>() != null)
        {
            UnityEngine.Debug.Log("Estatua (" + gameObject.name + "): Objeto '" + other.gameObject.name + "' es absorbible.");

            if (inventarioJugador != null)
            {
                inventarioJugador.AddItem(itemParaInventario);
                UnityEngine.Debug.Log("Estatua (" + gameObject.name + "): '" + itemParaInventario + "' añadido al inventario.");

                Destroy(other.gameObject);
                UnityEngine.Debug.Log("Estatua (" + gameObject.name + "): Objeto '" + other.gameObject.name + "' destruido.");

                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlayGritoEstatua();
                    UnityEngine.Debug.Log("Estatua (" + gameObject.name + "): Sonido de grito de estatua solicitado.");
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Estatua (" + gameObject.name + "): AudioManager no encontrado.");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("EstatuaController (" + gameObject.name + "): Referencia al inventario del jugador es nula.");
            }
        }
        else
        {
            UnityEngine.Debug.Log("Estatua (" + gameObject.name + "): Objeto '" + other.gameObject.name + "' NO es el objeto absorbible (Tag: '" + other.tag + "').");
        }
    }
}