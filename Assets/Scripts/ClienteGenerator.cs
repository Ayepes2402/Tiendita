using System.Collections.Generic;
using UnityEngine;

public class ClienteGenerator
{
    public List<Cliente> GenerarPoolDeClientes()
    {
        List<Cliente> clientes = new List<Cliente>
        {
            new Cliente("Miguel", TipoCliente.Normal, "pan", 20,
                "Glup, glup, pan, glup, glup", 0),

            new Cliente("Jerónimo", TipoCliente.Sospechoso, "leche", 15,
                "Che, ¿me das por favor una bolsa de leche? es que tengo siete cafeteras en casa", 1),

            new Cliente("Bob", TipoCliente.Apurado, "huevos", 25,
                "¡Bello! Mi want huevo, tankiu", 2),

            new Cliente("Josh", TipoCliente.Normal, "pan", 18,
                "Katniss sobrevivió… yo sobreviví… ahora solo necesito leche.", 3),

            new Cliente("Cam", TipoCliente.Pobre, "leche", 10,
                "Sí, he subido un par de kilos mientras esperábamos al bebé... pero ¿me puedes dar leche porfi?", 4),

            new Cliente("Justin", TipoCliente.Normal, "huevos", 30,
                "BAAABY, dame un huevo, porfi", 5),

            new Cliente("Dona Mercedes", TipoCliente.Normal, "leche", 25,
                "Buenas. Quiero una leche, pero me la da en buen estado, ¿sí?", 6),

            new Cliente("Fercho", TipoCliente.Apurado, "pan", 20,
                "Regáleme ahí un pan, por favor", 7),

            new Cliente("Wisin", TipoCliente.Sospechoso, "pan", 22,
                "W, el machucapan. Mera, dame un pan", 8),

            new Cliente("Yandel", TipoCliente.Sospechoso, "huevos", 22,
                "Rakata rakata, dame, dame, dame huevo", 9)
        };

        MezclarLista(clientes);
        return clientes;
    }

    public List<Cliente> ObtenerClientesDelDia(List<Cliente> clientesTotales, int numeroDia, int clientesPorDia)
    {
        List<Cliente> clientesDelDia = new List<Cliente>();
        int inicio = (numeroDia - 1) * clientesPorDia;

        for (int i = inicio; i < inicio + clientesPorDia && i < clientesTotales.Count; i++)
        {
            clientesDelDia.Add(clientesTotales[i]);
        }

        return clientesDelDia;
    }

    private void MezclarLista(List<Cliente> lista)
    {
        for (int i = 0; i < lista.Count; i++)
        {
            int indiceAleatorio = Random.Range(i, lista.Count);
            Cliente temporal = lista[i];
            lista[i] = lista[indiceAleatorio];
            lista[indiceAleatorio] = temporal;
        }
    }
}
