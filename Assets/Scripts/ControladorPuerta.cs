using UnityEngine;
using TMPro;

public class ControladorPuerta : MonoBehaviour
{
    [Header("Configuración General")]
    public Transform jugador;
    public float distanciaApertura = 3f;
    public float velocidadApertura = 2f;
    public float anguloApertura = 90f;

    [Header("Requisito de Ítem")]
    public bool requiereItem = false;
    public string itemNameRequerido = "El corazon indescifrable"; // Nombre del ítem para esta puerta
    private Inventario inventario;

    [Header("Mensajes UI")]
    public GameObject mensajePanel;
    public TextMeshProUGUI mensajeTexto;

    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;
    private bool puertaAbierta = false;
    private bool reproduciendoSonidoCierre = false;

    // Getter público para saber el estado de la puerta
    public bool IsPuertaAbierta()
    {
        return puertaAbierta;
    }

    void Start()
    {
        rotacionInicial = transform.rotation;
        rotacionFinal = rotacionInicial * Quaternion.Euler(0, anguloApertura, 0);

        if (jugador != null)
        {
            inventario = jugador.GetComponent<Inventario>();
            if (requiereItem && inventario == null)
            {
                UnityEngine.Debug.LogError("ControladorPuerta (" + gameObject.name + "): El jugador '" + jugador.name + "' no tiene un componente Inventario y la puerta requiere un ítem.");
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("ControladorPuerta (" + gameObject.name + "): Jugador no asignado. La funcionalidad de apertura por proximidad y por ítem no funcionará.");
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
        bool puedeAbrirConItem = requiereItem && inventario != null && inventario.HasItem(itemNameRequerido);
        bool debeAbrir = false;

        if (distancia < distanciaApertura)
        {
            if (!requiereItem || puedeAbrirConItem)
            {
                debeAbrir = true;
                if (mensajePanel != null && mensajePanel.activeSelf)
                {
                    mensajePanel.SetActive(false);
                }
            }
            else
            {
                if (!puertaAbierta && mensajePanel != null && mensajeTexto != null)
                {
                    mensajeTexto.text = "Necesitas " + itemNameRequerido + " para abrir esta puerta.";
                    mensajePanel.SetActive(true);
                }
            }
        }
        else
        {
            if (mensajePanel != null && mensajePanel.activeSelf)
            {
                mensajePanel.SetActive(false);
            }
        }

        if (debeAbrir && !puertaAbierta)
        {
            puertaAbierta = true;
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayAbrirPuerta();
            }
            else
            {
                UnityEngine.Debug.LogWarning("ControladorPuerta (" + gameObject.name + "): AudioManager.instance no encontrado!");
            }
            UnityEngine.Debug.Log("ControladorPuerta (" + gameObject.name + "): ¡Puerta abierta!");
            reproduciendoSonidoCierre = false;
        }
        else if (!debeAbrir && puertaAbierta)
        {
            // Lógica para cerrar la puerta si se implementa cierre automático
            // Por ejemplo:
            // puertaAbierta = false;
            // if (AudioManager.instance != null && !reproduciendoSonidoCierre)
            // {
            //    AudioManager.instance.PlayCerrarPuerta();
            //    reproduciendoSonidoCierre = true;
            // }
            // UnityEngine.Debug.Log("ControladorPuerta (" + gameObject.name + "): ¡Puerta cerrándose!");
        }

        Quaternion targetRotation = puertaAbierta ? rotacionFinal : rotacionInicial;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * velocidadApertura);

        if (!puertaAbierta && reproduciendoSonidoCierre)
        {
            if (Quaternion.Angle(transform.rotation, rotacionInicial) < 1.0f)
            {
                reproduciendoSonidoCierre = false;
            }
        }
    }

    public void ForzarApertura()
    {
        if (!puertaAbierta)
        {
            puertaAbierta = true;
            if (AudioManager.instance != null) AudioManager.instance.PlayAbrirPuerta();
            if (mensajePanel != null) mensajePanel.SetActive(false);
            UnityEngine.Debug.Log("ControladorPuerta (" + gameObject.name + "): ¡Puerta abierta forzada!");
            reproduciendoSonidoCierre = false;
        }
    }

    public void ForzarCierre()
    {
        if (puertaAbierta)
        {
            puertaAbierta = false;
            if (AudioManager.instance != null && !reproduciendoSonidoCierre)
            {
                AudioManager.instance.PlayCerrarPuerta();
                reproduciendoSonidoCierre = true;
            }
            else if (AudioManager.instance == null)
            {
                UnityEngine.Debug.LogWarning("ControladorPuerta (" + gameObject.name + "): AudioManager.instance no encontrado!");
            }
            UnityEngine.Debug.Log("ControladorPuerta (" + gameObject.name + "): ¡Puerta cerrada forzada!");
        }
    }
}