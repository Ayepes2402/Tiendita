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

        
        producto.ReducirStock();
        return true;
    }
}