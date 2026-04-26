using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instancia;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Sonidos de diálogo")]
    public AudioClip[] sonidosDialogo;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    public void ReproducirDialogo(int indiceVoz = 0, float tonoVoz = 1f)
    {
        if (sonidosDialogo.Length == 0) return;

         
        int indexSeguro = indiceVoz % sonidosDialogo.Length;

        audioSource.clip = sonidosDialogo[indexSeguro];
        audioSource.pitch = tonoVoz;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void DetenerDialogo()
    {
        audioSource.Stop();
    }
}