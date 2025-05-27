using UnityEngine;
using UnityEngine.UI; // Necesario para interactuar con el Botón UI

public class PlayerRespawn : MonoBehaviour
{
    public static Vector3 currentCheckpointPosition;
    private Rigidbody rb;
    private Vector3 startPosition;

    [Header("Muerte y UI")]
    public GameObject canvasMuerte; // Arrastra tu CanvasMuerte aquí desde el Inspector
    public Button botonRespawn;    // Arrastra tu BotonRespawn aquí
    public string tagTrampa = "Trampa"; // Tag para los objetos que matan al jugador

    private bool estaMuerto = false;

    // Opcional: Referencia a tu script de movimiento para desactivarlo al morir
    // public PlayerMovement playerMovementScript;

    void Awake()
    {
        startPosition = transform.position;
        currentCheckpointPosition = startPosition;
        rb = GetComponent<Rigidbody>();

        if (!CompareTag("Player"))
        {
            UnityEngine.Debug.LogWarning("El objeto Jugador no tiene el tag 'Player'. Los triggers de checkpoint podrían no funcionar.");
        }

        // Asegurarse de que el Canvas de muerte esté oculto al inicio
        if (canvasMuerte != null)
        {
            canvasMuerte.SetActive(false);
        }
        else
        {
            UnityEngine.Debug.LogError("CanvasMuerte no asignado en el script PlayerRespawn.");
        }

        // Configurar el listener del botón de respawn
        if (botonRespawn != null)
        {
            botonRespawn.onClick.AddListener(RespawnDesdeBoton); // El botón llamará a RespawnDesdeBoton
        }
        else
        {
            UnityEngine.Debug.LogError("BotonRespawn no asignado en el script PlayerRespawn.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Si el jugador colisiona con un objeto con el tag "Trampa" y no está ya muerto
        if (!estaMuerto && collision.gameObject.CompareTag(tagTrampa))
        {
            Morir();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // También puedes usar Triggers para las trampas si lo prefieres
        if (!estaMuerto && other.gameObject.CompareTag(tagTrampa))
        {
            Morir();
        }
    }

    void Morir()
    {
        if (estaMuerto) return; // Evitar múltiples llamadas

        estaMuerto = true;
        UnityEngine.Debug.Log("El jugador ha muerto.");

        // Mostrar el Canvas de muerte
        if (canvasMuerte != null)
        {
            canvasMuerte.SetActive(true);
        }

        // Opcional: Desactivar el control del jugador
        // if (playerMovementScript != null) playerMovementScript.enabled = false;
        // Time.timeScale = 0f; // Pausar el juego (cuidado con animaciones UI si no usan UnscaledTime)
    }

    // Este método será llamado por el botón del Canvas
    public void RespawnDesdeBoton()
    {
        if (!estaMuerto) return; // Solo respawnear si estaba muerto

        estaMuerto = false;
        transform.position = currentCheckpointPosition;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Ocultar el Canvas de muerte
        if (canvasMuerte != null)
        {
            canvasMuerte.SetActive(false);
        }

        // Opcional: Reactivar el control del jugador
        // if (playerMovementScript != null) playerMovementScript.enabled = true;
        // Time.timeScale = 1f; // Reanudar el juego si se pausó

        UnityEngine.Debug.Log("Jugador ha respawneado en: " + currentCheckpointPosition + " (desde botón)");
    }

    // Método para respawnear por otras causas (ej. tecla, caída)
    public void RespawnPorCaidaOTecla()
    {
        // Si está "muerto" (con el canvas activo), la tecla R no debería hacer nada,
        // el jugador debe usar el botón.
        if (estaMuerto && canvasMuerte != null && canvasMuerte.activeSelf) return;

        // Si el canvas está activo pero `estaMuerto` es falso (estado inconsistente), no hacer nada.
        // O si simplemente se presiona R sin estar formalmente muerto (sin canvas)
        if (!estaMuerto)
        {
            transform.position = currentCheckpointPosition;
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            UnityEngine.Debug.Log("Jugador ha respawneado en: " + currentCheckpointPosition + " (por tecla/caída sin canvas)");
        }
        else // Si `estaMuerto` es true pero el canvas no está activo (ej. cayó y murió)
        {
            // En este caso, si cayó y murió, queremos que aparezca el canvas.
            Morir();
        }
    }


    void Update()
    {
        // Para propósitos de prueba: Presiona la tecla 'R' para respawnear
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Si el jugador no está "muerto" con el canvas activo, la tecla R funciona como antes.
            // Si está "muerto" y el canvas está activo, la tecla R no hace nada (debe usar el botón).
            if (estaMuerto && canvasMuerte != null && canvasMuerte.activeSelf)
            {
                // No hacer nada si el canvas de muerte está activo, el botón es el camino.
            }
            else if (estaMuerto) // Muerto pero sin canvas (ej. por caída que aún no mostró canvas)
            {
                Morir(); // Asegura que el canvas aparezca
            }
            else // No está muerto
            {
                transform.position = currentCheckpointPosition; // Respawn directo
                if (rb != null) { rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
                UnityEngine.Debug.Log("Jugador ha respawneado en: " + currentCheckpointPosition + " (por tecla)");
            }
        }

        // Ejemplo: Si el jugador cae por debajo de cierta altura, muere.
        if (transform.position.y < -10f && !estaMuerto) // Ajusta este valor
        {
            UnityEngine.Debug.Log("Jugador ha caído y muerto.");
            Morir(); // Llama al proceso de muerte (mostrará el canvas)
        }
    }
}