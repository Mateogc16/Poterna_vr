using UnityEngine;

public class ControladorNota : MonoBehaviour
{
    [Header("Configuración de la Nota")]
    [Tooltip("Arrastra aquí el Canvas específico para ESTA nota.")]
    public GameObject miCanvasDeNota;

    // No necesitamos una referencia directa al AudioClip aquí,
    // ya que el AudioManager se encargará de gestionarlo.

    void Start()
    {
        if (miCanvasDeNota != null)
        {
            miCanvasDeNota.SetActive(false);
        }
        else
        {
            UnityEngine.Debug.LogError("ControladorNota (" + gameObject.name + "): No se ha asignado 'Mi Canvas De Nota' en el Inspector. Por favor, asigna un Canvas.");
        }
    }

    // Este método público es el que debes conectar al evento de tu XR Interactable
    // (por ejemplo, 'Activated' en XRGrabInteractable o el evento de selección que uses)
    public void ActivarOcultarNota()
    {
        UnityEngine.Debug.LogWarning("!!! MÉTODO ActivarOcultarNota() EJECUTADO en " + gameObject.name + " !!!");

        if (miCanvasDeNota == null)
        {
            UnityEngine.Debug.LogError("ControladorNota (" + gameObject.name + "): 'Mi Canvas De Nota' es nulo. No se puede activar/ocultar. Asegúrate de haberlo asignado en el Inspector.");
            return;
        }

        bool estadoPrevio = miCanvasDeNota.activeSelf;
        miCanvasDeNota.SetActive(!estadoPrevio);

        if (!estadoPrevio) // Si antes estaba inactivo, ahora está activo (se muestra la nota)
        {
            UnityEngine.Debug.Log("ControladorNota (" + gameObject.name + "): Canvas de nota ACTIVADO.");

            // Intentar reproducir el sonido del papel a través del AudioManager
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayPapel();
                UnityEngine.Debug.Log("ControladorNota (" + gameObject.name + "): Sonido de papel solicitado al AudioManager.");
            }
            else
            {
                UnityEngine.Debug.LogWarning("ControladorNota (" + gameObject.name + "): AudioManager.instance es nulo. No se puede reproducir el sonido del papel. Asegúrate de que haya un AudioManager en la escena.");
            }
        }
        else // Si antes estaba activo, ahora está inactivo (se oculta la nota)
        {
            UnityEngine.Debug.Log("ControladorNota (" + gameObject.name + "): Canvas de nota DESACTIVADO.");
            // Aquí podrías reproducir otro sonido si quisieras, por ejemplo, al cerrar la nota.
        }
    }
}