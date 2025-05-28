using UnityEngine;
using TMPro;

public class ControladorPuerta : MonoBehaviour
{
    public Transform jugador;
    public float distanciaApertura = 3f;
    public float velocidadApertura = 2f;
    public float anguloApertura = 90f;

    // REMOVEMOS ESTAS LÍNEAS YA QUE USAREMOS EL AUDIOMANAGER
    // public AudioSource audioSource;       // Ya no es necesario aquí
    // public AudioClip sonidoApertura;    // Se configurará en el AudioManager
    // public AudioClip sonidoCierre;      // Se configurará en el AudioManager

    public bool requiereItem = false;
    public string requiredItem;
    private Inventario inventario;

    public GameObject mensajePanel;
    public TextMeshProUGUI mensajeTexto;

    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;
    private bool puertaAbierta = false;
    private bool reproduciendoSonidoCierre = false; // Para evitar múltiples reproducciones del sonido de cierre

    void Start()
    {
        rotacionInicial = transform.rotation;
        rotacionFinal = rotacionInicial * Quaternion.Euler(0, anguloApertura, 0);

        inventario = jugador != null ? jugador.GetComponent<Inventario>() : null;
        if (requiereItem && inventario == null && jugador != null) // Solo error si el jugador está asignado pero no tiene inventario
        {
            UnityEngine.Debug.LogError("El jugador '" + jugador.name + "' no tiene un componente Inventario y la puerta requiere un ítem.");
        }
        else if (jugador == null)
        {
            UnityEngine.Debug.LogWarning("Jugador no asignado al ControladorPuerta: " + gameObject.name + ". La funcionalidad de apertura por proximidad y por ítem no funcionará.");
        }


        if (mensajePanel != null)
        {
            mensajePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);
        bool debeAbrir = false;

        if (distancia < distanciaApertura)
        {
            if (!requiereItem || (inventario != null && inventario.HasItem(requiredItem)))
            {
                debeAbrir = true;
                if (mensajePanel != null && mensajePanel.activeSelf) // Ocultar mensaje si se abre
                {
                    mensajePanel.SetActive(false);
                }
            }
            else // Está cerca pero le falta el ítem
            {
                if (!puertaAbierta && mensajePanel != null && mensajeTexto != null) // Solo mostrar si la puerta no se ha abierto aún por esta condición
                {
                    mensajeTexto.text = "Necesitas " + requiredItem + " para continuar.";
                    mensajePanel.SetActive(true);
                }
                // UnityEngine.Debug.Log("Necesitas " + requiredItem + " para abrir esta puerta."); // El mensaje en panel es mejor
            }
        }
        else // El jugador está lejos
        {
            debeAbrir = false; // La puerta debería tender a cerrarse si el jugador se aleja
            if (mensajePanel != null && mensajePanel.activeSelf) // Ocultar mensaje si el jugador se aleja
            {
                mensajePanel.SetActive(false);
            }
        }

        // Lógica de Apertura y Cierre con Sonidos
        if (debeAbrir && !puertaAbierta)
        {
            puertaAbierta = true;
            // LLAMAMOS AL AUDIOMANAGER
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayAbrirPuerta();
            }
            else
            {
                UnityEngine.Debug.LogWarning("AudioManager.instance no encontrado!");
            }
            UnityEngine.Debug.Log("¡Puerta abierta!");
            reproduciendoSonidoCierre = false; // Reseteamos al abrir
        }
        else if (!debeAbrir && puertaAbierta)
        {
            // Considerar si quieres que la puerta se cierre automáticamente al alejarse
            // Si es así, aquí cambiarías puertaAbierta a false y reproducirías el sonido de cierre.
            // Por ahora, tu lógica solo la abre y la mantiene abierta por Lerp.
            // Si implementas cierre automático:
            // puertaAbierta = false;
            // if (AudioManager.instance != null && !reproduciendoSonidoCierre)
            // {
            //     AudioManager.instance.PlayCerrarPuerta();
            //     reproduciendoSonidoCierre = true; // Para evitar que suene en cada frame mientras se cierra
            // }
            // UnityEngine.Debug.Log("¡Puerta cerrándose por alejamiento!");
        }


        // La animación de la puerta
        Quaternion targetRotation = puertaAbierta ? rotacionFinal : rotacionInicial;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * velocidadApertura);

        // Si la puerta se ha cerrado completamente (o casi) después de estar abierta, resetear flag de sonido de cierre
        if (!puertaAbierta && reproduciendoSonidoCierre)
        {
            if (Quaternion.Angle(transform.rotation, rotacionInicial) < 1.0f)
            { // Un pequeño umbral
                reproduciendoSonidoCierre = false;
            }
        }
    }

    public void ForzarApertura()
    {
        if (!puertaAbierta)
        {
            puertaAbierta = true;
            // LLAMAMOS AL AUDIOMANAGER
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayAbrirPuerta();
            }
            else
            {
                UnityEngine.Debug.LogWarning("AudioManager.instance no encontrado!");
            }

            if (mensajePanel != null)
                mensajePanel.SetActive(false);

            UnityEngine.Debug.Log("¡Puerta abierta forzada!");
            reproduciendoSonidoCierre = false;
        }
    }

    public void ForzarCierre()
    {
        if (puertaAbierta)
        {
            puertaAbierta = false;
            // LLAMAMOS AL AUDIOMANAGER
            if (AudioManager.instance != null && !reproduciendoSonidoCierre)
            {
                AudioManager.instance.PlayCerrarPuerta();
                reproduciendoSonidoCierre = true; // Para que no suene múltiples veces si se llama rápido
            }
            else if (AudioManager.instance == null)
            {
                UnityEngine.Debug.LogWarning("AudioManager.instance no encontrado!");
            }
            UnityEngine.Debug.Log("¡Puerta cerrada forzada!");
        }
    }
}