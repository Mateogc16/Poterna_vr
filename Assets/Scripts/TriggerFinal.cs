using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerFinal : MonoBehaviour
{
    [Header("Configuración del Trigger")]
    [Tooltip("Arrastra aquí el Canvas que quieres mostrar.")]
    public GameObject canvasParaMostrar;

    [Tooltip("Tag del objeto que activará este trigger (normalmente el jugador).")]
    public string tagDelActivador = "Player";

    [Tooltip("¿Debería este trigger activarse solo una vez?")]
    public bool activarSoloUnaVez = true;

    [Tooltip("¿Debería pausarse el juego al activar este trigger?")]
    public bool pausarJuegoAlActivar = true;

    private bool yaActivado = false;
    private Collider triggerCollider;

    void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
        {
            UnityEngine.Debug.LogError("TriggerFinal (" + gameObject.name + "): No se encontró un componente Collider. El trigger no funcionará.");
            enabled = false;
            return;
        }

        if (!triggerCollider.isTrigger)
        {
            UnityEngine.Debug.LogWarning("TriggerFinal (" + gameObject.name + "): El Collider no está marcado como 'Is Trigger'. Se marcará automáticamente, pero verifica la configuración.");
            triggerCollider.isTrigger = true;
        }

        if (canvasParaMostrar == null)
        {
            UnityEngine.Debug.LogError("TriggerFinal (" + gameObject.name + "): No se ha asignado 'Canvas Para Mostrar' en el Inspector. Este script no funcionará correctamente.");
            enabled = false;
            return;
        }

        canvasParaMostrar.SetActive(false); // Asegurarse de que el canvas esté oculto al inicio
    }

    void OnTriggerEnter(Collider other)
    {
        if ((activarSoloUnaVez && yaActivado) || canvasParaMostrar == null)
        {
            return;
        }

        if (other.CompareTag(tagDelActivador))
        {
            UnityEngine.Debug.Log("TriggerFinal (" + gameObject.name + "): Activado por " + other.name + ".");

            yaActivado = true; // Marcar como activado

            // 1. Silenciar todos los sonidos continuos a través del AudioManager
            if (AudioManager.instance != null)
            {
                AudioManager.instance.DetenerTodosLosSonidosContinuos();
                UnityEngine.Debug.Log("TriggerFinal: Todos los sonidos continuos detenidos.");

                // 2. Reproducir el sonido final bueno
                AudioManager.instance.PlaySonidoFinalBueno();
                UnityEngine.Debug.Log("TriggerFinal: Reproduciendo sonido final bueno.");
            }
            else
            {
                UnityEngine.Debug.LogWarning("TriggerFinal (" + gameObject.name + "): AudioManager.instance no encontrado. No se pudieron detener sonidos ni reproducir el sonido final.");
            }

            // 3. Mostrar el Canvas.
            canvasParaMostrar.SetActive(true);
            UnityEngine.Debug.Log("TriggerFinal: Mostrando canvas final.");

            // 4. Pausar el juego (opcional).
            if (pausarJuegoAlActivar)
            {
                Time.timeScale = 0f;
                UnityEngine.Debug.Log("TriggerFinal: Juego pausado.");
            }

            if (activarSoloUnaVez)
            {
                // Considera desactivar el script o el collider si ya no se necesita
                // enabled = false; 
                // triggerCollider.enabled = false;
            }

            // Lógica del cursor si es un juego de PC y se pausa
            if (pausarJuegoAlActivar)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

   
}