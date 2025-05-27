using UnityEngine;
using UnityEngine.UI; 

public class PlayerRespawn : MonoBehaviour
{
    public static Vector3 currentCheckpointPosition;
    private Rigidbody rb;
    private Vector3 startPosition;

    [Header("Muerte y UI")]
    public GameObject canvasMuerte;
    public Button botonRespawn;    
    public string tagTrampa = "Trampa"; 

    private bool estaMuerto = false;

    

    void Awake()
    {
        startPosition = transform.position;
        currentCheckpointPosition = startPosition;
        rb = GetComponent<Rigidbody>();

        if (!CompareTag("Player"))
        {
            UnityEngine.Debug.LogWarning("El objeto Jugador no tiene el tag 'Player'. Los triggers de checkpoint podrían no funcionar.");
        }

        
        if (canvasMuerte != null)
        {
            canvasMuerte.SetActive(false);
        }
        else
        {
            UnityEngine.Debug.LogError("CanvasMuerte no asignado en el script PlayerRespawn.");
        }

        
        if (botonRespawn != null)
        {
            botonRespawn.onClick.AddListener(RespawnDesdeBoton); 
        }
        else
        {
            UnityEngine.Debug.LogError("BotonRespawn no asignado en el script PlayerRespawn.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        
        if (!estaMuerto && collision.gameObject.CompareTag(tagTrampa))
        {
            Morir();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (!estaMuerto && other.gameObject.CompareTag(tagTrampa))
        {
            Morir();
        }
    }

    void Morir()
    {
        if (estaMuerto) return; 

        estaMuerto = true;
        UnityEngine.Debug.Log("El jugador ha muerto.");

      
        if (canvasMuerte != null)
        {
            canvasMuerte.SetActive(true);
        }

    }


    public void RespawnDesdeBoton()
    {
        if (!estaMuerto) return; 

        estaMuerto = false;
        transform.position = currentCheckpointPosition;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

       
        if (canvasMuerte != null)
        {
            canvasMuerte.SetActive(false);
        }

        

        UnityEngine.Debug.Log("Jugador ha respawneado en: " + currentCheckpointPosition + " (desde botón)");
    }

    
    public void RespawnPorCaidaOTecla()
    {
        
        if (estaMuerto && canvasMuerte != null && canvasMuerte.activeSelf) return;

        
        
        if (!estaMuerto)
        {
            transform.position = currentCheckpointPosition;
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            UnityEngine.Debug.Log("Jugador ha respawneado en: " + currentCheckpointPosition + " (por tecla/caída sin canvas)");
        }
        else 
        {
            
            Morir();
        }
    }


    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            
            
            if (estaMuerto && canvasMuerte != null && canvasMuerte.activeSelf)
            {
                
            }
            else if (estaMuerto) 
            {
                Morir(); 
            }
            else 
            {
                transform.position = currentCheckpointPosition; 
                if (rb != null) { rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
                UnityEngine.Debug.Log("Jugador ha respawneado en: " + currentCheckpointPosition + " (por tecla)");
            }
        }

        
        if (transform.position.y < -10f && !estaMuerto) 
        {
            UnityEngine.Debug.Log("Jugador ha caído y muerto.");
            Morir(); 
        }
    }
}