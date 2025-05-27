using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public Transform spawnPointOverride;
    public string playerTag = "Player";
    private bool isActivatedThisSession = false;
    public Color activatedColor = Color.green;
    private Renderer objectRenderer;
    private Color originalColor;

    void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
        {
            UnityEngine.Debug.LogError("CheckpointTrigger en '" + gameObject.name + "' necesita un componente Collider con la opción 'Is Trigger' marcada.", gameObject);
        }

        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Vector3 newCheckpointPos = spawnPointOverride != null ? spawnPointOverride.position : transform.position;
            if (PlayerRespawn.currentCheckpointPosition != newCheckpointPos || !isActivatedThisSession)
            {
                PlayerRespawn.currentCheckpointPosition = newCheckpointPos;
                UnityEngine.Debug.Log("Checkpoint activado en: " + newCheckpointPos + " por " + gameObject.name);
                if (!isActivatedThisSession && objectRenderer != null)
                {
                    objectRenderer.material.color = activatedColor;
                }
                isActivatedThisSession = true;
            }
        }
    }
}