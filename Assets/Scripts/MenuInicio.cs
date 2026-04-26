using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuInicio : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelAjustes;

    [Header("Ajustes de Audio")]
    public Slider sliderVolumen;

    [Header("Sonidos")]
    public AudioSource audioSourceMusica;
    public AudioSource audioSourceEfectos;
    public AudioClip sonidoClick;

    private void Start()
    {
        // --- SEGURO DE REINICIO ---
        // Si venimos de una partida a medias, matamos al GameManager viejo para empezar limpios
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
            GameManager.Instance = null;
        }

        // --- LO QUE YA TENÍAS ---
        if (panelAjustes != null) panelAjustes.SetActive(false);

        if (sliderVolumen != null)
        {
            float volumenGuardado = PlayerPrefs.GetFloat("VolumenJuego", 0.75f);
            sliderVolumen.value = volumenGuardado;
            AudioListener.volume = volumenGuardado;
            sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
        }

        if (audioSourceMusica != null && !audioSourceMusica.isPlaying)
        {
            audioSourceMusica.Play();
        }
    }

    public void ReproducirClick()
    {
        if (audioSourceEfectos != null && sonidoClick != null)
        {
            audioSourceEfectos.PlayOneShot(sonidoClick);
        }
    }

    // --- BOTONES CON SONIDO ---

    public void BotonJugar()
    {
        ReproducirClick();
        // Esperamos un pelín para que el sonido se alcance a escuchar antes del cambio de escena
        Invoke("CargarLore", 0.15f);
    }

    private void CargarLore() => SceneManager.LoadScene("EscenaLore");

    public void AbrirAjustes()
    {
        ReproducirClick();
        if (panelAjustes != null) panelAjustes.SetActive(true);
    }

    public void CerrarAjustes()
    {
        ReproducirClick();
        if (panelAjustes != null) panelAjustes.SetActive(false);
    }

    public void BotonSalir()
    {
        ReproducirClick();
        Application.Quit();
    }

    public void CambiarVolumen(float volumen)
    {
        AudioListener.volume = volumen;
        PlayerPrefs.SetFloat("VolumenJuego", volumen);
    }
}