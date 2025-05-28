using UnityEngine;

public class TrampaBolaEspinas : MonoBehaviour
{
    [Header("Configuración de la Trampa")]
    [Tooltip("Arrastra aquí el GameObject de la Bola de Espinas.")]
    public GameObject bolaDeEspinas;

    [Tooltip("Velocidad a la que se disparará la bola hacia el jugador.")]
    public float velocidadDisparo = 15f;

    [Tooltip("¿Debería la trampa activarse solo una vez?")]
    public bool activarSoloUnaVez = true;

    private Rigidbody rigidbodyDeLaBola;
    private bool trampaActivada = false;

    void Start()
    {
        if (bolaDeEspinas == null)
        {
            UnityEngine.Debug.LogError("Trampa (" + gameObject.name + "): ¡No se ha asignado el GameObject 'bolaDeEspinas' en el Inspector!");
            enabled = false; // Desactiva este script si no hay bola asignada
            return;
        }

        rigidbodyDeLaBola = bolaDeEspinas.GetComponent<Rigidbody>();
        if (rigidbodyDeLaBola == null)
        {
            UnityEngine.Debug.LogError("Trampa (" + gameObject.name + "): El GameObject 'bolaDeEspinas' (" + bolaDeEspinas.name + ") no tiene un componente Rigidbody. No se puede disparar.");
            enabled = false; // Desactiva este script si la bola no tiene Rigidbody
            return;
        }

        // Si la bola no está sujeta por un HingeJoint, la hacemos kinemática al inicio
        // para que no caiga por gravedad hasta que se dispare.
        if (bolaDeEspinas.GetComponent<HingeJoint>() == null)
        {
            rigidbodyDeLaBola.isKinematic = true;
        }
        // Considera también si quieres desactivar la gravedad explícitamente al inicio
        // y activarla solo al disparar, independientemente del HingeJoint.
        // rigidbodyDeLaBola.useGravity = false; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activarSoloUnaVez && trampaActivada)
        {
            return; // Si ya se activó y es de un solo uso, no hacer nada
        }

        if (other.CompareTag("Player")) // Asegúrate de que tu jugador tenga el tag "Player"
        {
            UnityEngine.Debug.Log("Trampa (" + gameObject.name + "): Jugador detectado. ¡Disparando la bola!");

            DispararBolaHaciaObjetivo(other.transform); // Pasamos la transformación del jugador

            if (activarSoloUnaVez)
            {
                trampaActivada = true;
                // Opcional: Desactivar el trigger de esta trampa para que no se vuelva a llamar
                // GetComponent<Collider>().enabled = false;
            }
        }
    }

    void DispararBolaHaciaObjetivo(Transform objetivo)
    {
        if (rigidbodyDeLaBola == null)
        {
            UnityEngine.Debug.LogError("Trampa (" + gameObject.name + "): Rigidbody de la bola es nulo. No se puede disparar.");
            return;
        }

        // Si la bola está sujeta por un HingeJoint, destrúyelo para liberarla.
        HingeJoint jointExistente = bolaDeEspinas.GetComponent<HingeJoint>();
        if (jointExistente != null)
        {
            Destroy(jointExistente);
            UnityEngine.Debug.Log("Trampa (" + gameObject.name + "): HingeJoint existente en la bola destruido para permitir el disparo.");
        }

        // Asegurarse de que el Rigidbody no sea kinemático y use gravedad
        rigidbodyDeLaBola.isKinematic = false;
        rigidbodyDeLaBola.useGravity = true; // Importante para que la física y la gravedad tengan efecto

        // Calcular la dirección desde la bola hacia el objetivo (jugador)
        Vector3 posicionBola = bolaDeEspinas.transform.position;
        Vector3 posicionObjetivo = objetivo.position; // Podrías ajustar esto a una parte específica del jugador, como el torso.

        Vector3 direccionDisparo = (posicionObjetivo - posicionBola).normalized;

        // Aplicar la velocidad al Rigidbody de la bola
        rigidbodyDeLaBola.velocity = Vector3.zero; // Detener cualquier movimiento previo
        rigidbodyDeLaBola.angularVelocity = Vector3.zero; // Detener cualquier rotación previa
        rigidbodyDeLaBola.velocity = direccionDisparo * velocidadDisparo;

        UnityEngine.Debug.Log("Trampa (" + gameObject.name + "): Bola disparada hacia " + objetivo.name + " con velocidad " + velocidadDisparo + " en dirección " + direccionDisparo);
    }
}