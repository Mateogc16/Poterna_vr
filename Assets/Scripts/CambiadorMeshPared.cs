using UnityEngine;

// Asegúrate de que el GameObject tenga un MeshFilter.
[RequireComponent(typeof(MeshFilter))]
public class CambiadorMeshPared : MonoBehaviour
{
    [Header("Configuración de Mallas")]
    [Tooltip("Arrastra aquí la malla que quieres que la pared tenga inicialmente. Si se deja vacío, se usará la malla actual del MeshFilter.")]
    public Mesh meshInicial;

    [Tooltip("Arrastra aquí la malla a la que quieres cambiar después del evento.")]
    public Mesh meshNuevo;

    // El MeshFilter se obtendrá automáticamente del GameObject.
    private MeshFilter meshFilterPared;
    private MeshCollider meshColliderPared; // Para actualizar el collider si existe

    private bool meshCambiado = false;

    void Awake()
    {
        // Obtener el MeshFilter automáticamente del GameObject al que este script está adherido.
        meshFilterPared = GetComponent<MeshFilter>();
        meshColliderPared = GetComponent<MeshCollider>(); // Intentar obtener el MeshCollider

        if (meshFilterPared == null)
        {
            UnityEngine.Debug.LogError("CambiadorMeshPared (" + gameObject.name + "): No se encontró un componente MeshFilter. Este script no funcionará.");
            enabled = false; // Desactivar el script si no hay MeshFilter.
            return;
        }
    }

    void Start()
    {
        // Establecer la malla inicial si está asignada.
        // Si meshInicial no está asignado en el Inspector, se asume que la malla
        // que ya tiene el MeshFilter es la deseada como inicial.
        if (meshInicial != null)
        {
            meshFilterPared.mesh = meshInicial;
            if (meshColliderPared != null)
            {
                meshColliderPared.sharedMesh = meshInicial; // Actualizar también el collider
            }
            UnityEngine.Debug.Log("CambiadorMeshPared (" + gameObject.name + "): Malla inicial '" + meshInicial.name + "' aplicada.");
        }
        else
        {
            if (meshFilterPared.mesh != null)
            {
                // Si no se especifica una malla inicial, guardamos la actual para poder revertir si es necesario.
                meshInicial = meshFilterPared.mesh;
                UnityEngine.Debug.Log("CambiadorMeshPared (" + gameObject.name + "): Usando la malla actual '" + meshFilterPared.mesh.name + "' como inicial y guardada para posible reversión.");
            }
            else
            {
                UnityEngine.Debug.LogWarning("CambiadorMeshPared (" + gameObject.name + "): No se asignó meshInicial y el MeshFilter no tiene una malla por defecto.");
            }
        }
    }

    // Este método público será llamado por otro script para realizar el cambio de malla.
    public void CambiarAMeshNuevo()
    {
        if (meshCambiado)
        {
            UnityEngine.Debug.Log("CambiadorMeshPared (" + gameObject.name + "): La malla ya fue cambiada previamente a la nueva.");
            return;
        }

        if (meshFilterPared == null)
        {
            UnityEngine.Debug.LogError("CambiadorMeshPared (" + gameObject.name + "): MeshFilter no asignado. No se puede cambiar la malla.");
            return;
        }

        if (meshNuevo == null)
        {
            UnityEngine.Debug.LogError("CambiadorMeshPared (" + gameObject.name + "): 'Mesh Nuevo' no asignado en el Inspector. No se puede cambiar la malla.");
            return;
        }

        meshFilterPared.mesh = meshNuevo;
        meshCambiado = true;
        UnityEngine.Debug.Log("CambiadorMeshPared (" + gameObject.name + "): Malla cambiada a '" + meshNuevo.name + "'.");

        // Actualizar el MeshCollider si existe.
        if (meshColliderPared != null)
        {
            meshColliderPared.sharedMesh = meshNuevo;
            UnityEngine.Debug.Log("CambiadorMeshPared (" + gameObject.name + "): MeshCollider actualizado con la nueva malla.");
        }
    }

    // Método opcional para revertir a la malla inicial, si lo necesitas.
    public void RevertirAMeshInicial()
    {
        if (!meshCambiado)
        {
            UnityEngine.Debug.Log("CambiadorMeshPared (" + gameObject.name + "): La malla no ha sido cambiada a la nueva, ya está en el estado inicial (o no se especificó una inicial diferente).");
            return;
        }

        if (meshFilterPared == null)
        {
            UnityEngine.Debug.LogError("CambiadorMeshPared (" + gameObject.name + "): MeshFilter no asignado. No se puede revertir la malla.");
            return;
        }

        if (meshInicial == null)
        {
            // Esto no debería ocurrir si Start() guardó la malla original cuando meshInicial era null.
            UnityEngine.Debug.LogWarning("CambiadorMeshPared (" + gameObject.name + "): No hay una 'Mesh Inicial' definida o guardada a la cual revertir.");
            return;
        }

        meshFilterPared.mesh = meshInicial;
        meshCambiado = false; // Marcar que ahora está en el estado inicial
        UnityEngine.Debug.Log("CambiadorMeshPared (" + gameObject.name + "): Malla revertida a '" + meshInicial.name + "'.");

        if (meshColliderPared != null)
        {
            meshColliderPared.sharedMesh = meshInicial;
            UnityEngine.Debug.Log("CambiadorMeshPared (" + gameObject.name + "): MeshCollider actualizado con la malla inicial.");
        }
    }
}