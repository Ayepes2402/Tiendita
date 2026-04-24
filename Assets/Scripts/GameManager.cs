using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
    private bool esperandoContinuarDia = false;

    [Header("Economía y Balanceo Final")]
    public int dinero = 50;
    public int deuda = 600;      // Meta de victoria
    public int dia = 1;
    [Tooltip("El juego obligatoriamente durará estos días")]
    public int diasMaximos = 5;

    [Header("Configuración de Reglas")]
    public int dineroInicial = 50;
    public int amonestaciones = 0;
    public int maxAmonestaciones = 3;
    public int clientesPorDia = 5;
    public int metaMinimaDiaria = 20;

    [Header("Planteamiento Futura Tienda")]
    public int costoCompraPan = 10;
    public int costoCompraLeche = 15;
    public int costoCompraHuevos = 18;

    private bool juegoTerminado = false;
    private string ultimoMensajeEvento = "";
    private int dineroGanadoEnElDia = 0;

    void Start() { InicializarJuego(); }

    private void InicializarJuego()
    {
        // Forzamos el límite a 5 días
        diasMaximos = 5;

        inventario = new Inventario(new List<Producto> {
            new Producto("pan", 20, 15, CategoriaProducto.Basico),
            new Producto("leche", 25, 12, CategoriaProducto.Lacteo),
            new Producto("huevos", 30, 12, CategoriaProducto.Proteina)
        });

        ventaService = new VentaService(inventario);
        generador = new ClienteGenerator();
        uiManager = FindObjectOfType<UIManager>();

        dinero = dineroInicial;
        dia = 1;
        juegoTerminado = false;
        PrepararDia(1);
    }

    private void PrepararDia(int nuevoDia)
    {
        dia = nuevoDia;
        clientesAtendidosHoy = 0;
        indiceClienteActualDelDia = 0;
        amonestaciones = 0;
        esperandoContinuarDia = false;
        dineroGanadoEnElDia = 0;

        List<string> prohibidos = new List<string>();

        if (dia == 1) prohibidos.Add("leche");
        else if (dia == 2) prohibidos.Add("huevos");
        else if (dia == 3) prohibidos.Add("pan");
        else if (dia == 4) { prohibidos.Add("leche"); prohibidos.Add("huevos"); }
        else if (dia >= 5) { prohibidos.Add("huevos"); prohibidos.Add("pan"); }

        reglaDelDia = new ReglaGobierno(prohibidos);
        eventoDelDiaActual = "PROHIBIDO: " + reglaDelDia.ObtenerListaProhibida().ToUpper();

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
        if (juegoTerminado || esperandoContinuarDia || clienteActual == null) return;

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
                ultimoMensajeEvento = "Venta exitosa.";
            }
            else ultimoMensajeEvento = "Sin stock.";
        }
        FinalizarTurno();
    }

    public void BotonRechazar()
    {
        if (juegoTerminado || esperandoContinuarDia || clienteActual == null) return;

        bool esProhibido = !reglaDelDia.PuedeVender(clienteActual.ProductoPedido);
        bool sinStock = !inventario.TieneProducto(clienteActual.ProductoPedido);

        if (esProhibido || sinStock) ultimoMensajeEvento = "Rechazo correcto.";
        else SumarAmonestacion("Rechazaste un cliente legal.", 0);

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
        // BLINDAJE: Solo cobra la deuda en el día 5
        if (dia >= 5)
        {
            bool gano = dinero >= deuda;
            string mensajeFin = gano ?
                "¡VICTORIA! Pagaste la deuda de $" + deuda + "." :
                "DERROTA. Fin del Día 5. Solo tienes $" + dinero + " de $" + deuda + ".";

            FinJuego(!gano, mensajeFin);
        }
        else
        {
            esperandoContinuarDia = true;
            string textoSiguiente = "Día " + dia + " completado.\nDinero: $" + dinero + "\nMeta Final en el Día 5: $" + deuda;
            if (uiManager != null) uiManager.MostrarPanelCambioDia(textoSiguiente);
        }
    }

    public void BotonContinuarDia()
    {
        if (!esperandoContinuarDia) return;
        esperandoContinuarDia = false;
        if (uiManager != null) uiManager.OcultarPanelCambioDia();
        PrepararDia(dia + 1);
    }

    private void SumarAmonestacion(string motivo, int multa)
    {
        amonestaciones++;
        dinero -= multa;
        ultimoMensajeEvento = motivo + " (Faltas: " + amonestaciones + "/" + maxAmonestaciones + ")";

        if (amonestaciones >= maxAmonestaciones)
        {
            FinJuego(true, "¡CLAUSURADO! Perdiste por acumular 3 faltas en un mismo día.");
        }
    }

    private void FinJuego(bool muerto, string m)
    {
        juegoTerminado = true;
        if (uiManager != null) uiManager.MostrarGameOver(m);
    }

    public void ReiniciarJuego() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    private void ActualizarUICompleta() { if (uiManager != null) uiManager.ActualizarUI(); }

    // --- MÉTODOS OBLIGATORIOS PARA EVITAR LOS ERRORES ROJOS EN UNITY ---
    public string ObtenerUltimoMensajeEvento() => ultimoMensajeEvento;
    public string ObtenerEventoDelDiaActual() => eventoDelDiaActual;
    public int ObtenerMetaMinimaDiaria() => metaMinimaDiaria;
    public int ObtenerClientesAtendidosHoy() => clientesAtendidosHoy;
    public int ObtenerClientesPorDia() => clientesDelDia != null ? clientesDelDia.Count : 5;
    public Inventario ObtenerInventario() => inventario;
    public int ObtenerDineroGanadoEnElDia() => dineroGanadoEnElDia;
}