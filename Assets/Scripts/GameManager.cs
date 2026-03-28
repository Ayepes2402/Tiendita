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

    private List<Cliente> clientesTotales;
    private List<Cliente> clientesDelDia;
    private int indiceClienteActualDelDia;
    private int clientesAtendidosHoy;
    private string eventoDelDiaActual = "Sin evento";
    private bool esperandoContinuarDia = false;

    public int dinero = 0;
    public int dia = 1;
    public int deuda = 200;

    [Header("Estado del juego")]
    public int dineroInicial = 50;
    public int diasMaximos = 2;
    public int amonestaciones = 0;
    public int maxAmonestaciones = 3;
    public int clientesPorDia = 5;

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
        diasMaximos = 2;
        dia = 1;
        amonestaciones = 0;
        dineroGanadoEnElDia = 0;
        juegoTerminado = false;
        ultimoMensajeEvento = "La tienda abrió.";

        clientesTotales = generador.GenerarPoolDeClientes();
        PrepararDia(1, true);
        ActualizarUICompleta();
    }

    private void PrepararDia(int nuevoDia, bool esPrimerDia)
    {
        dia = nuevoDia;
        clientesAtendidosHoy = 0;
        dineroGanadoEnElDia = 0;
        esperandoContinuarDia = false;
        indiceClienteActualDelDia = 0;
        clientesDelDia = generador.ObtenerClientesDelDia(clientesTotales, dia, clientesPorDia);

        AplicarEventoDelDia(esPrimerDia);

        if (uiManager != null)
        {
            uiManager.MostrarAvisoDia("Comienza el día " + dia);
        }

        MostrarClienteActual();
    }

    private void AplicarEventoDelDia(bool esPrimerDia)
    {
        TipoEvento evento = eventoSistema.GenerarEvento(inventario);
        string contexto = esPrimerDia ? "al iniciar el juego" : "al iniciar el día " + dia;

        switch (evento)
        {
            case TipoEvento.Robo:
                dinero -= 20;
                eventoDelDiaActual = "Robo en la tienda " + contexto + ". Perdiste $20.";
                break;

            case TipoEvento.Propina:
                dinero += 10;
                eventoDelDiaActual = "Llegó una buena racha " + contexto + ". Ganaste $10 extra.";
                break;

            case TipoEvento.Nada:
                eventoDelDiaActual = "Hoy no ocurrió nada especial.";
                break;

            case TipoEvento.ClienteMolestoPorFaltaDeStock:
                amonestaciones++;
                dinero -= 15;
                eventoDelDiaActual = "Clientes molestos por bajo stock. 1 amonestación y pierdes $15.";
                break;

            case TipoEvento.DescuentoPorProductoPorVencer:
                dinero -= 8;
                eventoDelDiaActual = "Tocó rematar productos por vencer. Pierdes $8.";
                break;

            case TipoEvento.InspeccionSanitaria:
                if (InventarioVacio())
                {
                    amonestaciones++;
                    dinero -= 25;
                    eventoDelDiaActual = "Inspección sanitaria: estantes vacíos. 1 amonestación y multa de $25.";
                }
                else
                {
                    eventoDelDiaActual = "Inspección sanitaria superada sin problemas.";
                }
                break;

            case TipoEvento.BonificacionProveedor:
                dinero += 12;
                eventoDelDiaActual = "Bonificación del proveedor. Ganaste $12.";
                break;

            case TipoEvento.MultaPorDesorden:
                dinero -= 10;
                eventoDelDiaActual = "Multa por desorden en la tienda. Perdiste $10.";
                break;
        }

        ultimoMensajeEvento = eventoDelDiaActual;

        if (amonestaciones >= maxAmonestaciones)
        {
            FinJuego(true, "Acumulaste demasiadas amonestaciones.");
            return;
        }

        VerificarQuiebra();
    }

    private void MostrarClienteActual()
    {
        if (juegoTerminado)
        {
            return;
        }

        if (clientesDelDia == null || indiceClienteActualDelDia >= clientesDelDia.Count)
        {
            CerrarDiaActual();
            return;
        }

        clienteActual = clientesDelDia[indiceClienteActualDelDia];
        Debug.Log("Cliente del día " + dia + ": " + clienteActual.Nombre + " pide " + clienteActual.ProductoPedido);

        if (uiManager != null)
        {
            uiManager.MostrarCliente(clienteActual);
            uiManager.ActualizarUI();
        }
    }


    public void AtenderCliente(Cliente cliente)
    {
        if (juegoTerminado || esperandoContinuarDia || cliente == null)
            return;

        clienteActual = cliente;
        BotonVender();
    }

    public void GenerarNuevoCliente()
    {
        MostrarClienteActual();
    }

    public void BotonVender()
    {
        if (juegoTerminado || esperandoContinuarDia || clienteActual == null)
            return;

        bool venta = ventaService.RealizarVenta(clienteActual);
        clientesAtendidosHoy++;

        if (venta)
        {
            Producto producto = inventario.ObtenerProducto(clienteActual.ProductoPedido);
            dinero += producto.Precio;
            dineroGanadoEnElDia += producto.Precio;
            ultimoMensajeEvento = clienteActual.Nombre + " compró " + clienteActual.ProductoPedido +
                                  ". Ganaste $" + producto.Precio + ".";
            Debug.Log(ultimoMensajeEvento);
        }
        else
        {
            amonestaciones++;
            dinero -= 5;
            ultimoMensajeEvento = "No se pudo vender a " + clienteActual.Nombre +
                                  ". Recibiste 1 amonestación y perdiste $5.";
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
            PasarAlSiguienteCliente();
        }
    }

    public void BotonRechazar()
    {
        if (juegoTerminado || esperandoContinuarDia || clienteActual == null)
            return;

        clientesAtendidosHoy++;
        dinero -= 2;
        ultimoMensajeEvento = clienteActual.Nombre + " se fue bravo. Perdiste $2 por rechazarlo.";
        Debug.Log(ultimoMensajeEvento);

        VerificarQuiebra();
        ActualizarUICompleta();

        if (!juegoTerminado)
        {
            PasarAlSiguienteCliente();
        }
    }

    private void PasarAlSiguienteCliente()
    {
        indiceClienteActualDelDia++;

        if (clientesAtendidosHoy >= clientesPorDia || indiceClienteActualDelDia >= clientesDelDia.Count)
        {
            CerrarDiaActual();
        }
        else
        {
            MostrarClienteActual();
        }
    }

    private void CerrarDiaActual()
    {
        if (juegoTerminado)
        {
            return;
        }

        if (dia >= diasMaximos)
        {
            FinJuego(false, "Terminaste los " + diasMaximos + " días disponibles.");
            ActualizarUICompleta();
            return;
        }

        string mensajeCambioDia = "Se acabó el día " + dia + ". Pasamos al día " + (dia + 1) + ".";
        ultimoMensajeEvento = mensajeCambioDia;
        esperandoContinuarDia = true;
        Debug.Log(mensajeCambioDia);

        if (uiManager != null)
        {
            uiManager.MostrarAvisoDia(mensajeCambioDia);
            uiManager.MostrarPanelCambioDia(mensajeCambioDia);
        }

        ActualizarUICompleta();
    }


    public void BotonContinuarDia()
    {
        if (juegoTerminado || !esperandoContinuarDia)
        {
            return;
        }

        esperandoContinuarDia = false;

        if (uiManager != null)
        {
            uiManager.OcultarPanelCambioDia();
        }

        PrepararDia(dia + 1, false);
        ActualizarUICompleta();
    }

    public void FinDelDia()
    {
        if (juegoTerminado)
            return;

        ultimoMensajeEvento = "Ahora el día termina automáticamente al atender 5 clientes.";
        ActualizarUICompleta();
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
        else if (!string.IsNullOrEmpty(motivo))
        {
            ultimoMensajeEvento = motivo;
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

    public string ObtenerEventoDelDiaActual()
    {
        return eventoDelDiaActual;
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

    public int ObtenerClientesAtendidosHoy()
    {
        return clientesAtendidosHoy;
    }

    public int ObtenerClientesPorDia()
    {
        return clientesPorDia;
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
            uiManager.MostrarEventoDelDia(eventoDelDiaActual);
        }
    }

}
