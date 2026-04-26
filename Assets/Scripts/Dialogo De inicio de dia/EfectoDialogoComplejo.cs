using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EfectoDialogo : MonoBehaviour
{
    [Header("Referencias UI - Texto FIJO")]
    public TextMeshProUGUI textoFijo; // Aquí dirá "DÍA X"
    public float velocidadFadeInFijo = 1f;

    [Header("Referencias UI - Texto ANIMADO")]
    public TextMeshProUGUI textoDialogoAnimado; // Aquí se escribe la REGLA
    public GameObject botonFlecha;

    [Header("Botón Nueva Escena")]
    public CanvasGroup grupoBotonEscena;

    [Header("Transición de Escena (Pantalla Negra)")]
    public CanvasGroup pantallaNegra;
    public float velocidadTransicionNegra = 2f;

    [Header("Configuración Máquina de Escribir")]
    public float velocidadEscritura = 0.04f;
    public float velocidadFadeBoton = 1.5f;

    [Header("Sonidos")]
    public AudioSource audioSourceEfectos; // El parlante para el click del botón
    public AudioClip sonidoClickNormal;    // El sonido del click

    private string textoDeLaRegla;
    private bool estaEscribiendo = false;
    private bool textoFijoCompletado = false;
    private Coroutine corrutinaEscritura;

    void Start()
    {
        // --- CONFIGURACIÓN DINÁMICA ---
        if (GameManager.Instance != null)
        {
            // Sumamos 1 porque el GameManager guarda el día que ACABAMOS de terminar
            int proximoDia = GameManager.Instance.dia + 1;

            if (textoFijo != null) textoFijo.text = "DÍA " + proximoDia;

            textoDeLaRegla = ObtenerTextoSegunDia(proximoDia);
        }
        else
        {
            textoDeLaRegla = "Error: No se encontró el GameManager.";
        }

        // --- INICIALIZACIÓN VISUAL ---
        botonFlecha.SetActive(false);
        if (grupoBotonEscena != null) { grupoBotonEscena.alpha = 0; grupoBotonEscena.interactable = false; }
        if (pantallaNegra != null) { pantallaNegra.alpha = 0; }
        textoDialogoAnimado.text = "";

        if (textoFijo != null)
        {
            Color c = textoFijo.color; c.a = 0; textoFijo.color = c;
        }

        StartCoroutine(SecuenciaIntro());
    }

    // --- EL CORAZÓN DE TU IDEA: LAS REGLAS DINÁMICAS ---
    string ObtenerTextoSegunDia(int d)
    {
        switch (d)
        {
            case 1: return "Hoy las vacas se fueron de paseo y no hay leche. No vendas, ¡hay escasez!";
            case 2: return "Las gallinas han entrado en huelga. PROHIBIDO vender huevos hasta nuevo aviso.";
            case 3: return "El trigo se ha quemado. El PAN es ahora un artículo de lujo ilegal.";
            case 4: return "Crisis total: No hay LECHE ni HUEVOS. La policía está vigilando.";
            case 5: return "Último día. El mercado negro de PAN y HUEVOS está en su punto máximo. ¡Cuidado!";
            default: return "Sigue las normas de la tienda.";
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (textoFijoCompletado) AvanzarOSaltar();
        }
    }

    IEnumerator SecuenciaIntro()
    {
        // Fade in de "DÍA X"
        if (textoFijo != null)
        {
            while (textoFijo.color.a < 1)
            {
                Color c = textoFijo.color;
                c.a += Time.deltaTime * velocidadFadeInFijo;
                textoFijo.color = c;
                yield return null;
            }
        }
        textoFijoCompletado = true;

        // Empezar a escribir la regla
        corrutinaEscritura = StartCoroutine(EscribirTexto());
    }

    IEnumerator EscribirTexto()
    {
        estaEscribiendo = true;

        // --- PRENDEMOS EL AUDIO DEL DIÁLOGO ---
        if (AudioManager.instancia != null) AudioManager.instancia.ReproducirDialogo();

        textoDialogoAnimado.text = textoDeLaRegla;
        textoDialogoAnimado.maxVisibleCharacters = 0;

        textoDialogoAnimado.ForceMeshUpdate();
        int total = textoDialogoAnimado.textInfo.characterCount;

        for (int i = 0; i <= total; i++)
        {
            textoDialogoAnimado.maxVisibleCharacters = i;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        // --- APAGAMOS EL AUDIO AL TERMINAR ---
        if (AudioManager.instancia != null) AudioManager.instancia.DetenerDialogo();

        estaEscribiendo = false;
        StartCoroutine(AparecerBotonEscena());
    }

    public void AvanzarOSaltar()
    {
        if (estaEscribiendo)
        {
            StopCoroutine(corrutinaEscritura);

            // --- APAGAMOS EL AUDIO SI EL JUGADOR SALTA LA ANIMACIÓN ---
            if (AudioManager.instancia != null) AudioManager.instancia.DetenerDialogo();

            textoDialogoAnimado.maxVisibleCharacters = textoDialogoAnimado.textInfo.characterCount;
            estaEscribiendo = false;
            StartCoroutine(AparecerBotonEscena());
        }
    }

    IEnumerator AparecerBotonEscena()
    {
        if (grupoBotonEscena != null)
        {
            grupoBotonEscena.gameObject.SetActive(true);
            while (grupoBotonEscena.alpha < 1)
            {
                grupoBotonEscena.alpha += Time.deltaTime * velocidadFadeBoton;
                yield return null;
            }
            grupoBotonEscena.interactable = true;
            grupoBotonEscena.blocksRaycasts = true;
        }
    }

    // Este método lo conectas al OnClick() de tu botón "Continuar"
    public void ClickContinuar()
    {
        // --- SONIDO AL DAR CLIC EN CONTINUAR ---
        if (audioSourceEfectos != null && sonidoClickNormal != null)
        {
            audioSourceEfectos.PlayOneShot(sonidoClickNormal);
        }

        StartCoroutine(TransicionSalida());
    }

    IEnumerator TransicionSalida()
    {
        pantallaNegra.blocksRaycasts = true;
        while (pantallaNegra.alpha < 1)
        {
            pantallaNegra.alpha += Time.deltaTime * velocidadTransicionNegra;
            yield return null;
        }

        // Llamamos al GameManager para que aumente el día y cargue la tienda
        GameManager.Instance.IniciarSiguienteDia();
    }
}