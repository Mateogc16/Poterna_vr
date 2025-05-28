using UnityEngine;

// El nombre de la clase ahora es BolaEspinas para que coincida con tu archivo .cs
public class BolaEspinas : MonoBehaviour
{
    [Tooltip("Tag del objeto que debe ser afectado al colisionar (ej. 'Player').")]
    public string tagDelObjetivo = "Player";

    [Tooltip("¿Debería la bola destruirse a sí misma después de golpear al objetivo?")]
    public bool destruirBolaAlImpacto = false;

    // Opcional: Efecto a instanciar al impactar (ej. una pequeña explosión)
    // public GameObject efectoImpactoPrefab;

    private static bool juegoPausadoPorTrampa = false;
    private bool estaBolaYaPausoElJuego = false; // Para evitar que la misma bola pause múltiples veces

    void OnCollisionEnter(Collision collision)
    {
        // Si el juego ya está pausado por CUALQUIER trampa de bola, o si ESTA bola ya causó una pausa, no hacer nada más.
        if (juegoPausadoPorTrampa || estaBolaYaPausoElJuego)
        {
            // Si la bola debe destruirse al impacto independientemente de si ya pausó,
            // podrías mover esa lógica aquí, pero usualmente se liga al primer impacto dañino.
            if (destruirBolaAlImpacto && collision.gameObject.CompareTag(tagDelObjetivo) && !estaBolaYaPausoElJuego)
            {
                // Asegurarse de que solo se destruya una vez por colisión con el objetivo
                estaBolaYaPausoElJuego = true; // Usamos esta misma bandera para el control de destrucción
                Destroy(gameObject, 0.1f);
            }
            return;
        }

        if (collision.gameObject.CompareTag(tagDelObjetivo))
        {
            UnityEngine.Debug.Log(gameObject.name + " ha colisionado con " + collision.gameObject.name + " (Tag: " + tagDelObjetivo + "). ¡Juego Pausado!");

            // Pausar el juego
            Time.timeScale = 0f;
            juegoPausadoPorTrampa = true;
            estaBolaYaPausoElJuego = true; // Esta bola específica ha cumplido su función de pausar

            // Aquí podrías activar una UI de "Game Over" o "Has Muerto", etc.
            // Ejemplo: UIManager.Instance.MostrarPantallaMuerte();

            // Opcional: Instanciar un efecto de impacto
            // if (efectoImpactoPrefab != null)
            // {
            //     Instantiate(efectoImpactoPrefab, collision.contacts[0].point, Quaternion.identity);
            // }

            if (destruirBolaAlImpacto)
            {
                // Destruir la bola de espinas después de un pequeño retraso para que se vea el impacto
                Destroy(gameObject, 0.1f);
            }
        }
    }

    // Este método DEBE ser llamado por tu sistema de respawn cuando el jugador reaparece.
    // Es estático para que sea fácil de llamar desde cualquier script sin necesitar una referencia a esta bola específica.
    public static void ReanudarJuegoTrasRespawn()
    {
        if (juegoPausadoPorTrampa) // Solo reanudar si fue una trampa de bola la que pausó
        {
            Time.timeScale = 1f;
            juegoPausadoPorTrampa = false; // Resetear el estado global de pausa por trampa
            UnityEngine.Debug.Log("BolaEspinas: Juego reanudado después del respawn."); // Nombre de clase actualizado aquí
        }
    }

    // Opcional: Resetear el estado de esta bola si se resetea la trampa sin cambiar de escena
    // (por ejemplo, si la bola no se destruye y la trampa se puede reactivar)
    public void ResetearEstadoDeEstaBola()
    {
        estaBolaYaPausoElJuego = false;
    }

    // Comentarios finales sobre la lógica de pausa y destrucción.
    // Es buena práctica asegurarse de que si este objeto se destruye mientras el juego está pausado por él,
    // y no hay un sistema de respawn que llame a ReanudarJuegoTrasRespawn, el juego podría quedar pausado.
    // Sin embargo, la lógica de reanudar está pensada para el respawn del jugador.
    // Si la bola se destruye, el estado 'juegoPausadoPorTrampa' sigue siendo true hasta que el jugador respawnee.
}