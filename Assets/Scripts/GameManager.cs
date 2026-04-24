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
    private EventoAleatorio eventoSistema;
    private ReglaGobierno reglaDelDia;

    private List<Cliente> clientesDelDia;
    private int indiceClienteActualDelDia;
    private int clientesAtendidosHoy;
    private string eventoDelDiaActual = "";
    private bool esperandoContinuarDia = false;

    public int dinero = 0;
    public int dia = 1;
    public int deuda = 200;

    [Header("Configuración")]
    public int dineroInicial = 50;
    public int diasMaximos = 5;
    public int amonestaciones = 0;
    public int maxAmonestaciones = 3;
    public int clientesPorDia = 5;
    public int metaMinimaDiaria = 20;

    private bool juegoTerminado = false;
    private string ultimoMensajeEvento = "";
    private int dineroGanadoEnElDia = 0;

    void Start() { InicializarJuego(); }

    private void InicializarJuego()
    {
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
        diasMaximos = 5;
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
        else if (dia == 5) { prohibidos.Add("huevos"); prohibidos.Add("pan"); }

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
            SumarAmonestacion("¡Error de entrega! No era lo que pidió.", 0);
        }
        else if (!reglaDelDia.PuedeVender(productoEntregado))
        {
            if (ventaService.RealizarVenta(clienteActual))
            {
                int precio = inventario.ObtenerProducto(productoEntregado).Precio;
                dinero += precio;
                dineroGanadoEnElDia += precio;
                SumarAmonestacion("Venta ilegal de " + productoEntregado + ".", 0);
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

        if (esProhibido || sinStock) ultimoMensajeEvento = "Rechazo justificado.";
        else SumarAmonestacion("Rechazaste a un cliente legal con stock.", 2);

        FinalizarTurno();
    }

    private void FinalizarTurno()
    {
        clientesAtendidosHoy++;
        ActualizarUICompleta();
        if (!juegoTerminado)
        {
            indiceClienteActualDelDia++;
            if (indiceClienteActualDelDia < clientesDelDia.Count) MostrarClienteActual();
            else CerrarDiaActual();
        }
    }

    private void MostrarClienteActual()
    {
        clienteActual = clientesDelDia[indiceClienteActualDelDia];
        if (uiManager != null) uiManager.MostrarCliente(clienteActual);
    }

    private void CerrarDiaActual()
    {
        if (dia >= diasMaximos)
        {
            bool gano = (amonestaciones == 0);
            string mensajeFin = gano
                ? "¡DÍA 5 IMPECABLE! Pagaste la deuda con los $" + dinero + " ahorrados."
                : "Paila. Llegaste al final pero las multas de hoy te hundieron.";
            FinJuego(!gano, mensajeFin);
        }
        else
        {
            esperandoContinuarDia = true;
            string textoSiguiente = "Terminó el día " + dia + ".\n\n¿Pasamos al día " + (dia + 1) + "?";
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
        ultimoMensajeEvento = motivo + " Amonestación: " + amonestaciones;
        if (amonestaciones >= maxAmonestaciones) FinJuego(true, "Cerrado por el gobierno.");
    }

    private void FinJuego(bool muerto, string m) { juegoTerminado = true; if (uiManager != null) uiManager.MostrarGameOver(m); }
    public void ReiniciarJuego() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    private void ActualizarUICompleta() { if (uiManager != null) uiManager.ActualizarUI(); }

    public string ObtenerUltimoMensajeEvento() => ultimoMensajeEvento;
    public string ObtenerEventoDelDiaActual() => eventoDelDiaActual;
    public int ObtenerMetaMinimaDiaria() => metaMinimaDiaria;
    public int ObtenerClientesAtendidosHoy() => clientesAtendidosHoy;
    public int ObtenerClientesPorDia() => clientesDelDia.Count;
    public Inventario ObtenerInventario() => inventario;
    public int ObtenerDineroGanadoEnElDia() => dineroGanadoEnElDia;
}