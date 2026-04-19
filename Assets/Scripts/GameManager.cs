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
        maxAmonestaciones = 3;
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
        string productoProhibido = eventoSistema.GenerarProductoProhibido(inventario);
        reglaDelDia = new ReglaGobierno(productoProhibido);

        if (string.IsNullOrEmpty(productoProhibido))
        {
            eventoDelDiaActual = "Hoy no hay restricción de venta.";
        }
        else
        {
            eventoDelDiaActual = "Hoy no puedes vender " + productoProhibido + ". Si lo vendes, recibes una amonestación.";
        }

        ultimoMensajeEvento = eventoDelDiaActual;
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
     //   BotonVender();
    }

    public void GenerarNuevoCliente()
    {
        MostrarClienteActual();
    }

    public void IntentarVender(string productoEntregado)
    {
        if (juegoTerminado || esperandoContinuarDia || clienteActual == null)
            return;

        // 1. Validar si le empujó la mercancía que no era
        if (productoEntregado != clienteActual.ProductoPedido)
        {
            SumarAmonestacion("¡Qué chambonada! Le diste " + productoEntregado + " pero el cliente quería " + clienteActual.ProductoPedido + ".", 5);
        }
        // 2. Validar si coronó pero el producto estaba prohibido por el gobierno
        else if (reglaDelDia != null && !reglaDelDia.PuedeVender(productoEntregado))
        {
            bool venta = ventaService.RealizarVenta(clienteActual);
            if (venta)
            {
                Producto producto = inventario.ObtenerProducto(productoEntregado);
                dinero += producto.Precio;
                dineroGanadoEnElDia += producto.Precio;

                // Le da la plata, pero le clava la multa por torcido
                SumarAmonestacion("Vendiste " + productoEntregado + " aunque estaba prohibido por el gobierno. Ganaste $" + producto.Precio + " pero te llevas la multa.", 0);
            }
        }
        // 3. Venta melita y legal
        else
        {
            bool venta = ventaService.RealizarVenta(clienteActual);
            if (venta)
            {
                Producto producto = inventario.ObtenerProducto(productoEntregado);
                dinero += producto.Precio;
                dineroGanadoEnElDia += producto.Precio;

                ultimoMensajeEvento = clienteActual.Nombre + " compró " + productoEntregado + ". Ganaste $" + producto.Precio + ".";
                Debug.Log(ultimoMensajeEvento);
            }
            else
            {
                SumarAmonestacion("Paila, no se pudo vender a " + clienteActual.Nombre + ".", 5);
            }
        }

        clientesAtendidosHoy++;

        if (!juegoTerminado)
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

        // Pillamos si el cliente estaba pidiendo una vuelta prohibida
        if (reglaDelDia != null && !reglaDelDia.PuedeVender(clienteActual.ProductoPedido))
        {
            ultimoMensajeEvento = "Bien jugado. Rechazaste a " + clienteActual.Nombre + " porque pedía algo prohibido. Evitaste la multa.";
            // No restamos plata ni amonestamos porque hizo la vuelta bien
        }
        else
        {
            // Lo echó sin razón y pedía algo legal. Tome su amonestación.
            SumarAmonestacion("Echaste a " + clienteActual.Nombre + " por pura pereza. El producto no estaba prohibido.", 2);
        }

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

        if (clientesAtendidosHoy >= clientesPorDia && dineroGanadoEnElDia < metaMinimaDiaria)
        {
            FinJuego(true, "No cumpliste la meta mínima del día de $" + metaMinimaDiaria +
                          ". Solo ganaste $" + dineroGanadoEnElDia + " en los 5 clientes.");
            ActualizarUICompleta();
            return;
        }

        if (dineroGanadoEnElDia < metaMinimaDiaria)
        {
            ultimoMensajeEvento = "No puedes pasar de día todavía. Debes ganar al menos $" +
                                  metaMinimaDiaria + " y solo llevas $" + dineroGanadoEnElDia + ".";
            ActualizarUICompleta();
            return;
        }

        if (dia >= diasMaximos)
        {
            FinJuego(false, "Cumpliste la meta y terminaste los " + diasMaximos + " días disponibles.");
            ActualizarUICompleta();
            return;
        }

        string mensajeCambioDia = "Se acabó el día " + dia + ". Cumpliste la meta. Pasamos al día " + (dia + 1) + ".";
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

        ultimoMensajeEvento = "Solo puedes pasar el día si cumples la meta mínima. Si no la cumples al llegar a 5 clientes, pierdes automáticamente.";
        ActualizarUICompleta();
    }

    private void SumarAmonestacion(string motivo, int penalizacionDinero)
    {
        amonestaciones++;
        dinero -= penalizacionDinero;

        if (penalizacionDinero > 0)
        {
            ultimoMensajeEvento = motivo + " Recibiste 1 amonestación y perdiste $" + penalizacionDinero + ".";
        }
        else
        {
            ultimoMensajeEvento = motivo + " Recibiste 1 amonestación.";
        }

        if (amonestaciones >= maxAmonestaciones)
        {
            FinJuego(true, "Acumulaste demasiadas amonestaciones.");
            return;
        }

        if (juegoTerminado)
        {
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
