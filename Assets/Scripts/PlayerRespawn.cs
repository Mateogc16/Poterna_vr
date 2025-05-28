using UnityEngine;
using UnityEngine.UI;

public class PlayerRespawn : MonoBehaviour
{
    // Checkpoint y Posición Inicial
    public static Vector3 currentCheckpointPosition;
    private Vector3 startPosition;
    private Rigidbody rb;

    [Header("Muerte y UI")]
    public GameObject canvasMuerte; // El panel UI que se muestra al morir
    public Button botonRespawn;     // El botón en el canvasMuerte para reaparecer
    public string tagTrampa = "Trampa"; // Tag para los objetos que matan al jugador

    private bool estaMuerto = false;

    // Tiempo de juego
    [Header("Configuración de Tiempo")]
    public float tiempoAlMorir = 0f; // Time.timeScale cuando el jugador muere (0 para pausar)
    public float tiempoNormal = 1f;  // Time.timeScale normal del juego

    void Awake()
    {
        // Guardar la posición inicial como el primer checkpoint
        startPosition = transform.position;
        currentCheckpointPosition = startPosition;

        rb = GetComponent<Rigidbody>();

        // Advertencia si el jugador no tiene el tag correcto
        if (!CompareTag("Player"))
        {
            UnityEngine.Debug.LogWarning("El objeto Jugador no tiene el tag 'Player'. Los triggers de checkpoint podrían no funcionar correctamente.");
        }

        // Configurar el canvas de muerte
        if (canvasMuerte != null)
        {
            canvasMuerte.SetActive(false); // Ocultar el canvas al inicio
        }
        else
        {
            UnityEngine.Debug.LogError("CanvasMuerte no ha sido asignado en el script PlayerRespawn en el objeto " + gameObject.name);
        }

        // Configurar el botón de respawn
        if (botonRespawn != null)
        {
            botonRespawn.onClick.AddListener(RespawnDesdeBoton); // Añadir listener al botón
        }
        else
        {
            UnityEngine.Debug.LogError("BotonRespawn no ha sido asignado en el script PlayerRespawn en el objeto " + gameObject.name);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Comprobar si colisiona con una trampa
        if (!estaMuerto && collision.gameObject.CompareTag(tagTrampa))
        {
            Morir();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Comprobar si entra en el trigger de una trampa
        if (!estaMuerto && other.gameObject.CompareTag(tagTrampa))
        {
            Morir();
        }
    }

    void Morir()
    {
        if (estaMuerto) return; // Si ya está muerto, no hacer nada

        estaMuerto = true;
        UnityEngine.Debug.Log("El jugador ha muerto.");

        // Reproducir sonido de muerte
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySonidoMuerte(); // Llama al método en tu AudioManager
            UnityEngine.Debug.Log("Sonido de muerte solicitado al AudioManager.");
        }
        else
        {
            UnityEngine.Debug.LogWarning("AudioManager.instance es nulo. No se puede reproducir el sonido de muerte.");
        }

        // Mostrar el canvas de muerte
        if (canvasMuerte != null)
        {
            canvasMuerte.SetActive(true);
        }

        // Pausar o ralentizar el juego
        Time.timeScale = tiempoAlMorir;
        // Aquí podrías añadir otras lógicas de muerte, como desactivar controles del jugador, etc.
    }

    // Método centralizado para la lógica de reaparición
    void RealizarRespawn()
    {
        if (!estaMuerto && !(canvasMuerte != null && canvasMuerte.activeSelf))
        {
            transform.position = currentCheckpointPosition;
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            UnityEngine.Debug.Log("Jugador ha reseteado a: " + currentCheckpointPosition);
            return;
        }

        estaMuerto = false;
        transform.position = currentCheckpointPosition; // Mover al jugador al checkpoint

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (canvasMuerte != null)
        {
            canvasMuerte.SetActive(false);
        }

        Time.timeScale = tiempoNormal;

        UnityEngine.Debug.Log("Jugador ha respawneado en: " + currentCheckpointPosition);
    }

    public void RespawnDesdeBoton()
    {
        if (estaMuerto && canvasMuerte != null && canvasMuerte.activeSelf)
        {
            RealizarRespawn();
        }
        else if (!estaMuerto)
        {
            UnityEngine.Debug.LogWarning("RespawnDesdeBoton llamado cuando el jugador no está muerto.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (estaMuerto)
            {
                RealizarRespawn();
            }
            else
            {
                UnityEngine.Debug.Log("Jugador vivo presionó R. Reseteando a checkpoint.");
                transform.position = currentCheckpointPosition;
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        if (transform.position.y < -10f && !estaMuerto)
        {
            UnityEngine.Debug.Log("Jugador ha caído fuera del mapa y ha muerto.");
            Morir();
        }
    }

    public static void ActualizarCheckpoint(Vector3 nuevaPosicionCheckpoint)
    {
        currentCheckpointPosition = nuevaPosicionCheckpoint;
        UnityEngine.Debug.Log("Checkpoint actualizado a: " + nuevaPosicionCheckpoint);
    }
}