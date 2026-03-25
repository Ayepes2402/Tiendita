using System.Collections.Generic;

using UnityEngine;



public class Inventario
{
    private List<Producto> productos;

    public Inventario(List<Producto> productosIniciales)
    {
        productos = productosIniciales;
    }

    public bool TieneProducto(string nombre)
    {
        foreach (Producto p in productos)
        {
            if (p.Nombre == nombre && p.HayStock())
                return true;
        }

        return false;
    }

    public Producto ObtenerProducto(string nombre)
    {
        foreach (Producto p in productos)
        {
            if (p.Nombre == nombre)
                return p;
        }

        return null;
    }

    public List<Producto> ObtenerProductos()
    {
        return productos;
    }
}