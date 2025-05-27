using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HingeJoint))]
public class BolaDePinchos : MonoBehaviour
{
    private Rigidbody rb;
    // private HingeJoint hinge; // No necesitamos interactuar directamente con el HingeJoint vía script para este caso básico

    private Vector3 initialPosition;    // Posición inicial en el mundo
    private Quaternion initialRotation; // Rotación inicial en el mundo

    private bool isActive = false;

    // Opcional: Una pequeña fuerza para "empujar" la bola hacia el objetivo al soltarse
    public float initialPushForce = 1.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // hinge = GetComponent<HingeJoint>(); // Obtenemos la referencia si fuera necesaria

        // Guarda la posición y rotación iniciales tal como está en el editor
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Asegúrate de que la bola tenga el tag "Trampa"
        if (!CompareTag("Trampa"))
        {
            UnityEngine.Debug.LogWarning("La BolaDePinchos '" + gameObject.name + "' no tiene el tag 'Trampa'. No dañará al jugador.", this);
        }
    }

    void Start()
    {
        // Al inicio del juego, asegúrate de que la bola esté en su posición inicial
        // y sea cinemática (no afectada por la física hasta que se active).
        // Esto te permite colocarla en el editor en su posición "lista para soltar".
        ResetTrapInternal(false); // No mostrar mensaje de reset al inicio
    }

    /// <summary>
    /// Activa la bola para que comience a oscilar.
    /// </summary>
    /// <param name="targetPlayerPosition">La posición donde estaba el jugador al activar la trampa.</param>
    public void ActivarBalanceo(Vector3 targetPlayerPosition)
    {
        if (isActive) return;

        UnityEngine.Debug.Log("BolaDePinchos '" + gameObject.name + "' activada. Soltando para oscilar.");
        isActive = true;
        rb.isKinematic = false; // ¡La soltamos! La gravedad y el HingeJoint harán su trabajo.

        // Opcional: Aplicar una pequeña fuerza inicial hacia la posición del jugador
        // Esto puede ayudar a dirigir el péndulo si su balanceo natural no es suficiente.
        // Ajusta initialPushForce según sea necesario, o ponlo a 0 si no quieres este empuje.
        if (initialPushForce > 0)
        {
            Vector3 directionToTarget = (targetPlayerPosition - transform.position).normalized;
            rb.AddForce(directionToTarget * initialPushForce, ForceMode.Impulse);
        }
    }

    private void ResetTrapInternal(bool logMessage)
    {
        isActive = false;
        rb.isKinematic = true; // La hacemos cinemática de nuevo
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;   // Resetea a su posición inicial
        transform.rotation = initialRotation; // Resetea a su rotación inicial
        if (logMessage)
        {
            UnityEngine.Debug.Log("BolaDePinchos '" + gameObject.name + "' reseteada.");
        }
    }

    /// <summary>
    /// Resetea la trampa a su estado inicial (por ejemplo, si el jugador muere antes de que la bola golpee).
    /// Puedes llamar a esto desde otro script si es necesario.
    /// </summary>
    public void ResetearTrampa()
    {
        ResetTrapInternal(true);
    }
}