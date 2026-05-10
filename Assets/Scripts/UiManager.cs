using System.Collections;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Referencias UI Generales")]
    [SerializeField] private TextMeshProUGUI textoDinero;
    [SerializeField] private TextMeshProUGUI textoDia;
    [SerializeField] private TextMeshProUGUI textoInventario;
    [SerializeField] private TextMeshProUGUI textoEstado;
    [SerializeField] private TextMeshProUGUI textoAmonestaciones;
    [SerializeField] private TextMeshProUGUI textoMetaDiaria;
    [SerializeField] private TextMeshProUGUI textoEventoDelDia;
    [SerializeField] private TextMeshProUGUI textoAvisoDia;

    [Header("Paneles de Fin de Juego")]
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private TextMeshProUGUI textoGameOver;

    [Header("Ajustes y Menú Pausa")]
    [SerializeField] private GameObject panelAjustes;
    [SerializeField] private Slider sliderVolumen;

    [Header("Sonidos UI")]
    [SerializeField] private AudioSource audioSourceMusica;
    [SerializeField] private AudioSource audioSourceEfectos;
    [SerializeField] private AudioClip sonidoClick;
    [SerializeField] private AudioClip sonidoGanarPlata;

    [Header("Referencias del Cliente")]
    [SerializeField] private TextMeshProUGUI textoCliente;
    [SerializeField] private GameObject burbujaDialogo;
    [SerializeField] private Image imagenCliente;
    [SerializeField] private Sprite[] spritesClientes;

    [Header("Opciones de Diálogo")]
    [SerializeField] private GameObject panelOpciones;
    [SerializeField] private TextMeshProUGUI textoBotonOpcion1;
    [SerializeField] private TextMeshProUGUI textoBotonOpcion2;

    [Header("Animación de Clientes")]
    [SerializeField] private float tiempoEsperaEntrada = 0.4f;
    [SerializeField] private float velocidadTransicion = 6f;
    [SerializeField] private float distanciaVertical = 600f;

    [Header("Animación de Textos")]
    [SerializeField] private float velocidadEscrituraTexto = 0.03f;

    private RectTransform rectCliente;
    private Vector2 posicionOriginalCliente;
    private Coroutine animacionActual;
    private Coroutine corrutinaTextoCliente;

    private ClienteData clienteActualUI;

    [Header("Efectos Visuales")]
    [SerializeField] private GameObject prefabTextoGanancia;
    [SerializeField] private Transform puntoAparicionGanancia;

    [Header("Referencias de la Repisa")]
    [SerializeField] private RepisaHover scriptRepisa;

    
    private void OnEnable()
    {
       
        GameManager.OnDatosActualizados += ActualizarUI;
    }

    private void OnDisable()
    {
        
        GameManager.OnDatosActualizados -= ActualizarUI;
    }

    void Awake()
    {
        if (imagenCliente != null)
        {
            rectCliente = imagenCliente.GetComponent<RectTransform>();
            posicionOriginalCliente = rectCliente.anchoredPosition;
        }
    }

    void Start()
    {
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (panelAjustes != null) panelAjustes.SetActive(false);

        if (imagenCliente != null) imagenCliente.enabled = false;
        if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
        if (textoCliente != null) textoCliente.text = "";

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

        ActualizarUI();
    }

    public void ReproducirClick()
    {
        if (audioSourceEfectos != null && sonidoClick != null)
        {
            audioSourceEfectos.PlayOneShot(sonidoClick);
        }
    }

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

    public void CambiarVolumen(float volumen)
    {
        AudioListener.volume = volumen;
        PlayerPrefs.SetFloat("VolumenJuego", volumen);
    }

    public void MostrarCliente(ClienteData cliente, string productoReal = "")
    {
        clienteActualUI = cliente;
        string frase = cliente != null ? cliente.FrasePedido : "";

        if (!string.IsNullOrEmpty(productoReal) && cliente != null)
        {
            frase = frase.Replace("[producto]", productoReal);
        }

        if (scriptRepisa != null) scriptRepisa.enabled = false;

        string textoAColocar = (cliente == null) ? "" : frase + "\"";

        if (AudioManager.instancia != null) AudioManager.instancia.DetenerDialogo();
        if (corrutinaTextoCliente != null) StopCoroutine(corrutinaTextoCliente);
        if (textoCliente != null) textoCliente.text = "";

        if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);

        if (imagenCliente != null)
        {
            if (animacionActual != null) StopCoroutine(animacionActual);

            if (cliente != null && spritesClientes != null &&
                cliente.SpriteIndex >= 0 && cliente.SpriteIndex < spritesClientes.Length)
            {
                Sprite nuevoSprite = spritesClientes[cliente.SpriteIndex];
                animacionActual = StartCoroutine(RutinaCambioCliente(nuevoSprite, textoAColocar));
            }
            else
            {
                animacionActual = StartCoroutine(RutinaSalidaCliente());
            }
        }
    }

    IEnumerator RutinaCambioCliente(Sprite nuevoSprite, string textoParaEscribir)
    {
        if (rectCliente == null) yield break;
        Vector2 posicionOculta = posicionOriginalCliente + new Vector2(0, -distanciaVertical);

        if (imagenCliente.enabled)
        {
            while (Vector2.Distance(rectCliente.anchoredPosition, posicionOculta) > 1f)
            {
                rectCliente.anchoredPosition = Vector2.Lerp(rectCliente.anchoredPosition, posicionOculta, Time.deltaTime * velocidadTransicion);
                yield return null;
            }
        }

        yield return new WaitForSeconds(tiempoEsperaEntrada);
        imagenCliente.sprite = nuevoSprite;
        imagenCliente.enabled = true;
        rectCliente.anchoredPosition = posicionOculta;

        while (Vector2.Distance(rectCliente.anchoredPosition, posicionOriginalCliente) > 1f)
        {
            rectCliente.anchoredPosition = Vector2.Lerp(rectCliente.anchoredPosition, posicionOriginalCliente, Time.deltaTime * velocidadTransicion);
            yield return null;
        }

        rectCliente.anchoredPosition = posicionOriginalCliente;

        if (burbujaDialogo != null && !string.IsNullOrEmpty(textoParaEscribir))
        {
            burbujaDialogo.SetActive(true);
            if (textoCliente != null)
            {
                corrutinaTextoCliente = StartCoroutine(RutinaEscribirDialogo(textoParaEscribir, true));
            }
        }
    }

    IEnumerator RutinaSalidaCliente()
    {
        if (rectCliente == null) yield break;
        if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);

        Vector2 posicionOculta = posicionOriginalCliente + new Vector2(0, -distanciaVertical);
        while (Vector2.Distance(rectCliente.anchoredPosition, posicionOculta) > 1f)
        {
            rectCliente.anchoredPosition = Vector2.Lerp(rectCliente.anchoredPosition, posicionOculta, Time.deltaTime * velocidadTransicion);
            yield return null;
        }
        imagenCliente.enabled = false;
    }

    
    IEnumerator RutinaEscribirDialogo(string mensaje, bool mostrarOpcionesAlFinal)
    {
        float tonoDeVoz = 1f;
        int indiceDeVozDelCliente = 0;

        if (clienteActualUI != null)
        {
            tonoDeVoz = 0.8f + (clienteActualUI.Nombre.Length * 0.05f);
            indiceDeVozDelCliente = clienteActualUI.SpriteIndex;
        }

        textoCliente.text = mensaje;
        textoCliente.maxVisibleCharacters = 0;
        textoCliente.ForceMeshUpdate();

        int totalCaracteres = textoCliente.textInfo.characterCount;

        for (int i = 0; i <= totalCaracteres; i++)
        {
            textoCliente.maxVisibleCharacters = i;

          
            if (i > 0 && i <= mensaje.Length && mensaje[i - 1] != ' ')
            {
                if (AudioManager.instancia != null)
                    AudioManager.instancia.ReproducirDialogo(indiceDeVozDelCliente, tonoDeVoz);
            }

            yield return new WaitForSeconds(velocidadEscrituraTexto);
        }

        if (AudioManager.instancia != null) AudioManager.instancia.DetenerDialogo();

        if (mostrarOpcionesAlFinal && panelOpciones != null && clienteActualUI != null)
        {
            if (textoBotonOpcion1 != null) textoBotonOpcion1.text = clienteActualUI.Opcion1;
            if (textoBotonOpcion2 != null) textoBotonOpcion2.text = clienteActualUI.Opcion2;
            panelOpciones.SetActive(true);
        }
    }


    public void ClickEnOpcion1()
    {
        ReproducirClick();
        if (panelOpciones != null) panelOpciones.SetActive(false);

        if (scriptRepisa != null) scriptRepisa.enabled = true;

        if (corrutinaTextoCliente != null) StopCoroutine(corrutinaTextoCliente);

        
        if (AudioManager.instancia != null) AudioManager.instancia.DetenerDialogo();

        corrutinaTextoCliente = StartCoroutine(RutinaEscribirDialogo(clienteActualUI.Respuesta1 + "\"", false));
    }

    public void ClickEnOpcion2()
    {
        ReproducirClick();
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (AudioManager.instancia != null) AudioManager.instancia.DetenerDialogo();
        if (corrutinaTextoCliente != null) StopCoroutine(corrutinaTextoCliente);

        StartCoroutine(RutinaRechazarCliente());
    }


    private IEnumerator RutinaRechazarCliente()
    {
        
        yield return StartCoroutine(RutinaEscribirDialogo(clienteActualUI.Respuesta2 + "\"", false));

        yield return new WaitForSeconds(1f);

       
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BotonRechazar();
        }
    }

    public void ActualizarUI()
    {
        if (GameManager.Instance == null) return;

        if (textoDinero != null) textoDinero.text = "" + GameManager.Instance.Dinero;
        if (textoDia != null) textoDia.text = "Día: " + GameManager.Instance.Dia;
        if (textoAmonestaciones != null) textoAmonestaciones.text = GameManager.Instance.Amonestaciones + "/" + GameManager.Instance.MaxAmonestaciones;

        if (textoMetaDiaria != null)
        {
            
            textoMetaDiaria.text = "Meta diaria: $" + GameManager.Instance.DineroGanadoEnElDia + "/$" + GameManager.Instance.ObtenerMetaMinimaDiaria() + " | Clientes: " + GameManager.Instance.ObtenerClientesAtendidosHoy() + "/" + GameManager.Instance.ObtenerClientesPorDia();
        }

        MostrarEstado(GameManager.Instance.ObtenerUltimoMensajeEvento());
        MostrarEventoDelDia(GameManager.Instance.ObtenerEventoDelDiaActual());
        ActualizarInventarioUI();
    }

    private void ActualizarInventarioUI()
    {
        if (textoInventario == null) return;
        Inventario inventario = GameManager.Instance.ObtenerInventario();
        if (inventario == null) return;

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Inventario:");
        foreach (Producto producto in inventario.ObtenerProductos())
        {
            builder.AppendLine(producto.Nombre + " | Stock: " + producto.Cantidad + " | $" + producto.Precio);
        }
        textoInventario.text = builder.ToString();
    }

    public void MostrarEstado(string mensaje) { if (textoEstado != null) textoEstado.text = mensaje; }
    public void MostrarEventoDelDia(string mensaje) { if (textoEventoDelDia != null) textoEventoDelDia.text = "Evento: " + mensaje; }
    public void MostrarAvisoDia(string mensaje) { if (textoAvisoDia != null) textoAvisoDia.text = mensaje; }

    public void BotonRestart()
    {
        ReproducirClick();
        if (GameManager.Instance != null) GameManager.Instance.ReiniciarJuego();
    }

    public void MostrarGanancia(int cantidad)
    {
        if (audioSourceEfectos != null && sonidoGanarPlata != null) audioSourceEfectos.PlayOneShot(sonidoGanarPlata);
        if (prefabTextoGanancia != null && puntoAparicionGanancia != null)
        {
            GameObject nuevoEfecto = Instantiate(prefabTextoGanancia, puntoAparicionGanancia);
            nuevoEfecto.GetComponent<EfectoGanancia>()?.IniciarEfecto(cantidad);
        }
    }
}