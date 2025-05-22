using UnityEngine;
using TMPro;

public class ControladorPuerta : MonoBehaviour
{
    public Transform jugador;
    public float distanciaApertura = 3f;
    public float velocidadApertura = 2f;
    public float anguloApertura = 90f;

    public AudioSource audioSource;
    public AudioClip sonidoApertura;
    public AudioClip sonidoCierre;

    public bool requiereItem = false; // ¿La puerta necesita un ítem para abrirse?
    public string requiredItem; // Nombre del ítem necesario (si `requiereItem` es `true`)
    private Inventario inventario;

    public GameObject mensajePanel; // Panel de mensaje en la UI
    public TextMeshProUGUI mensajeTexto; // Texto dentro del panel

    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;
    private bool puertaAbierta = false;

    void Start()
    {
        rotacionInicial = Quaternion.Euler(0, 0, 0);
        rotacionFinal = Quaternion.Euler(0, anguloApertura, 0);

        inventario = jugador.GetComponent<Inventario>();
        if (inventario == null)
        {
            UnityEngine.Debug.LogError("El jugador no tiene un Inventario asignado.");
        }

        if (mensajePanel != null)
        {
            mensajePanel.SetActive(false);
        }
    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia < distanciaApertura)
        {
            if (!puertaAbierta)
            {
                if (!requiereItem || (inventario != null && inventario.HasItem(requiredItem)))
                {
                    puertaAbierta = true;
                    if (audioSource && sonidoApertura) audioSource.PlayOneShot(sonidoApertura);
                    UnityEngine.Debug.Log("Puerta abierta!");

                    if (mensajePanel != null)
                    {
                        mensajePanel.SetActive(false);
                    }
                }
                else
                {
                    if (mensajePanel != null && mensajeTexto != null)
                    {
                        mensajeTexto.text = "Necesitas " + requiredItem + " para continuar.";
                        mensajePanel.SetActive(true);
                    }
                    UnityEngine.Debug.Log("Necesitas " + requiredItem + " para abrir esta puerta.");
                }
            }
        }
        else
        {
            // Si el jugador se aleja, ocultar el mensaje del HUD
            if (mensajePanel != null)
            {
                mensajePanel.SetActive(false);
            }
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, puertaAbierta ? rotacionFinal : rotacionInicial, Time.deltaTime * velocidadApertura);
    }
}