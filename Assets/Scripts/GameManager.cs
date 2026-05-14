using System; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // SINGLETON 
    public static GameManager Instance { get; private set; }

    // OBSERVADOR 
 
    public static event Action OnDatosActualizados;

    [Header("Configuración y Referencias")]
    [SerializeField] private ClienteGenerator generador;
    
    [SerializeField] private int dineroInicial = 1500;
    [SerializeField] private int metaMinimaDiaria = 20;


    private Inventario inventario;
    private VentaService ventaService;
    private ReglaGobierno reglaDelDia;
    private List<ClienteData> clientesDelDia;
    private ClienteData clienteActual;
    private int indiceClienteActualDelDia;
    private int clientesAtendidosHoy;
    private string eventoDelDiaActual = "";
    private string ultimoMensajeEvento = "";

   
    public int Dinero { get; private set; }
    public int Dia { get; private set; }
    public int Amonestaciones { get; private set; }
    public bool JuegoTerminado { get; private set; } = false;
    public int DineroGanadoEnElDia { get; private set; } = 0;

 
    public int Deuda { get; private set; } = 6500;
    public int MaxAmonestaciones { get; private set; } = 3;
    public int CostoCompraPan { get; private set; } = 300;
    public int CostoCompraLeche { get; private set; } = 500;
    public int CostoCompraHuevos { get; private set; } = 400;

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
        inventario = new Inventario(new List<Producto> {
            new Producto("pan", 600, 5, CategoriaProducto.Basico),
            new Producto("leche", 800, 5, CategoriaProducto.Lacteo),
            new Producto("huevos", 1000, 5, CategoriaProducto.Proteina)
        });

        ventaService = new VentaService(inventario);
        Dinero = dineroInicial;
        Dia = 0;
        JuegoTerminado = false;
    }

    private void OnEnable() => SceneManager.sceneLoaded += AlCargarEscena;
    private void OnDisable() => SceneManager.sceneLoaded -= AlCargarEscena;

    private void AlCargarEscena(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Escena_Tienda")
        {
           
            if (generador == null) generador = FindObjectOfType<ClienteGenerator>();
            PrepararDia(Dia);
        }
    }

    private void PrepararDia(int nuevoDia)
    {
        Dia = nuevoDia;
        clientesAtendidosHoy = 0;
        indiceClienteActualDelDia = 0;
        Amonestaciones = 0;
        DineroGanadoEnElDia = 0;

        List<string> prohibidos = new List<string>();

        
        if (Dia == 1) prohibidos.Add("leche");
        else if (Dia == 2) prohibidos.Add("huevos");
        else if (Dia == 3) prohibidos.Add("pan");
        else if (Dia == 4) { prohibidos.Add("leche"); prohibidos.Add("huevos"); }
        else if (Dia >= 5) { prohibidos.Add("huevos"); prohibidos.Add("pan"); }

        reglaDelDia = new ReglaGobierno(prohibidos);
        eventoDelDiaActual = "PROHIBIDO: " + reglaDelDia.ObtenerListaProhibida().ToUpper();

        if (generador != null)
            clientesDelDia = generador.ObtenerClientesDelDia(Dia);

    
        NotificarCambios();
        MostrarClienteActual();
    }

    public void IntentarVender(string productoEntregado)
    {
        if (JuegoTerminado || clienteActual == null) return;

        string productoQueQuiere = ObtenerProductoRealDelCliente();

        if (productoEntregado != productoQueQuiere)
        {
            SumarAmonestacion("¡Error! Producto equivocado.", 0);
        }
        else if (!reglaDelDia.PuedeVender(productoEntregado))
        {
            Cliente clienteTemp = new Cliente(clienteActual.Nombre, clienteActual.Tipo, productoEntregado, clienteActual.Dinero, "", 0);
            if (ventaService.RealizarVenta(clienteTemp))
            {
                int precioBase = inventario.ObtenerProducto(productoEntregado).Precio;
                int pagoFinal = (clienteActual.Tipo == TipoCliente.Sospechoso) ? precioBase * 2 : precioBase;

                ultimoMensajeEvento = (clienteActual.Tipo == TipoCliente.Sospechoso) ?
                    "¡Venta ilegal de alto riesgo! Pago doble." : "Venta ilegal realizada.";

                Dinero += pagoFinal;
                DineroGanadoEnElDia += pagoFinal;
                SumarAmonestacion(ultimoMensajeEvento, 0);

                FindObjectOfType<UIManager>()?.MostrarGanancia(pagoFinal);
            }
        }
        else
        {
            Cliente clienteTemp = new Cliente(clienteActual.Nombre, clienteActual.Tipo, productoEntregado, clienteActual.Dinero, "", 0);
            if (ventaService.RealizarVenta(clienteTemp))
            {
                int precio = inventario.ObtenerProducto(productoEntregado).Precio;
                Dinero += precio;
                DineroGanadoEnElDia += precio;
                UIManager ui = FindObjectOfType<UIManager>();
                if (ui != null)
                {
                    ui.MostrarGanancia(precio); 
                }
                ultimoMensajeEvento = "Venta exitosa.";
            }
        }

        FinalizarTurno();
    }

    public void BotonRechazar()
    {
        if (JuegoTerminado || clienteActual == null) return;

        string productoQueQuiere = ObtenerProductoRealDelCliente();
        bool esProhibido = !reglaDelDia.PuedeVender(productoQueQuiere);
        bool sinStock = !inventario.TieneProducto(productoQueQuiere);

        if (esProhibido || sinStock)
            ultimoMensajeEvento = "Rechazo correcto.";
        else
            SumarAmonestacion("Rechazaste un cliente legal.", 0);

        FinalizarTurno();
    }

    private void FinalizarTurno()
    {
        clientesAtendidosHoy++;

        if (!JuegoTerminado)
        {
            indiceClienteActualDelDia++;
            if (indiceClienteActualDelDia < clientesDelDia.Count)
                MostrarClienteActual();
            else
                CerrarDiaActual();
        }
        NotificarCambios();
    }

    private void NotificarCambios()
    {
        
        OnDatosActualizados?.Invoke();
    }

  
    private string ObtenerProductoRealDelCliente()
    {
        if (clienteActual.ProductoPedido != "trampa") return clienteActual.ProductoPedido;

        return Dia switch
        {
            1 => "leche",
            2 => "huevos",
            3 => "pan",
            4 => "leche",
            _ => "huevos"
        };
    }

    private void MostrarClienteActual()
    {
        if (indiceClienteActualDelDia < clientesDelDia.Count)
        {
            clienteActual = clientesDelDia[indiceClienteActualDelDia];
          
            FindObjectOfType<UIManager>()?.MostrarCliente(clienteActual, ObtenerProductoRealDelCliente());
        }
    }

    private void CerrarDiaActual()
    {
        if (Dia >= 5)
        {
            SceneManager.LoadScene(Dinero >= Deuda ? "EscenaGanasteJuego" : "Escena de Gameover por no pagar la deuda");
        }
        else SceneManager.LoadScene("Escena_FinDeDia");
    }

    public bool ComprarArticulo(string nombreProducto, int costo)
    {
        if (Dinero >= costo)
        {
            Producto p = inventario.ObtenerProducto(nombreProducto);
            if (p != null)
            {
                p.AumentarStock(1);
                Dinero -= costo;
                NotificarCambios();
                return true;
            }
        }
        return false;
    }

    public void IniciarSiguienteDia()
    {
        Dia++;
        SceneManager.LoadScene("Escena_Tienda");
    }
    public void ReiniciarJuego()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("Escena_Tienda");
    }
    private void SumarAmonestacion(string motivo, int multa)
    {
        Amonestaciones++;
        Dinero -= multa;
        ultimoMensajeEvento = $"{motivo} (Faltas: {Amonestaciones}/{MaxAmonestaciones})";

        if (Amonestaciones >= MaxAmonestaciones)
        {
            JuegoTerminado = true;
            SceneManager.LoadScene("Escena de Gameover por amonestaciones");
        }
    }

    
    public string ObtenerUltimoMensajeEvento() => ultimoMensajeEvento;
    public string ObtenerEventoDelDiaActual() => eventoDelDiaActual;
    public int ObtenerMetaMinimaDiaria() => metaMinimaDiaria;
    public int ObtenerClientesAtendidosHoy() => clientesAtendidosHoy;
    public int ObtenerClientesPorDia() => clientesDelDia?.Count ?? 5;
    public Inventario ObtenerInventario() => inventario;
}