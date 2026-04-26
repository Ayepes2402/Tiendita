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

    // Ahora le pedimos el índice de la voz que queremos usar
    // Al ponerle "= 0", si un script (como la pantalla final) no le manda número,
    // por defecto usará la voz 0 sin estallar el juego.
    public void ReproducirDialogo(int indiceVoz = 0, float tonoVoz = 1f)
    {
        if (sonidosDialogo.Length == 0) return;

        // El "%" (módulo) es un escudo protector. 
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