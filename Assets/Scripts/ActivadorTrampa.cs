using UnityEngine;

public class ActivadorTrampa : MonoBehaviour
{
    public BolaDePinchos bolaDePinchosControlada; // Arrastra la BolaDePinchos aquí en el Inspector
    public string playerTag = "Player";
    private bool haSidoActivado = false;

    void Awake()
    {
        if (bolaDePinchosControlada == null)
        {
            UnityEngine.Debug.LogError("No se ha asignado una BolaDePinchos al ActivadorTrampaBola en '" + gameObject.name + "'.", this);
        }
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
        {
            UnityEngine.Debug.LogError("ActivadorTrampaBola en '" + gameObject.name + "' necesita un Collider con 'Is Trigger' marcado.", this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (haSidoActivado) return; // Solo se activa una vez

        if (other.CompareTag(playerTag))
        {
            if (bolaDePinchosControlada != null)
            {
                UnityEngine.Debug.Log("Jugador (" + other.name + ") activó la trampa de bola en: " + other.transform.position);
                // Le pasamos la posición del jugador en el momento de la activación
                bolaDePinchosControlada.ActivarBalanceo(other.transform.position);
                haSidoActivado = true;

                // Opcional: Desactivar este trigger después de usarlo
                // gameObject.SetActive(false);
            }
        }
    }

    // Opcional: si necesitas resetear el activador también
    public void ResetearActivador()
    {
        haSidoActivado = false;
        // gameObject.SetActive(true); // Si lo desactivaste
    }
}