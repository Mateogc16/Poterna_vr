using UnityEngine;
// using System.Collections.Generic; // No es estrictamente necesario para esta versión simplificada
// Si tienes "using System.Net.Mime;" o "using System.Net;", podrías quitarlos si no los necesitas,
// o mantenerlos y usar la corrección de abajo.

public class AudioManager : MonoBehaviour
{
    [Header("----- Clips de Sonidos Continuos -----")]
    public AudioClip latidoCorazonClip;
    public AudioClip ambientacion1Clip;
    public AudioClip ambientacion2Clip;
    public AudioClip musicaPersecucionClip; // Música que puede ser continua

    [Header("----- Clips de Efectos de Sonido (SFX) -----")]
    // SFX Originales
    public AudioClip abrirPuertaClip;
    public AudioClip cerrarPuertaClip;
    public AudioClip recogerObjetoClip;

    // Nuevos SFX Solicitados
    public AudioClip papelClip;
    public AudioClip gritoEstatuaClip;
    public AudioClip sonidoMuerteClip;
    public AudioClip ruidosCarelibroClip;
    public AudioClip sonidoBolaEspinaClip;
    public AudioClip sonidoFinalBuenoClip;

    [Header("----- Audio Sources -----")]
    [Tooltip("AudioSource para el latido del corazón (loop)")]
    public AudioSource latidoCorazonSource;
    [Tooltip("AudioSource para la primera ambientación (loop)")]
    public AudioSource ambientacion1Source;
    [Tooltip("AudioSource para la segunda ambientación (loop)")]
    public AudioSource ambientacion2Source;
    [Tooltip("AudioSource para la música de persecución (loop opcional)")]
    public AudioSource musicaPersecucionSource;
    [Tooltip("AudioSource principal para reproducir efectos de sonido (one-shot)")]
    public AudioSource efectosSonidoSource;
    [Tooltip("AudioSource secundario para SFX (opcional, para más concurrencia o tipos)")]
    public AudioSource efectosSonidoSource2;


    public static AudioManager instance;

    void Awake()
    {
        // Configuración del Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetupContinuousSounds();
        SetupSFXSources();
    }

    void SetupContinuousSounds()
    {
        ConfigureAudioSource(latidoCorazonSource, latidoCorazonClip, true, true, 0.8f, "Latido Corazón Source");
        ConfigureAudioSource(ambientacion1Source, ambientacion1Clip, true, true, 0.6f, "Ambientación 1 Source");
        ConfigureAudioSource(ambientacion2Source, ambientacion2Clip, true, true, 0.5f, "Ambientación 2 Source");
        ConfigureAudioSource(musicaPersecucionSource, musicaPersecucionClip, true, false, 0.7f, "Música Persecución Source"); // playOnAwake = false, se controla por script
    }

    void SetupSFXSources()
    {
        efectosSonidoSource = EnsureAudioSourceExists(efectosSonidoSource, "Efectos Sonido Principal Source");
        if (efectosSonidoSource != null) efectosSonidoSource.playOnAwake = false;

        efectosSonidoSource2 = EnsureAudioSourceExists(efectosSonidoSource2, "Efectos Sonido Secundario Source", true); // true para opcional
        if (efectosSonidoSource2 != null) efectosSonidoSource2.playOnAwake = false;
    }

    // Método ayudante para configurar AudioSources continuos
    void ConfigureAudioSource(AudioSource source, AudioClip clip, bool loop, bool playOnAwake, float volume, string sourceNameForDebug)
    {
        if (source == null)
        {
            if (clip != null)
            {
                UnityEngine.Debug.LogWarning($"AudioManager: {sourceNameForDebug} no asignado en el Inspector. Intentando crear uno nuevo en este GameObject.");
                source = gameObject.AddComponent<AudioSource>();
                if (sourceNameForDebug.Contains("Latido")) this.latidoCorazonSource = source;
                else if (sourceNameForDebug.Contains("Ambientación 1")) this.ambientacion1Source = source;
                else if (sourceNameForDebug.Contains("Ambientación 2")) this.ambientacion2Source = source;
                else if (sourceNameForDebug.Contains("Persecución")) this.musicaPersecucionSource = source;

            }
            else
            {
                return;
            }
        }

        if (clip != null)
        {
            source.clip = clip;
            source.loop = loop;
            source.playOnAwake = playOnAwake;
            source.volume = volume;
            // AQUÍ ESTÁ LA CORRECCIÓN:
            if (playOnAwake && UnityEngine.Application.isPlaying && !source.isPlaying)
            {
                source.Play();
            }
        }
        else if (source != null)
        {
            // UnityEngine.Debug.Log($"AudioManager: {sourceNameForDebug} asignado pero sin AudioClip.");
        }
    }

    // Método ayudante para asegurar que un AudioSource para SFX exista
    AudioSource EnsureAudioSourceExists(AudioSource source, string sourceNameForDebug, bool isOptional = false)
    {
        if (source == null)
        {
            if (!isOptional)
            {
                UnityEngine.Debug.LogWarning($"AudioManager: {sourceNameForDebug} no asignado en el Inspector. Creando uno nuevo para este AudioManager. Considera asignarlo manually para mejor control.");
                source = gameObject.AddComponent<AudioSource>();
                if (sourceNameForDebug.Contains("Principal")) this.efectosSonidoSource = source;
            }
            else
            {
                return null;
            }
        }
        return source;
    }


    // --- Métodos Públicos para reproducir SFX ---

    private void PlaySFX(AudioClip clip, AudioSource specificSource = null)
    {
        AudioSource sourceToUse = specificSource != null ? specificSource : efectosSonidoSource;

        if (sourceToUse != null && clip != null)
        {
            sourceToUse.PlayOneShot(clip);
        }
        else
        {
            if (sourceToUse == null) UnityEngine.Debug.LogWarning("AudioManager: No se puede reproducir SFX. El AudioSource designado no está asignado o disponible (efectosSonidoSource o el específico).");
            if (clip == null) UnityEngine.Debug.LogWarning("AudioManager: No se puede reproducir SFX. El AudioClip no está asignado (variable pública de clip vacía en el Inspector para el método llamado).");
        }
    }

    // SFX Originales
    public void PlayAbrirPuerta() => PlaySFX(abrirPuertaClip);
    public void PlayCerrarPuerta() => PlaySFX(cerrarPuertaClip);
    public void PlayRecogerObjeto() => PlaySFX(recogerObjetoClip);

    // Nuevos SFX Solicitados
    public void PlayPapel() => PlaySFX(papelClip);
    public void PlayGritoEstatua() => PlaySFX(gritoEstatuaClip);
    public void PlaySonidoMuerte() => PlaySFX(sonidoMuerteClip);
    public void PlayRuidosCarelibro() => PlaySFX(ruidosCarelibroClip);
    public void PlaySonidoBolaEspina() => PlaySFX(sonidoBolaEspinaClip);
    public void PlaySonidoFinalBueno() => PlaySFX(sonidoFinalBuenoClip);


    // --- Métodos de control para Sonidos Continuos ---
    public void IniciarMusicaPersecucion()
    {
        if (musicaPersecucionSource != null && musicaPersecucionClip != null && !musicaPersecucionSource.isPlaying)
        {
            musicaPersecucionSource.Play();
        }
    }

    public void DetenerMusicaPersecucion()
    {
        if (musicaPersecucionSource != null && musicaPersecucionSource.isPlaying)
        {
            musicaPersecucionSource.Stop();
        }
    }

    public void IniciarAmbientacion2()
    {
        if (ambientacion2Source != null && ambientacion2Clip != null && !ambientacion2Source.isPlaying)
        {
            ambientacion2Source.Play();
        }
    }

    public void DetenerAmbientacion2()
    {
        if (ambientacion2Source != null && ambientacion2Source.isPlaying)
        {
            ambientacion2Source.Stop();
        }
    }

    // --- Métodos de Control de Volumen ---
    public void SetLatidoVolume(float volume)
    {
        if (latidoCorazonSource != null)
        {
            latidoCorazonSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void SetMusicaPersecucionVolume(float volume)
    {
        if (musicaPersecucionSource != null)
        {
            musicaPersecucionSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void SetVolumenGeneralSFX(float volume)
    {
        if (efectosSonidoSource != null)
        {
            efectosSonidoSource.volume = Mathf.Clamp01(volume);
        }
        if (efectosSonidoSource2 != null) // También al secundario si existe
        {
            efectosSonidoSource2.volume = Mathf.Clamp01(volume);
        }
    }
}
