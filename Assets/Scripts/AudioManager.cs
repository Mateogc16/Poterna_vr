using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HingeJoint))]
public class BolaDePinchos : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isActive = false;
    public float initialPushForce = 1.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        if (!CompareTag("Trampa"))
        {
            UnityEngine.Debug.LogWarning("La BolaDePinchos '" + gameObject.name + "' no tiene el tag 'Trampa'. No dañará al jugador.", this);
        }
    }

    void Start()
    {
        ResetTrapInternal(false);
    }

    public void ActivarBalanceo(Vector3 targetPlayerPosition)
    {
        if (isActive) return;

        UnityEngine.Debug.Log("BolaDePinchos '" + gameObject.name + "' activada. Soltando para oscilar.");
        isActive = true;
        rb.isKinematic = false;

        if (initialPushForce > 0)
        {
            Vector3 directionToTarget = (targetPlayerPosition - transform.position).normalized;
            rb.AddForce(directionToTarget * initialPushForce, ForceMode.Impulse);
        }
    }

    private void ResetTrapInternal(bool logMessage)
    {
        isActive = false;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        if (logMessage)
        {
            UnityEngine.Debug.Log("BolaDePinchos '" + gameObject.name + "' reseteada.");
        }
    }

    public void ResetearTrampa()
    {
        ResetTrapInternal(true);
    }
}
