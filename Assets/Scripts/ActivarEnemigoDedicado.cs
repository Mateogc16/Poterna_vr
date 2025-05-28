using UnityEngine;

// Asegura que este script esté en un GameObject que también tenga el script ControladorPuerta.
[RequireComponent(typeof(ControladorPuerta))]
public class ActivadorEnemigoDedicado : MonoBehaviour
{
    [Header("Referencias y Configuración")]
    [Tooltip("Arrastra aquí el GameObject del ENEMIGO que quieres activar.")]
    public ENEMIGO enemigoParaActivar;

    [Tooltip("Arrastra aquí el GameObject de la PARED cuya malla quieres cambiar.")]
    public CambiadorMeshPared paredParaCambiar; // <--- AÑADIDO AQUÍ

    [Tooltip("Nombre del ítem que, si es requerido por la puerta y esta se abre, activará al enemigo y cambiará la pared.")]
    public string itemCondicionActivacion = "El corazon indescifrable";

    private ControladorPuerta miControladorPuerta;
    private bool eventoYaHaSidoActivado = false; // Renombrado para claridad

    void Awake()
    {
        // Obtener la referencia al ControladorPuerta en el mismo GameObject.
        miControladorPuerta = GetComponent<ControladorPuerta>();

        if (miControladorPuerta == null)
        {
            UnityEngine.Debug.LogError("ActivadorEnemigoDedicado: No se pudo encontrar el script 'ControladorPuerta' en el GameObject: " + gameObject.name + ". Este script no funcionará.");
            enabled = false; // Desactivar este script si no encuentra el controlador de la puerta.
            return;
        }

        if (enemigoParaActivar == null)
        {
            UnityEngine.Debug.LogError("ActivadorEnemigoDedicado: No se ha asignado 'Enemigo Para Activar' en el Inspector de: " + gameObject.name + ". El enemigo no se activará.");
            // No necesariamente desactivar el script completo, podría solo no activar el enemigo.
        }

        if (paredParaCambiar == null)
        {
            UnityEngine.Debug.LogWarning("ActivadorEnemigoDedicado: No se ha asignado 'Pared Para Cambiar' en el Inspector de: " + gameObject.name + ". La malla de la pared no se cambiará.");
            // No necesariamente desactivar, podría solo no cambiar la pared.
        }
    }

    void Update()
    {
        // No hacer nada si el evento ya ocurrió, si no hay puerta o no hay enemigo.
        if (eventoYaHaSidoActivado || miControladorPuerta == null) // No necesitamos verificar enemigoParaActivar aquí si solo queremos que el script no siga corriendo
        {
            return;
        }

        // Comprobar si la puerta está abierta Y si la condición del ítem se cumple.
        // Usamos el método público IsPuertaAbierta() del ControladorPuerta.
        if (miControladorPuerta.IsPuertaAbierta())
        {
            bool condicionDeItemCumplida = false;
            if (miControladorPuerta.requiereItem && miControladorPuerta.itemNameRequerido == itemCondicionActivacion)
            {
                condicionDeItemCumplida = true;
            }

            if (condicionDeItemCumplida)
            {
                // Activar al enemigo (si está asignado)
                if (enemigoParaActivar != null)
                {
                    UnityEngine.Debug.Log("ActivadorEnemigoDedicado: Puerta '" + gameObject.name + "' abierta con '" + itemCondicionActivacion + "'. Activando al ENEMIGO: " + enemigoParaActivar.name);
                    enemigoParaActivar.ActivarNPC();
                }

                // Cambiar la malla de la pared (si está asignada)
                if (paredParaCambiar != null)
                {
                    UnityEngine.Debug.Log("ActivadorEnemigoDedicado: Activando cambio de malla para la pared: " + paredParaCambiar.gameObject.name);
                    paredParaCambiar.CambiarAMeshNuevo(); // <--- AÑADIDO AQUÍ
                }

                eventoYaHaSidoActivado = true;  // Asegurar que solo se active una vez.

                // Opcional: Desactivar este script después de la activación si ya no se necesita.
                // enabled = false;
            }
        }
    }
}