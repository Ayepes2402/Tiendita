using System.Collections;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Referencias UI Generales")]
    public TextMeshProUGUI textoDinero;
    public TextMeshProUGUI textoDia;
    public TextMeshProUGUI textoInventario;
    public TextMeshProUGUI textoEstado;
    public TextMeshProUGUI textoAmonestaciones;
    public TextMeshProUGUI textoMetaDiaria;
    public TextMeshProUGUI textoEventoDelDia;
    public TextMeshProUGUI textoAvisoDia;

    [Header("Paneles de Fin de Juego")]
    public GameObject panelGameOver;
    public TextMeshProUGUI textoGameOver;

    [Header("Ajustes y Menú Pausa")]
    public GameObject panelAjustes;
    public Slider sliderVolumen;

    [Header("Sonidos UI")]
    public AudioSource audioSourceMusica;
    public AudioSource audioSourceEfectos;
    public AudioClip sonidoClick;
    public AudioClip sonidoGanarPlata; // <-- NUEVO: El sonidito de las monedas

    [Header("Referencias del Cliente")]
    public TextMeshProUGUI textoCliente;
    public GameObject burbujaDialogo;
    public Image imagenCliente;
    public Sprite[] spritesClientes;

    [Header("Opciones de Diálogo")]
    public GameObject panelOpciones;
    public TextMeshProUGUI textoBotonOpcion1;
    public TextMeshProUGUI textoBotonOpcion2;

    [Header("Animación de Clientes")]
    public float tiempoEsperaEntrada = 0.4f;
    public float velocidadTransicion = 6f;
    public float distanciaVertical = 600f;

    [Header("Animación de Textos")]
    public float velocidadEscrituraTexto = 0.03f;

    private RectTransform rectCliente;
    private Vector2 posicionOriginalCliente;
    private Coroutine animacionActual;
    private Coroutine corrutinaTextoCliente;
    private Cliente clienteActualUI;

    [Header("Efectos Visuales")]
    public GameObject prefabTextoGanancia;
    public Transform puntoAparicionGanancia;

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

        // --- LÓGICA DE VOLUMEN COMPARTIDO ---
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

    // --- MÉTODOS DE SONIDO Y AJUSTES ---

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

    // ------------------------------------------

    public void MostrarCliente(Cliente cliente)
    {
        clienteActualUI = cliente;
        string textoAColocar = (cliente == null) ? "" : cliente.FrasePedido + "\"";

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

        if (AudioManager.instancia != null)
        {
            AudioManager.instancia.ReproducirDialogo(indiceDeVozDelCliente, tonoDeVoz);
        }

        textoCliente.text = mensaje;
        textoCliente.maxVisibleCharacters = 0;
        textoCliente.ForceMeshUpdate();

        int totalCaracteres = textoCliente.textInfo.characterCount;

        for (int i = 0; i <= totalCaracteres; i++)
        {
            textoCliente.maxVisibleCharacters = i;
            yield return new WaitForSeconds(velocidadEscrituraTexto);
        }

        if (AudioManager.instancia != null)
        {
            AudioManager.instancia.DetenerDialogo();
        }

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
        if (AudioManager.instancia != null) AudioManager.instancia.DetenerDialogo();
        if (corrutinaTextoCliente != null) StopCoroutine(corrutinaTextoCliente);

        corrutinaTextoCliente = StartCoroutine(RutinaEscribirDialogo(clienteActualUI.Respuesta1 + "\"", false));
    }

    public void ClickEnOpcion2()
    {
        ReproducirClick();
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (AudioManager.instancia != null) AudioManager.instancia.DetenerDialogo();
        if (corrutinaTextoCliente != null) StopCoroutine(corrutinaTextoCliente);

        corrutinaTextoCliente = StartCoroutine(RutinaEscribirDialogo(clienteActualUI.Respuesta2 + "\"", false));
    }

    public void MostrarEstado(string mensaje) { if (textoEstado != null) textoEstado.text = mensaje; }
    public void MostrarEventoDelDia(string mensaje) { if (textoEventoDelDia != null) textoEventoDelDia.text = "Evento del día: " + mensaje; }
    public void MostrarAvisoDia(string mensaje) { if (textoAvisoDia != null) textoAvisoDia.text = mensaje; }

    public void MostrarGameOver(string mensaje)
    {
        if (panelGameOver != null) panelGameOver.SetActive(true);
        if (textoGameOver != null) textoGameOver.text = mensaje + "\nPresiona Restart para volver a jugar.";
        MostrarEstado(mensaje);
    }

    public void ActualizarUI()
    {
        if (GameManager.Instance == null) return;

        if (textoDinero != null) textoDinero.text = "" + GameManager.Instance.dinero;
        if (textoDia != null) textoDia.text = "Día: " + GameManager.Instance.dia;
        if (textoAmonestaciones != null) textoAmonestaciones.text = GameManager.Instance.amonestaciones + "/" + GameManager.Instance.maxAmonestaciones;
        if (textoMetaDiaria != null)
        {
            textoMetaDiaria.text = "Meta diaria: $" + GameManager.Instance.ObtenerDineroGanadoEnElDia() + "/$" + GameManager.Instance.ObtenerMetaMinimaDiaria() + " | Clientes: " + GameManager.Instance.ObtenerClientesAtendidosHoy() + "/" + GameManager.Instance.ObtenerClientesPorDia();
        }

        MostrarEstado(GameManager.Instance.ObtenerUltimoMensajeEvento());
        MostrarEventoDelDia(GameManager.Instance.ObtenerEventoDelDiaActual());
        ActualizarInventarioUI();
    }

    private void ActualizarInventarioUI()
    {
        if (textoInventario == null) return;
        Inventario inventario = GameManager.Instance.ObtenerInventario();
        if (inventario == null) { textoInventario.text = "Inventario no disponible"; return; }

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Inventario y Precios de Venta:");
        foreach (Producto producto in inventario.ObtenerProductos())
        {
            builder.AppendLine(producto.Nombre + " | Stock: " + producto.Cantidad + " | Venta: $" + producto.Precio);
        }
        textoInventario.text = builder.ToString();
    }

    public void BotonRechazar()
    {
        ReproducirClick();
        if (GameManager.Instance != null) GameManager.Instance.BotonRechazar();
    }

    public void IntentarVender(string producto)
    {
        // NOTA: Aquí no reproducimos click normal porque ya va a sonar la plata (o el error si falla), 
        // así evitamos que suenen dos cosas al mismo tiempo y se sature.
        if (GameManager.Instance != null) GameManager.Instance.IntentarVender(producto);
    }

    public void BotonRestart()
    {
        ReproducirClick();
        if (GameManager.Instance != null) GameManager.Instance.ReiniciarJuego();
    }

    // --- AQUÍ AÑADIMOS LA MAGIA DE LA PLATA ---
    public void MostrarGanancia(int cantidad)
    {
        // 1. Reproducimos el sonido de corona coronada
        if (audioSourceEfectos != null && sonidoGanarPlata != null)
        {
            audioSourceEfectos.PlayOneShot(sonidoGanarPlata);
        }

        // 2. Mostramos los numeritos verdes volando (tu código original)
        if (prefabTextoGanancia != null && puntoAparicionGanancia != null)
        {
            GameObject nuevoEfecto = Instantiate(prefabTextoGanancia, puntoAparicionGanancia);
            nuevoEfecto.transform.localScale = Vector3.one;

            EfectoGanancia scriptEfecto = nuevoEfecto.GetComponent<EfectoGanancia>();
            if (scriptEfecto != null)
            {
                scriptEfecto.IniciarEfecto(cantidad);
            }
        }
    }
}