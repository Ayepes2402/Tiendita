using System;
using UnityEngine;

public class VentaService
{
    private Inventario inventario;

    public VentaService(Inventario inventario)
    {
        this.inventario = inventario;
    }

    public bool RealizarVenta(Cliente cliente)
    {
        if (!inventario.TieneProducto(cliente.ProductoPedido))
            return false;

        Producto producto = inventario.ObtenerProducto(cliente.ProductoPedido);

        // Se ignora el chequeo de dinero del cliente según tu última modificación.
        producto.ReducirStock();
        return true;
    }
}