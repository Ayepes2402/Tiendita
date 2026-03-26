using System.Collections.Generic;

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

    public List<Producto> ObtenerProductosPorCategoria(CategoriaProducto categoria)
    {
        List<Producto> productosFiltrados = new List<Producto>();

        foreach (Producto producto in productos)
        {
            if (producto.Categoria == categoria)
            {
                productosFiltrados.Add(producto);
            }
        }

        return productosFiltrados;
    }

    public bool TieneProductosEnCategoria(CategoriaProducto categoria)
    {
        foreach (Producto producto in productos)
        {
            if (producto.Categoria == categoria && producto.HayStock())
            {
                return true;
            }
        }

        return false;
    }
}
