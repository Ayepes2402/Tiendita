using System.Collections.Generic;
using UnityEngine;

public class EventoAleatorio
{
    public string GenerarProductoProhibido(Inventario inventario)
    {
        if (inventario == null)
        {
            return "";
        }

        List<Producto> productos = inventario.ObtenerProductos();

        if (productos == null || productos.Count == 0)
        {
            return "";
        }

        int indice = Random.Range(0, productos.Count);
        return productos[indice].Nombre;
    }
}
