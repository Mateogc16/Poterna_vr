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

    public bool requiereItem = false;
    public string requiredItem;
    private Inventario inventario;

    public GameObject mensajePanel;
    public TextMeshProUGUI mensajeTexto;

    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;
    private bool puertaAbierta = false;

    void Start()
    {
        rotacionInicial = Quaternion.Euler(0, 0, 0);
        rotacionFinal = Quaternion.Euler(0, anguloApertura, 0);

        inventario = jugador != null ? jugador.GetComponent<Inventario>() : null;
        if (requiereItem && inventario == null)
        {
            Debug.LogError("El jugador no tiene un Inventario asignado.");
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

                    Debug.Log("¡Puerta abierta!");
                }
                else
                {
                    if (mensajePanel != null && mensajeTexto != null)
                    {
                        mensajeTexto.text = "Necesitas " + requiredItem + " para continuar.";
                        mensajePanel.SetActive(true);
                    }

                    Debug.Log("Necesitas " + requiredItem + " para abrir esta puerta.");
                }
            }
        }
        else
        {
            if (mensajePanel != null)
            {
                mensajePanel.SetActive(false);
            }
        }

        // Mantiene rotación animada si la puerta está abierta
        transform.rotation = Quaternion.Lerp(transform.rotation, puertaAbierta ? rotacionFinal : rotacionInicial, Time.deltaTime * velocidadApertura);
    }

    // NUEVO: Permite abrir la puerta desde otros scripts
    public void ForzarApertura()
    {
        if (!puertaAbierta)
        {
            puertaAbierta = true;

            if (audioSource && sonidoApertura)
                audioSource.PlayOneShot(sonidoApertura);

            if (mensajePanel != null)
                mensajePanel.SetActive(false);

            Debug.Log("¡Puerta abierta forzada!");
        }
    }
}
