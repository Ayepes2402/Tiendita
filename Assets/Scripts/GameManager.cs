using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // --- SINGLETON ---
    public static GameManager Instance;

    private Cliente clienteActual;
    private ClienteGenerator generador;
    private UIManager uiManager;
    private Inventario inventario;
    private VentaService ventaService;
    private ReglaGobierno reglaDelDia;

    private List<Cliente> clientesDelDia;
    private int indiceClienteActualDelDia;
    private int clientesAtendidosHoy;
    private string eventoDelDiaActual = "";


    [System.NonSerialized] public int dinero = 0;
    [System.NonSerialized] public int deuda = 6500;
    [System.NonSerialized] public int dia = 0;
    [System.NonSerialized] public int diasMaximos = 5;

    [System.NonSerialized] public int dineroInicial = 1500;
    [System.NonSerialized] public int amonestaciones = 0;
    [System.NonSerialized] public int maxAmonestaciones = 3;
    [System.NonSerialized] public int clientesPorDia = 11;
    [System.NonSerialized] public int metaMinimaDiaria = 20;

    [System.NonSerialized] public int costoCompraPan = 300;
    [System.NonSerialized] public int costoCompraLeche = 500;
    [System.NonSerialized] public int costoCompraHuevos = 400;

    private bool juegoTerminado = false;
    private string ultimoMensajeEvento = "";
    private int dineroGanadoEnElDia = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

           
            InicializarComponentesBase();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void InicializarComponentesBase()
    {
        diasMaximos = 5;

        inventario = new Inventario(new List<Producto> {
            new Producto("pan", 600, 3, CategoriaProducto.Basico),
            new Producto("leche", 800, 0, CategoriaProducto.Lacteo),
            new Producto("huevos", 1000, 1, CategoriaProducto.Proteina)
        });

        ventaService = new VentaService(inventario);
        generador = new ClienteGenerator();

        dinero = dineroInicial;
        dia = 0; 
        juegoTerminado = false;
    }

   
    void OnEnable()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded += AlCargarEscena;
        }
    }

    void OnDisable()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= AlCargarEscena;
        }
    }

    private void AlCargarEscena(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Escena_Tienda")
        {
            uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                PrepararDia(dia);
            }
        }
    }

    private void PrepararDia(int nuevoDia)
    {
        dia = nuevoDia;
        clientesAtendidosHoy = 0;
        indiceClienteActualDelDia = 0;
        amonestaciones = 0;
        dineroGanadoEnElDia = 0;

        List<string> prohibidos = new List<string>();

        if (dia == 1) prohibidos.Add("leche");
        else if (dia == 2) prohibidos.Add("huevos");
        else if (dia == 3) prohibidos.Add("pan");
        else if (dia == 4) { prohibidos.Add("leche"); prohibidos.Add("huevos"); }
        else if (dia >= 5) { prohibidos.Add("huevos"); prohibidos.Add("pan"); }

        reglaDelDia = new ReglaGobierno(prohibidos);
        eventoDelDiaActual = "PROHIBIDO: " + reglaDelDia.ObtenerListaProhibida().ToUpper();

        // Como el generador ya se creó en el Awake, esto nunca fallará
        clientesDelDia = generador.ObtenerClientesDelDia(dia, prohibidos);

        if (uiManager != null)
        {
            uiManager.ActualizarUI();
            uiManager.MostrarAvisoDia("Día " + dia + ". " + eventoDelDiaActual);
        }
        MostrarClienteActual();
    }

    public void IntentarVender(string productoEntregado)
    {
        if (juegoTerminado || clienteActual == null) return;

        if (productoEntregado != clienteActual.ProductoPedido)
        {
            SumarAmonestacion("¡Error! Producto equivocado.", 0);
        }
        else if (!reglaDelDia.PuedeVender(productoEntregado))
        {
            if (ventaService.RealizarVenta(clienteActual))
            {
                int precio = inventario.ObtenerProducto(productoEntregado).Precio;
                dinero += precio;
                dineroGanadoEnElDia += precio;

                if (uiManager != null) uiManager.MostrarGanancia(precio);
                SumarAmonestacion("Venta ilegal realizada.", 0);
            }
        }
        else
        {
            if (ventaService.RealizarVenta(clienteActual))
            {
                int precio = inventario.ObtenerProducto(productoEntregado).Precio;
                dinero += precio;
                dineroGanadoEnElDia += precio;

                if (uiManager != null) uiManager.MostrarGanancia(precio);
                ultimoMensajeEvento = "Venta exitosa.";
            }
            else ultimoMensajeEvento = "Sin stock.";
        }
        FinalizarTurno();
    }

    public void BotonRechazar()
    {
        Debug.Log("🕵️‍♂️ 1. El GameManager recibió la orden de rechazar.");

        if (juegoTerminado)
        {
            Debug.Log("❌ Cancelado: El juego ya terminó.");
            return;
        }

        if (clienteActual == null)
        {
            Debug.Log("❌ Cancelado: El GameManager cree que clienteActual es NULO.");
            return;
        }

        Debug.Log("✅ 2. Todo en orden, procediendo a rechazar al cliente.");

        bool esProhibido = !reglaDelDia.PuedeVender(clienteActual.ProductoPedido);
        bool sinStock = !inventario.TieneProducto(clienteActual.ProductoPedido);

        if (esProhibido || sinStock)
            ultimoMensajeEvento = "Rechazo correcto.";
        else
            SumarAmonestacion("Rechazaste un cliente legal.", 0);

        FinalizarTurno();
    }

    private void FinalizarTurno()
    {
        clientesAtendidosHoy++;
        ActualizarUICompleta();

        if (!juegoTerminado)
        {
            indiceClienteActualDelDia++;
            if (indiceClienteActualDelDia < clientesDelDia.Count)
            {
                MostrarClienteActual();
            }
            else
            {
                CerrarDiaActual();
            }
        }
    }

    private void MostrarClienteActual()
    {
        if (indiceClienteActualDelDia < clientesDelDia.Count)
        {
            clienteActual = clientesDelDia[indiceClienteActualDelDia];
            if (uiManager != null) uiManager.MostrarCliente(clienteActual);
        }
    }

    private void CerrarDiaActual()
    {
        if (uiManager != null) uiManager.MostrarCliente(null);

        if (dia >= 5)
        {
            bool gano = dinero >= deuda;

            if (gano)
            {
                // Aquí puedes dejar el mensaje de victoria o cargar una escena de ganar si la haces luego
                SceneManager.LoadScene("EscenaGanasteJuego");
            }
            else
            {
                // Si el nea no tiene la liga completa, para la calle
                juegoTerminado = true;
                SceneManager.LoadScene("Escena de Gameover por no pagar la deuda");
            }
        }
        else
        {
            // Si no es el día final, seguimos al resumen normal
            SceneManager.LoadScene("Escena_FinDeDia");
        }
    }

    // --- NUEVA LÓGICA DE COMPRA ---
    public bool ComprarArticulo(string nombreProducto, int costo)
    {
        if (dinero >= costo)
        {
            Producto p = inventario.ObtenerProducto(nombreProducto);
            if (p != null)
            {
                p.AumentarStock(1);
                dinero -= costo;
                return true;
            }
        }
        return false;
    }

    public void IniciarSiguienteDia()
    {
        dia++;
        SceneManager.LoadScene("Escena_Tienda");
    }

    private void SumarAmonestacion(string motivo, int multa)
    {
        amonestaciones++;
        dinero -= multa;
        ultimoMensajeEvento = motivo + " (Faltas: " + amonestaciones + "/" + maxAmonestaciones + ")";

        if (amonestaciones >= maxAmonestaciones)
        {
            // En lugar de abrir un panel, cargamos la escena directamente
            juegoTerminado = true;
            SceneManager.LoadScene("Escena de Gameover por amonestaciones");
        }
    }

    private void FinJuego(bool muerto, string m)
    {
        juegoTerminado = true;
        if (uiManager != null)
        {
            uiManager.MostrarCliente(null);
            uiManager.MostrarGameOver(m);
        }
    }

    public void ReiniciarJuego()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("Escena_Tienda");
    }

    private void ActualizarUICompleta() { if (uiManager != null) uiManager.ActualizarUI(); }

    public string ObtenerUltimoMensajeEvento() => ultimoMensajeEvento;
    public string ObtenerEventoDelDiaActual() => eventoDelDiaActual;
    public int ObtenerMetaMinimaDiaria() => metaMinimaDiaria;
    public int ObtenerClientesAtendidosHoy() => clientesAtendidosHoy;
    public int ObtenerClientesPorDia() => clientesDelDia != null ? clientesDelDia.Count : 5;
    public Inventario ObtenerInventario() => inventario;
    public int ObtenerDineroGanadoEnElDia() => dineroGanadoEnElDia;
}