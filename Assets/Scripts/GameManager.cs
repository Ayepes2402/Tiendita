using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  
    public static GameManager Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private ClienteGenerator generador;
    private UIManager uiManager;
    private Inventario inventario;
    private VentaService ventaService;
    private ReglaGobierno reglaDelDia;

   
    private List<ClienteData> clientesDelDia;
    private ClienteData clienteActual;

    private int indiceClienteActualDelDia;
    private int clientesAtendidosHoy;
    private string eventoDelDiaActual = "";


    public int Dinero { get; private set; }
    public int Dia { get; private set; }
    public int Amonestaciones { get; private set; }

   
    public int Deuda { get; private set; } = 6500;
    public int MaxAmonestaciones { get; private set; } = 3;
    public int CostoCompraPan { get; private set; } = 300;
    public int CostoCompraLeche { get; private set; } = 500;
    public int CostoCompraHuevos { get; private set; } = 400;

    private int diasMaximos = 5;
    private int dineroInicial = 1500;
    private int metaMinimaDiaria = 20;

    public bool juegoTerminado = false;
    public string ultimoMensajeEvento = "";
    public int dineroGanadoEnElDia = 0;

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
        juegoTerminado = false;
    }

    void OnEnable()
    {
        if (Instance == this) SceneManager.sceneLoaded += AlCargarEscena;
    }

    void OnDisable()
    {
        if (Instance == this) SceneManager.sceneLoaded -= AlCargarEscena;
    }

    private void AlCargarEscena(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Escena_Tienda")
        {
            uiManager = FindObjectOfType<UIManager>(); 
        
        
        if (generador == null)
            {
                generador = FindObjectOfType<ClienteGenerator>(); 
        }

            if (uiManager != null)
            {
                PrepararDia(Dia); 
        }
        }
    }

    private void PrepararDia(int nuevoDia)
    {
        Dia = nuevoDia;
        clientesAtendidosHoy = 0;
        indiceClienteActualDelDia = 0;
        Amonestaciones = 0;
        dineroGanadoEnElDia = 0;

        List<string> prohibidos = new List<string>();

       
        if (Dia == 1) prohibidos.Add("leche");
        else if (Dia == 2) prohibidos.Add("huevos");
        else if (Dia == 3) prohibidos.Add("pan");
        else if (Dia == 4) { prohibidos.Add("leche"); prohibidos.Add("huevos"); }
        else if (Dia >= 5) { prohibidos.Add("huevos"); prohibidos.Add("pan"); }

        reglaDelDia = new ReglaGobierno(prohibidos);
        eventoDelDiaActual = "PROHIBIDO: " + reglaDelDia.ObtenerListaProhibida().ToUpper();

    
        if (generador != null)
        {
            clientesDelDia = generador.ObtenerClientesDelDia(Dia);
        }
        else
        {
            Debug.LogError("Falta asignar el GeneradorClientes en el Inspector.");
        }

        if (uiManager != null)
        {
            uiManager.ActualizarUI();
            uiManager.MostrarAvisoDia("Día " + Dia + ". " + eventoDelDiaActual);
        }
        MostrarClienteActual();
    }


    private string ObtenerProductoRealDelCliente()
    {
        string productoPedido = clienteActual.ProductoPedido;

       
        if (productoPedido == "trampa")
        {
            if (Dia == 1) productoPedido = "leche";
            else if (Dia == 2) productoPedido = "huevos";
            else if (Dia == 3) productoPedido = "pan";
            else if (Dia == 4) productoPedido = "leche"; 
            else if (Dia >= 5) productoPedido = "huevos"; 
        }

        return productoPedido;
    }

  

    public void IntentarVender(string productoEntregado)
    {
        if (juegoTerminado || clienteActual == null) return;

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
                int pagoFinal = precioBase;

                
                if (clienteActual.Tipo == TipoCliente.Sospechoso)
                {
                    pagoFinal *= 2;
                    ultimoMensajeEvento = "¡Venta ilegal de alto riesgo! Pago doble.";
                }
                else
                {
                    ultimoMensajeEvento = "Venta ilegal realizada.";
                }

                Dinero += pagoFinal;
                dineroGanadoEnElDia += pagoFinal;

                if (uiManager != null) uiManager.MostrarGanancia(pagoFinal);
                SumarAmonestacion(ultimoMensajeEvento, 0);
            }
            else ultimoMensajeEvento = "Sin stock.";
        }
        else
        {
           
            Cliente clienteTemp = new Cliente(clienteActual.Nombre, clienteActual.Tipo, productoEntregado, clienteActual.Dinero, "", 0);
            if (ventaService.RealizarVenta(clienteTemp))
            {
                int precio = inventario.ObtenerProducto(productoEntregado).Precio;
                Dinero += precio;
                dineroGanadoEnElDia += precio;
                if (uiManager != null) uiManager.MostrarGanancia(precio);
                ultimoMensajeEvento = "Venta exitosa.";
            }
            else ultimoMensajeEvento = "Sin stock.";
        }
        FinalizarTurno();
        ActualizarContadoresVisuales();
    }

    private void ActualizarContadoresVisuales()
    {
       
        DragUICategoria[] iconos = FindObjectsOfType<DragUICategoria>();
        foreach (DragUICategoria icono in iconos)
        {
            icono.ActualizarTextoDesdeBackend(); 
    }
    }

    public void BotonRechazar()
    {
        if (juegoTerminado || clienteActual == null) return;

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

            string productoReal = ObtenerProductoRealDelCliente();

          
            if (uiManager != null) uiManager.MostrarCliente(clienteActual, productoReal);
        }
    }

    private void CerrarDiaActual()
    {
        if (uiManager != null) uiManager.MostrarCliente(null);

        if (Dia >= 5)
        {
            bool gano = Dinero >= Deuda;
            if (gano) SceneManager.LoadScene("EscenaGanasteJuego");
            else
            {
                juegoTerminado = true;
                SceneManager.LoadScene("Escena de Gameover por no pagar la deuda");
            }
        }
        else
        {
            SceneManager.LoadScene("Escena_FinDeDia");
        }
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

    private void SumarAmonestacion(string motivo, int multa)
    {
        Amonestaciones++;
        Dinero -= multa;
        ultimoMensajeEvento = motivo + " (Faltas: " + Amonestaciones + "/" + MaxAmonestaciones + ")";

        if (Amonestaciones >= MaxAmonestaciones)
        {
            juegoTerminado = true;
            SceneManager.LoadScene("Escena de Gameover por amonestaciones");
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