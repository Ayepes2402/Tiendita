using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EfectoFinDeDia : MonoBehaviour
{
    [Header("Referencias UI - Texto FIJO")]
    public TextMeshProUGUI textoTituloResumen;
    public float velocidadFadeInFijo = 1f;

    [Header("Referencias UI - Texto ANIMADO")]
    public TextMeshProUGUI textoResumenAnimado;
    public float velocidadEscritura = 0.04f;
    private string textoDelResumen;
    private bool estaEscribiendo = false;
    private bool tituloCompletado = false;
    private Coroutine corrutinaEscritura;

    [Header("UI de Tienda (Aparece al final)")]
    public CanvasGroup grupoTiendaYBotones;
    public float velocidadFadeBotones = 1.5f;

    [Header("Textos Dinámicos de la Tienda")]
    public TextMeshProUGUI textoSaldoTotal;
    public TextMeshProUGUI infoPan;
    public TextMeshProUGUI infoLeche;
    public TextMeshProUGUI infoHuevos;

    [Header("Transición de Escena (Pantalla Negra)")]
    public CanvasGroup pantallaNegra;
    public float velocidadTransicionNegra = 2f;

    [Header("Sonidos")]
    public AudioSource audioSourceEfectos; 
    public AudioClip sonidoComprar;        
    public AudioClip sonidoError;          
    public AudioClip sonidoClickNormal;    

    private Color colorOriginalTexto = Color.white;

    void Start()
    {
        if (infoPan != null) colorOriginalTexto = infoPan.color;

        if (GameManager.Instance != null)
        {
            if (textoTituloResumen != null)
                textoTituloResumen.text = "RESUMEN DEL DIA " + GameManager.Instance.dia;

            textoDelResumen = "Dinero ganado hoy: $" + GameManager.Instance.ObtenerDineroGanadoEnElDia() +
                              "\n\nFaltas del gobierno: " + GameManager.Instance.amonestaciones + " / " + GameManager.Instance.maxAmonestaciones;

            ActualizarTextosTienda();
        }
        else
        {
            textoDelResumen = "Error: No se encontró el GameManager.";
        }

        if (grupoTiendaYBotones != null)
        {
            grupoTiendaYBotones.alpha = 0;
            grupoTiendaYBotones.interactable = false;
            grupoTiendaYBotones.blocksRaycasts = false;
        }

        if (pantallaNegra != null) { pantallaNegra.alpha = 0; pantallaNegra.blocksRaycasts = false; }

        textoResumenAnimado.text = "";

        if (textoTituloResumen != null)
        {
            Color c = textoTituloResumen.color; c.a = 0; textoTituloResumen.color = c;
        }

        StartCoroutine(SecuenciaIntro());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (tituloCompletado && estaEscribiendo) AvanzarOSaltar();
        }
    }

    IEnumerator SecuenciaIntro()
    {
        if (textoTituloResumen != null)
        {
            while (textoTituloResumen.color.a < 1)
            {
                Color c = textoTituloResumen.color;
                c.a += Time.deltaTime * velocidadFadeInFijo;
                textoTituloResumen.color = c;
                yield return null;
            }
        }
        tituloCompletado = true;
        corrutinaEscritura = StartCoroutine(EscribirTexto());
    }

    IEnumerator EscribirTexto()
    {
        estaEscribiendo = true;

      
        if (AudioManager.instancia != null) AudioManager.instancia.ReproducirDialogo();

        textoResumenAnimado.text = textoDelResumen;
        textoResumenAnimado.maxVisibleCharacters = 0;
        textoResumenAnimado.ForceMeshUpdate();

        int total = textoResumenAnimado.textInfo.characterCount;

        for (int i = 0; i <= total; i++)
        {
            textoResumenAnimado.maxVisibleCharacters = i;
            yield return new WaitForSeconds(velocidadEscritura);
        }

       
        if (AudioManager.instancia != null) AudioManager.instancia.DetenerDialogo();

        estaEscribiendo = false;
        StartCoroutine(AparecerTienda());
    }

    public void AvanzarOSaltar()
    {
        if (estaEscribiendo)
        {
            StopCoroutine(corrutinaEscritura);

            
            if (AudioManager.instancia != null) AudioManager.instancia.DetenerDialogo();

            textoResumenAnimado.maxVisibleCharacters = textoResumenAnimado.textInfo.characterCount;
            estaEscribiendo = false;
            StartCoroutine(AparecerTienda());
        }
    }

    IEnumerator AparecerTienda()
    {
        if (grupoTiendaYBotones != null)
        {
            grupoTiendaYBotones.gameObject.SetActive(true);
            while (grupoTiendaYBotones.alpha < 1)
            {
                grupoTiendaYBotones.alpha += Time.deltaTime * velocidadFadeBotones;
                yield return null;
            }
            grupoTiendaYBotones.interactable = true;
            grupoTiendaYBotones.blocksRaycasts = true;
        }
    }

    private void ActualizarTextosTienda()
    {
        if (GameManager.Instance == null) return;

        if (textoSaldoTotal != null)
            textoSaldoTotal.text = "TU SALDO ACTUAL: $" + GameManager.Instance.dinero;

        Inventario inv = GameManager.Instance.ObtenerInventario();
        if (inv != null)
        {
            if (infoPan != null)
                infoPan.text = "Stock de pan: " + inv.ObtenerProducto("pan").Cantidad + " | Precio: $" + GameManager.Instance.costoCompraPan;

            if (infoLeche != null)
                infoLeche.text = "Stock de leche: " + inv.ObtenerProducto("leche").Cantidad + " | Precio: $" + GameManager.Instance.costoCompraLeche;

            if (infoHuevos != null)
                infoHuevos.text = "Stock de huevos: " + inv.ObtenerProducto("huevos").Cantidad + " | Precio: $" + GameManager.Instance.costoCompraHuevos;
        }
    }

   
    public void ComprarPan()
    {
        if (GameManager.Instance.ComprarArticulo("pan", GameManager.Instance.costoCompraPan))
        {
            if (audioSourceEfectos != null && sonidoComprar != null) audioSourceEfectos.PlayOneShot(sonidoComprar);
            ActualizarTextosTienda();
            if (infoPan != null) StartCoroutine(EfectoFeedbackCompra(infoPan));
        }
        else
        {
            if (audioSourceEfectos != null && sonidoError != null) audioSourceEfectos.PlayOneShot(sonidoError);
        }
    }

    public void ComprarLeche()
    {
        if (GameManager.Instance.ComprarArticulo("leche", GameManager.Instance.costoCompraLeche))
        {
            if (audioSourceEfectos != null && sonidoComprar != null) audioSourceEfectos.PlayOneShot(sonidoComprar);
            ActualizarTextosTienda();
            if (infoLeche != null) StartCoroutine(EfectoFeedbackCompra(infoLeche));
        }
        else
        {
            if (audioSourceEfectos != null && sonidoError != null) audioSourceEfectos.PlayOneShot(sonidoError);
        }
    }

    public void ComprarHuevos()
    {
        if (GameManager.Instance.ComprarArticulo("huevos", GameManager.Instance.costoCompraHuevos))
        {
            if (audioSourceEfectos != null && sonidoComprar != null) audioSourceEfectos.PlayOneShot(sonidoComprar);
            ActualizarTextosTienda();
            if (infoHuevos != null) StartCoroutine(EfectoFeedbackCompra(infoHuevos));
        }
        else
        {
            if (audioSourceEfectos != null && sonidoError != null) audioSourceEfectos.PlayOneShot(sonidoError);
        }
    }

    IEnumerator EfectoFeedbackCompra(TextMeshProUGUI textoAnimar)
    {
        textoAnimar.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        textoAnimar.color = Color.green;

        float duracionAnimacion = 0.3f;
        float tiempoPasado = 0f;

        while (tiempoPasado < duracionAnimacion)
        {
            tiempoPasado += Time.deltaTime;
            float porcentajeCompletado = tiempoPasado / duracionAnimacion;

            textoAnimar.transform.localScale = Vector3.Lerp(new Vector3(1.2f, 1.2f, 1.2f), Vector3.one, porcentajeCompletado);
            textoAnimar.color = Color.Lerp(Color.green, colorOriginalTexto, porcentajeCompletado);

            yield return null;
        }

        textoAnimar.transform.localScale = Vector3.one;
        textoAnimar.color = colorOriginalTexto;
    }

    public void ClickContinuar()
    {
        if (audioSourceEfectos != null && sonidoClickNormal != null) audioSourceEfectos.PlayOneShot(sonidoClickNormal);
        StartCoroutine(TransicionSalida());
    }

    IEnumerator TransicionSalida()
    {
        if (grupoTiendaYBotones != null)
        {
            grupoTiendaYBotones.interactable = false;
            grupoTiendaYBotones.blocksRaycasts = false;
        }

        float alphaActual = 1f;
        while (alphaActual > 0)
        {
            alphaActual -= Time.deltaTime * 2f;

            if (grupoTiendaYBotones != null)
                grupoTiendaYBotones.alpha = alphaActual;

            if (textoTituloResumen != null)
                textoTituloResumen.color = new Color(textoTituloResumen.color.r, textoTituloResumen.color.g, textoTituloResumen.color.b, alphaActual);

            if (textoResumenAnimado != null)
                textoResumenAnimado.color = new Color(textoResumenAnimado.color.r, textoResumenAnimado.color.g, textoResumenAnimado.color.b, alphaActual);

            yield return null;
        }

        if (pantallaNegra != null)
        {
            pantallaNegra.blocksRaycasts = true;
            while (pantallaNegra.alpha < 1)
            {
                pantallaNegra.alpha += Time.deltaTime * velocidadTransicionNegra;
                yield return null;
            }
            yield return new WaitForSeconds(0.3f);
        }

        SceneManager.LoadScene("Escena_EntreDias");
    }
}