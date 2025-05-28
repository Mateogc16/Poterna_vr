using UnityEngine;
using System.Collections;

// Este componente es necesario si vas a usar animaciones.
[RequireComponent(typeof(Animator))]
public class ENEMIGO : MonoBehaviour
{
    [Header("Configuración de Persecución")]
    [Tooltip("Velocidad a la que el NPC perseguirá al jugador.")]
    public float velocidadPersecucion = 3.0f;
    [Tooltip("Velocidad de rotación del NPC al mirar al jugador.")]
    public float velocidadRotacion = 5f;
    [Tooltip("Distancia a la que el NPC dejará de perseguir si el jugador se aleja mucho (0 para perseguir siempre).")]
    public float distanciaMaximaPersecucion = 25f;
    [Tooltip("Retraso en segundos después de la activación antes de que comience la persecución.")]
    public float retrasoAntesDePerseguir = 5.0f;
    [Tooltip("Distancia mínima al jugador para considerar que lo ha atrapado.")]
    public float distanciaParaAtrapar = 1.5f;


    [Header("Configuración de Evasión de Obstáculos")]
    [Tooltip("Distancia del raycast para detectar obstáculos al frente.")]
    public float distanciaDeteccionObstaculo = 1.5f;
    [Tooltip("Ángulo de giro en grados cuando se detecta un obstáculo.")]
    public float anguloDeGiroEvasion = 45.0f;
    [Tooltip("LayerMask para los obstáculos que el enemigo debe evitar (ej. 'Paredes', 'Default').")]
    public LayerMask mascaraDeObstaculos;
    [Tooltip("Offset vertical para el origen de los raycasts de detección de obstáculos.")]
    public float raycastVerticalOffset = 0.5f;


    [Header("Referencias")]
    [Tooltip("Transform del jugador. Si se deja vacío, se buscará por el tag 'Player'.")]
    public Transform jugadorTransform;

    private Animator animator;
    private PlayerRespawn jugadorRespawn;

    private bool haSidoActivado = false;
    private bool estaPersiguiendo = false;
    private float tiempoDesdeUltimoGiroEvasion = 0f;
    private float intervaloMinimoEntreGiros = 0.3f;

    // Hashes de los parámetros del Animator para optimización
    private int isChasingAnimHash;
    // Podrías añadir un hash para una animación de "idle" si la tienes
    // private int isIdleAnimHash;

    void Awake()
    {
        animator = GetComponent<Animator>();
        // Asegúrate de que el parámetro en tu Animator Controller se llame exactamente "IsChasing"
        isChasingAnimHash = Animator.StringToHash("IsChasing");
        // isIdleAnimHash = Animator.StringToHash("IsIdle");

        if (animator == null)
        {
            UnityEngine.Debug.LogError("ENEMIGO (" + gameObject.name + "): No se encontró el componente Animator.");
            enabled = false; // Desactivar el script si no hay animator
        }
    }

    void Start()
    {
        // Intentar encontrar al jugador si no está asignado
        if (jugadorTransform == null)
        {
            GameObject jugadorGO = GameObject.FindGameObjectWithTag("Player");
            if (jugadorGO != null)
            {
                jugadorTransform = jugadorGO.transform;
            }
        }

        // Obtener referencia al script de respawn del jugador
        if (jugadorTransform != null)
        {
            jugadorRespawn = jugadorTransform.GetComponent<PlayerRespawn>();
        }

        if (jugadorTransform == null)
        {
            UnityEngine.Debug.LogError("ENEMIGO (" + gameObject.name + "): No se pudo encontrar al jugador (Transform). La persecución no funcionará.");
            enabled = false; // Desactivar si no hay jugador
            return;
        }
        if (jugadorRespawn == null)
        {
            UnityEngine.Debug.LogError("ENEMIGO (" + gameObject.name + "): No se pudo encontrar el script PlayerRespawn en el jugador. El enemigo no podrá 'matar' al jugador.");
        }

        // El NPC comienza inactivo y sin perseguir
        if (animator != null)
        {
            animator.SetBool(isChasingAnimHash, false);
            // animator.SetBool(isIdleAnimHash, true); // Si tienes un estado de idle
        }
    }

    // ESTE MÉTODO DEBE SER LLAMADO POR OTRO SCRIPT (EJ. EL DE LA PUERTA O UN TRIGGER)
    public void ActivarNPC()
    {
        if (haSidoActivado)
        {
            UnityEngine.Debug.Log("ENEMIGO (" + gameObject.name + "): Ya ha sido activado previamente.");
            return;
        }
        haSidoActivado = true;
        UnityEngine.Debug.Log("ENEMIGO (" + gameObject.name + "): Activado. Reproduciendo sonido característico...");

        // 1. Reproducir el audio del ENEMIGO
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayRuidosCarelibro();
        }
        else
        {
            UnityEngine.Debug.LogWarning("ENEMIGO (" + gameObject.name + "): AudioManager no encontrado. No se puede reproducir el sonido 'ruidosCarelibro'.");
        }

        // 2. Iniciar corutina para el retraso antes de perseguir
        StartCoroutine(RutinaDeActivacion());
    }

    IEnumerator RutinaDeActivacion()
    {
        UnityEngine.Debug.Log("ENEMIGO (" + gameObject.name + "): Esperando " + retrasoAntesDePerseguir + " segundos para iniciar la persecución...");
        yield return new WaitForSeconds(retrasoAntesDePerseguir);

        UnityEngine.Debug.Log("ENEMIGO (" + gameObject.name + "): ¡Comenzando persecución!");
        estaPersiguiendo = true;
        if (animator != null)
        {
            animator.SetBool(isChasingAnimHash, true);
            // animator.SetBool(isIdleAnimHash, false);
        }

        // Iniciar música de persecución
        // <--- AQUÍ SE INICIA LA MÚSICA DE PERSECUCIÓN --->
        if (AudioManager.instance != null)
        {
            AudioManager.instance.IniciarMusicaPersecucion();
        }
    }

    void Update()
    {
        if (!haSidoActivado || !estaPersiguiendo || jugadorTransform == null)
        {
            return;
        }

        float distanciaAlJugador = Vector3.Distance(transform.position, jugadorTransform.position);

        if (distanciaMaximaPersecucion > 0 && distanciaAlJugador > distanciaMaximaPersecucion)
        {
            UnityEngine.Debug.Log("ENEMIGO (" + gameObject.name + "): Jugador demasiado lejos. Deteniendo persecución.");
            DetenerPersecucion();
            return;
        }

        if (distanciaAlJugador <= distanciaParaAtrapar)
        {
            AtraparJugador();
            return;
        }

        if (animator != null && !animator.GetBool(isChasingAnimHash))
        {
            animator.SetBool(isChasingAnimHash, true);
            if (AudioManager.instance != null && AudioManager.instance.musicaPersecucionSource != null && !AudioManager.instance.musicaPersecucionSource.isPlaying)
            {
                AudioManager.instance.IniciarMusicaPersecucion();
            }
        }

        MoverYEvitarObstaculosHaciaJugador();
    }

    void MoverYEvitarObstaculosHaciaJugador()
    {
        Vector3 direccionAlJugador = (jugadorTransform.position - transform.position);
        direccionAlJugador.y = 0;
        direccionAlJugador = direccionAlJugador.normalized;

        if (direccionAlJugador != Vector3.zero)
        {
            Quaternion rotacionDeseada = Quaternion.LookRotation(direccionAlJugador);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, velocidadRotacion * Time.deltaTime);
        }

        RaycastHit hit;
        Vector3 origenRaycast = transform.position + Vector3.up * raycastVerticalOffset + transform.forward * 0.1f;

        bool obstaculoAlFrente = Physics.Raycast(origenRaycast, transform.forward, out hit, distanciaDeteccionObstaculo, mascaraDeObstaculos);
        UnityEngine.Debug.DrawRay(origenRaycast, transform.forward * distanciaDeteccionObstaculo, obstaculoAlFrente ? Color.red : Color.green);

        if (obstaculoAlFrente)
        {
            if (Time.time > tiempoDesdeUltimoGiroEvasion + intervaloMinimoEntreGiros)
            {
                float anguloGiro = UnityEngine.Random.Range(0, 2) == 0 ? anguloDeGiroEvasion : -anguloDeGiroEvasion;
                transform.Rotate(0, anguloGiro, 0);
                tiempoDesdeUltimoGiroEvasion = Time.time;
                return;
            }
        }

        transform.Translate(Vector3.forward * velocidadPersecucion * Time.deltaTime);
    }

    void AtraparJugador()
    {
        UnityEngine.Debug.Log("ENEMIGO (" + gameObject.name + "): ¡Ha atrapado al jugador!");
        if (jugadorRespawn != null)
        {
            jugadorRespawn.Morir();
        }
        else
        {
            UnityEngine.Debug.LogError("ENEMIGO (" + gameObject.name + "): No se puede llamar a Morir() porque jugadorRespawn es nulo.");
        }
        DetenerPersecucion();
    }

    void DetenerPersecucion()
    {
        estaPersiguiendo = false;
        if (animator != null)
        {
            animator.SetBool(isChasingAnimHash, false);
        }
        if (AudioManager.instance != null)
        {
            AudioManager.instance.DetenerMusicaPersecucion();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!estaPersiguiendo || !haSidoActivado) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            AtraparJugador();
        }
    }

    void OnDisable()
    {
        if (estaPersiguiendo && AudioManager.instance != null)
        {
            AudioManager.instance.DetenerMusicaPersecucion();
        }
    }

    void OnDestroy()
    {
        if (estaPersiguiendo && AudioManager.instance != null)
        {
            AudioManager.instance.DetenerMusicaPersecucion();
        }
    }
}