using UnityEngine;

public class Producto
{
    public string Nombre { get; private set; }
    public int Precio { get; private set; }
    public int Cantidad { get; set; }
    public CategoriaProducto Categoria { get; private set; }

    public Producto(string nombre, int precio, int cantidad)
        : this(nombre, precio, cantidad, CategoriaProducto.Otro)
    {
    }

    public Producto(string nombre, int precio, int cantidad, CategoriaProducto categoria)
    {
        Nombre = nombre;
        Precio = precio;
        Cantidad = cantidad;
        Categoria = categoria;
    }

    public bool HayStock()
    {
        return Cantidad > 0;
    }

    public void ReducirStock()
    {
        if (Cantidad > 0)
        {
            Cantidad--;
        }
    }
    public void AumentarStock(int cantidadComprada)
    {
        Cantidad += cantidadComprada;
    }
    public bool EsDeCategoria(CategoriaProducto categoria)
    {
        return Categoria == categoria;
    }
}
