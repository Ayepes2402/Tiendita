using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Cliente clienteActual;
    private ClienteGenerator generador;
    private UIManager uiManager;
    private Inventario inventario;
    private VentaService ventaService;
    private EventoAleatorio eventoSistema;

    public int dinero = 0; // se analizará posteriormente la posibilidad de serializar las variables [SerializeField]
    public int dia = 1;
    public int deuda = 200;

    void Start()
    {
        List<Producto> productos = new List<Producto>();

        productos.Add(new Producto("pan", 10, 10));
        productos.Add(new Producto("leche", 12, 5));
        productos.Add(new Producto("huevos", 15, 6));

        inventario = new Inventario(productos);
        ventaService = new VentaService(inventario);
        eventoSistema = new EventoAleatorio();
        generador = new ClienteGenerator();
        uiManager = FindObjectOfType<UIManager>();

        dinero = 50;

        GenerarNuevoCliente();
    }

    public void AtenderCliente(Cliente cliente)
    {
        bool venta = ventaService.RealizarVenta(cliente);

        if (venta)
        {
            Producto producto = inventario.ObtenerProducto(cliente.ProductoPedido);
            dinero += producto.Precio;

            Debug.Log("Venta realizada de " + cliente.ProductoPedido);
        }
        else
        {
            Debug.Log("No se pudo realizar la venta");
        }
    }

    public void FinDelDia()
    {
        Debug.Log("Fin del día " + dia);

        
        TipoEvento evento = eventoSistema.GenerarEvento();

        ProcesarEvento(evento);

        dia++;

        if (dia > 7)
        {
            FinJuego();
        }
        else
        {
            Debug.Log("Comienza el día " + dia);
        }
    }

    private void ProcesarEvento(TipoEvento evento)
    {
        switch (evento)
        {
            case TipoEvento.Robo:
                dinero -= 20;
                Debug.Log("Un ladrón robó dinero de la caja");
                break;

            case TipoEvento.Propina:
                dinero += 10;
                Debug.Log("Un cliente dejó propina");
                break;

            case TipoEvento.Nada:
                Debug.Log("No ocurrió ningún evento hoy");
                break;
        }
    }

    private void FinJuego()
    {
        if (dinero >= deuda)
        {
            Debug.Log("Ganaste el juego, pagaste la deuda");
        }
        else
        {
            Debug.Log("Perdiste el juego, no pudiste pagar la deuda");
        }
    }

    public void GenerarNuevoCliente()
    {
        clienteActual = generador.GenerarCliente();

        Debug.Log("Cliente pide: " + clienteActual.ProductoPedido);

        uiManager.MostrarCliente(clienteActual);
        uiManager.ActualizarUI();
    }

    public void BotonVender()
    {
        bool venta = ventaService.RealizarVenta(clienteActual);

        if (venta)
        {
            Producto producto = inventario.ObtenerProducto(clienteActual.ProductoPedido);
            dinero += producto.Precio;

            Debug.Log("Venta exitosa");
        }
        else
        {
            Debug.Log("No se pudo vender");
        }

        SiguienteCliente();
    }

    public void BotonRechazar()
    {
        Debug.Log("Cliente rechazado");
        SiguienteCliente();
    }

    void SiguienteCliente()
    {
        GenerarNuevoCliente();
    }
}