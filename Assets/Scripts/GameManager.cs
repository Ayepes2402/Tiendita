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

    public int dinero = 0;
    public int dia = 1;
    public int deuda = 200;

    [Header("Estado del juego")]
    public int dineroInicial = 50;
    public int diasMaximos = 7;
    public int amonestaciones = 0;
    public int maxAmonestaciones = 3;

    [Header("Meta diaria")]
    public int metaMinimaDiaria = 20;
    private int dineroGanadoEnElDia = 0;

    private bool juegoTerminado = false;
    private string ultimoMensajeEvento = "";

    void Start()
    {
        InicializarJuego();
    }

    private void InicializarJuego()
    {
        List<Producto> productos = new List<Producto>
        {
            new Producto("pan", 10, 10, CategoriaProducto.Basico),
            new Producto("leche", 12, 5, CategoriaProducto.Lacteo),
            new Producto("huevos", 15, 6, CategoriaProducto.Proteina)
        };

        inventario = new Inventario(productos);
        ventaService = new VentaService(inventario);
        eventoSistema = new EventoAleatorio();
        generador = new ClienteGenerator();
        uiManager = FindObjectOfType<UIManager>();

        dinero = dineroInicial;
        dia = 1;
        amonestaciones = 0;
        dineroGanadoEnElDia = 0;
        juegoTerminado = false;
        ultimoMensajeEvento = "La tienda abrió. Meta del día: $" + metaMinimaDiaria;

        ProcesarEvento(eventoSistema.GenerarEvento(inventario), true);
        GenerarNuevoCliente();
        ActualizarUICompleta();
    }

    public void AtenderCliente(Cliente cliente)
    {
        if (juegoTerminado || cliente == null)
            return;

        bool venta = ventaService.RealizarVenta(cliente);

        if (venta)
        {
            Producto producto = inventario.ObtenerProducto(cliente.ProductoPedido);
            dinero += producto.Precio;
            dineroGanadoEnElDia += producto.Precio;
            ultimoMensajeEvento = "Venta realizada de " + cliente.ProductoPedido +
                                  ". Ganaste $" + producto.Precio +
                                  ". Progreso diario: $" + dineroGanadoEnElDia + "/$" + metaMinimaDiaria;
            Debug.Log(ultimoMensajeEvento);
        }
        else
        {
            ultimoMensajeEvento = "No se pudo realizar la venta de " + cliente.ProductoPedido;
            Debug.Log(ultimoMensajeEvento);
        }

        VerificarQuiebra();
        ActualizarUICompleta();
    }

    public void FinDelDia()
    {
        if (juegoTerminado)
            return;

        if (dineroGanadoEnElDia < metaMinimaDiaria)
        {
            ultimoMensajeEvento = "No puedes pasar de día. Debes ganar al menos $" +
                                  metaMinimaDiaria + " y solo llevas $" + dineroGanadoEnElDia + ".";
            Debug.Log(ultimoMensajeEvento);
            ActualizarUICompleta();
            return;
        }

        Debug.Log("Fin del día " + dia);

        ProcesarEvento(eventoSistema.GenerarEvento(inventario), false);
        VerificarQuiebra();

        if (juegoTerminado)
        {
            ActualizarUICompleta();
            return;
        }

        dia++;
        dineroGanadoEnElDia = 0;
        amonestaciones = 0;

        if (dia > diasMaximos)
        {
            FinJuego(false);
        }
        else
        {
            ultimoMensajeEvento = "Comienza el día " + dia + ". Meta del día: $" + metaMinimaDiaria;
            Debug.Log(ultimoMensajeEvento);
            GenerarNuevoCliente();
        }

        ActualizarUICompleta();
    }

    private void ProcesarEvento(TipoEvento evento, bool esInicioDelJuego)
    {
        string contexto = esInicioDelJuego ? "al iniciar la partida" : "al finalizar el día";

        switch (evento)
        {
            case TipoEvento.Robo:
                dinero -= 20;
                ultimoMensajeEvento = "Ocurrió un robo " + contexto + ". Perdiste $20.";
                break;

            case TipoEvento.Propina:
                dinero += 10;
                ultimoMensajeEvento = "Un cliente dejó propina " + contexto + ". Ganaste $10.";
                break;

            case TipoEvento.Nada:
                ultimoMensajeEvento = "No ocurrió ningún evento " + contexto + ".";
                break;

            case TipoEvento.ClienteMolestoPorFaltaDeStock:
                amonestaciones++;
                dinero -= 15;
                ultimoMensajeEvento = "Hubo quejas por poco inventario. Recibiste 1 amonestación y perdiste $15.";
                break;

            case TipoEvento.DescuentoPorProductoPorVencer:
                dinero -= 8;
                ultimoMensajeEvento = "Tuviste que rematar productos con mucho stock. Perdiste $8.";
                break;

            case TipoEvento.InspeccionSanitaria:
                if (InventarioVacio())
                {
                    amonestaciones++;
                    dinero -= 25;
                    ultimoMensajeEvento = "La inspección encontró estantes vacíos. 1 amonestación y multa de $25.";
                }
                else
                {
                    ultimoMensajeEvento = "Hubo inspección sanitaria, pero todo estaba en orden.";
                }
                break;

            case TipoEvento.BonificacionProveedor:
                dinero += 12;
                ultimoMensajeEvento = "El proveedor te dio una bonificación por buen movimiento de stock. Ganaste $12.";
                break;

            case TipoEvento.MultaPorDesorden:
                dinero -= 10;
                ultimoMensajeEvento = "Pagaste una multa por desorden en la tienda. Perdiste $10.";
                break;
        }

        Debug.Log(ultimoMensajeEvento);

        if (amonestaciones >= maxAmonestaciones)
        {
            FinJuego(true, "Acumulaste demasiadas amonestaciones.");
            return;
        }

        VerificarQuiebra();
    }

    private bool InventarioVacio()
    {
        foreach (Producto producto in inventario.ObtenerProductos())
        {
            if (producto.Cantidad > 0)
            {
                return false;
            }
        }

        return true;
    }

    private void VerificarQuiebra()
    {
        if (dinero < 0)
        {
            FinJuego(true, "Llegaste a quiebra. Te quedaste sin dinero.");
        }
    }

    private void FinJuego(bool forzarGameOver, string motivo = "")
    {
        juegoTerminado = true;

        if (forzarGameOver)
        {
            ultimoMensajeEvento = "GAME OVER. " + motivo;
        }
        else if (dinero >= deuda)
        {
            ultimoMensajeEvento = "Ganaste el juego, pagaste la deuda.";
        }
        else
        {
            ultimoMensajeEvento = "Perdiste el juego, no pudiste pagar la deuda.";
        }

        Debug.Log(ultimoMensajeEvento);

        if (uiManager != null)
        {
            uiManager.MostrarGameOver(ultimoMensajeEvento);
        }
    }

    public void GenerarNuevoCliente()
    {
        if (juegoTerminado)
            return;

        clienteActual = generador.GenerarCliente();
        Debug.Log("Cliente pide: " + clienteActual.ProductoPedido);

        if (uiManager != null)
        {
            uiManager.MostrarCliente(clienteActual);
            uiManager.ActualizarUI();
        }
    }

    public void BotonVender()
    {
        if (juegoTerminado || clienteActual == null)
            return;

        bool venta = ventaService.RealizarVenta(clienteActual);

        if (venta)
        {
            Producto producto = inventario.ObtenerProducto(clienteActual.ProductoPedido);
            dinero += producto.Precio;
            dineroGanadoEnElDia += producto.Precio;
            ultimoMensajeEvento = "Venta exitosa de " + clienteActual.ProductoPedido +
                                  ". Ganaste $" + producto.Precio +
                                  ". Progreso diario: $" + dineroGanadoEnElDia + "/$" + metaMinimaDiaria;
            Debug.Log(ultimoMensajeEvento);
        }
        else
        {
            amonestaciones++;
            dinero -= 5;
            ultimoMensajeEvento = "No se pudo vender. Recibiste 1 amonestación y perdiste $5.";
            Debug.Log(ultimoMensajeEvento);
        }

        if (amonestaciones >= maxAmonestaciones)
        {
            FinJuego(true, "Acumulaste demasiadas amonestaciones.");
        }
        else
        {
            VerificarQuiebra();
        }

        ActualizarUICompleta();

        if (!juegoTerminado)
        {
            SiguienteCliente();
        }
    }

    public void BotonRechazar()
    {
        if (juegoTerminado)
            return;

        dinero -= 2;
        ultimoMensajeEvento = "Cliente rechazado. Perdiste $2 por oportunidad desaprovechada.";
        Debug.Log(ultimoMensajeEvento);

        VerificarQuiebra();
        ActualizarUICompleta();

        if (!juegoTerminado)
        {
            SiguienteCliente();
        }
    }

    public void ReiniciarJuego()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public Inventario ObtenerInventario()
    {
        return inventario;
    }

    public string ObtenerUltimoMensajeEvento()
    {
        return ultimoMensajeEvento;
    }

    public bool JuegoTerminado()
    {
        return juegoTerminado;
    }

    public int ObtenerDineroGanadoEnElDia()
    {
        return dineroGanadoEnElDia;
    }

    public int ObtenerMetaMinimaDiaria()
    {
        return metaMinimaDiaria;
    }

    void SiguienteCliente()
    {
        GenerarNuevoCliente();
    }

    private void ActualizarUICompleta()
    {
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }

        if (uiManager != null)
        {
            uiManager.ActualizarUI();
            uiManager.MostrarEstado(ultimoMensajeEvento);
        }
    }
}