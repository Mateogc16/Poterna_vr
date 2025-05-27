using UnityEngine;
using TMPro; // Asegúrate de tener TextMeshPro importado si lo usas

public class ControladorPuerta : MonoBehaviour
{
    public Transform jugador;
    public float distanciaApertura = 3f;
    public float velocidadApertura = 2f;
    public float anguloApertura = 90f;

    public AudioSource audioSource;
    public AudioClip sonidoApertura;
    public AudioClip sonidoCierre;

    public bool requiereItem = false;
    public string requiredItem;
    private Inventario inventario; // Asegúrate de que tienes una clase Inventario en tu proyecto

    public GameObject mensajePanel;
    public TextMeshProUGUI mensajeTexto;

    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;
    private bool puertaAbierta = false;

    void Start()
    {
        rotacionInicial = transform.rotation;
        rotacionFinal = rotacionInicial * Quaternion.Euler(0, anguloApertura, 0);

        inventario = jugador != null ? jugador.GetComponent<Inventario>() : null;
        if (requiereItem && inventario == null)
        {
            // CORRECCIÓN AQUÍ:
            UnityEngine.Debug.LogError("El jugador no tiene un Inventario asignado o el componente Inventario no se encontró en el jugador.");
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

        if (distancia < distanciaApertura)
        {
            if (!puertaAbierta)
            {
                if (!requiereItem || (inventario != null && inventario.HasItem(requiredItem)))
                {
                    puertaAbierta = true;

                    if (audioSource && sonidoApertura)
                        audioSource.PlayOneShot(sonidoApertura);

                    if (mensajePanel != null)
                        mensajePanel.SetActive(false);

                    // CORRECCIÓN AQUÍ:
                    UnityEngine.Debug.Log("¡Puerta abierta!");
                }
                else
                {
                    if (mensajePanel != null && mensajeTexto != null)
                    {
                        mensajeTexto.text = "Necesitas " + requiredItem + " para continuar.";
                        mensajePanel.SetActive(true);
                    }
                    // CORRECCIÓN AQUÍ:
                    UnityEngine.Debug.Log("Necesitas " + requiredItem + " para abrir esta puerta.");
                }
            }
        }
        else
        {
            if (mensajePanel != null)
            {
                mensajePanel.SetActive(false);
            }

            // Lógica opcional para cerrar la puerta si el jugador se aleja:
            // if (puertaAbierta && !requiereItem)
            // {
            //     puertaAbierta = false;
            //     if (audioSource && sonidoCierre)
            //         audioSource.PlayOneShot(sonidoCierre);
            //     UnityEngine.Debug.Log("¡Puerta cerrada por distancia!"); // También necesitaría UnityEngine.Debug aquí
            // }
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, puertaAbierta ? rotacionFinal : rotacionInicial, Time.deltaTime * velocidadApertura);
    }

    public void ForzarApertura()
    {
        if (!puertaAbierta)
        {
            puertaAbierta = true;

            if (audioSource && sonidoApertura)
                audioSource.PlayOneShot(sonidoApertura);

            if (mensajePanel != null)
                mensajePanel.SetActive(false);

            // CORRECCIÓN AQUÍ:
            UnityEngine.Debug.Log("¡Puerta abierta forzada!");
        }
    }

    public void ForzarCierre()
    {
        if (puertaAbierta)
        {
            puertaAbierta = false;

            if (audioSource && sonidoCierre)
                audioSource.PlayOneShot(sonidoCierre);

            // CORRECCIÓN AQUÍ:
            UnityEngine.Debug.Log("¡Puerta cerrada forzada!");
        }
    }
}