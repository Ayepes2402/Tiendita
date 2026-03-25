using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private Inventario inventario;
    private VentaService ventaService;
    private EventoAleatorio eventoSistema;

    public int dinero = 0;
    public int dia = 1;
    public int deuda = 200;

    void Start()
    {
        // Crear productos iniciales
        List<Producto> productos = new List<Producto>();

        productos.Add(new Producto("pan", 10, 10));
        productos.Add(new Producto("leche", 12, 5));
        productos.Add(new Producto("huevos", 15, 6));

        // Crear sistemas del juego
        inventario = new Inventario(productos);
        ventaService = new VentaService(inventario);
        eventoSistema = new EventoAleatorio();

        Debug.Log("Comienza el día " + dia);

        // CLIENTE DE PRUEBA
        Cliente clientePrueba = new Cliente(TipoCliente.Normal, "pan", 20);

        AtenderCliente(clientePrueba);

        FinDelDia();
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

        // Generar evento aleatorio
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
}