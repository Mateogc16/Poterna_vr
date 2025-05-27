using UnityEngine;
using System.Collections;

public class EstatuaReaccion : MonoBehaviour
{
    public GameObject puerta;                    // Arrastra aquí la puerta con el script ControladorPuerta
    public AudioSource audioSource;              // Fuente de audio en la estatua
    public AudioClip sonidoActivacion;           // Sonido a reproducir cuando el corazón la toca
    public string tagCorazon = "Corazon";        // Asegúrate de que el corazón tenga este tag

    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!activado && other.CompareTag(tagCorazon))
        {
            activado = true;

            if (audioSource && sonidoActivacion)
            {
                audioSource.PlayOneShot(sonidoActivacion);
            }

            StartCoroutine(AbrirPuertaConRetraso(3f));
        }
    }

    private IEnumerator AbrirPuertaConRetraso(float delay)
    {
        yield return new WaitForSeconds(delay);

        ControladorPuerta controlador = puerta.GetComponent<ControladorPuerta>();
        if (controlador != null)
        {
            controlador.ForzarApertura();
        }
        else
        {
            Debug.LogWarning("No se encontró el script ControladorPuerta en el objeto de la puerta.");
        }
    }
}
