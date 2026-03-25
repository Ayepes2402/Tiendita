using System;

using UnityEngine;

public class Producto
{
    public string Nombre { get; private set; }
    public int Precio { get; private set; }
    public int Cantidad { get; private set; }

    public Producto(string nombre, int precio, int cantidad)
    {
        Nombre = nombre;
        Precio = precio;
        Cantidad = cantidad;
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
}