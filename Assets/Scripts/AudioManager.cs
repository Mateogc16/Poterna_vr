using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instancia;

    public AudioSource efectosSonido;
    public AudioSource musicaFondo;
    public List<AudioClip> listaSonidos; // Lista de sonidos asignados desde el Inspector
    private Dictionary<string, AudioClip> sonidosDict = new Dictionary<string, AudioClip>();

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Llenar el diccionario con los sonidos
        foreach (AudioClip clip in listaSonidos)
        {
            sonidosDict[clip.name] = clip;
        }
    }

    public void ReproducirSonido(string nombre)
    {
        if (sonidosDict.ContainsKey(nombre) && efectosSonido)
        {
            efectosSonido.PlayOneShot(sonidosDict[nombre]);
        }
    }

    public void ReproducirMusica(string nombre, bool enBucle = true)
    {
        if (sonidosDict.ContainsKey(nombre) && musicaFondo)
        {
            musicaFondo.clip = sonidosDict[nombre];
            musicaFondo.loop = enBucle;
            musicaFondo.Play();
        }
    }

    public void DetenerMusica()
    {
        if (musicaFondo)
        {
            musicaFondo.Stop();
        }
    }
}