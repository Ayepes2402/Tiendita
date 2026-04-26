using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EfectoDialogo : MonoBehaviour
{
    [Header("Referencias UI - Texto FIJO")]
    public TextMeshProUGUI textoFijo; 
    public float velocidadFadeInFijo = 1f;

    [Header("Referencias UI - Texto ANIMADO")]
    public TextMeshProUGUI textoDialogoAnimado; 
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
    public AudioSource audioSourceEfectos; 
    public AudioClip sonidoClickNormal;    

    private string textoDeLaRegla;
    private bool estaEscribiendo = false;
    private bool textoFijoCompletado = false;
    private Coroutine corrutinaEscritura;

    void Start()
    {
       
        if (GameManager.Instance != null)
        {
            
            int proximoDia = GameManager.Instance.dia + 1;

            if (textoFijo != null) textoFijo.text = "DIA " + proximoDia;

            textoDeLaRegla = ObtenerTextoSegunDia(proximoDia);
        }
        else
        {
            textoDeLaRegla = "Error: No se encontró el GameManager.";
        }

        
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

       
        corrutinaEscritura = StartCoroutine(EscribirTexto());
    }

    IEnumerator EscribirTexto()
    {
        estaEscribiendo = true;

        
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

        
        if (AudioManager.instancia != null) AudioManager.instancia.DetenerDialogo();

        estaEscribiendo = false;
        StartCoroutine(AparecerBotonEscena());
    }

    public void AvanzarOSaltar()
    {
        if (estaEscribiendo)
        {
            StopCoroutine(corrutinaEscritura);

            
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

    
    public void ClickContinuar()
    {
        
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

        
        GameManager.Instance.IniciarSiguienteDia();
    }
}