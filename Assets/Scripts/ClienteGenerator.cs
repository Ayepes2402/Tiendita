using UnityEngine;

public class ClienteGenerator
{
    public Cliente GenerarCliente()
    {
        TipoCliente tipo = (TipoCliente)Random.Range(0, 4);

        string[] productos = { "pan", "leche", "huevos" };

        string producto = productos[Random.Range(0, productos.Length)];

        int dinero = Random.Range(5, 30);

        return new Cliente(tipo, producto, dinero);
    }
}