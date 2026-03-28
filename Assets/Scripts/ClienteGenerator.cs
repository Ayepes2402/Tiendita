using System.Collections.Generic;
using UnityEngine;

public class ClienteGenerator
{
    public List<Cliente> GenerarPoolDeClientes()
    {
        List<Cliente> clientes = new List<Cliente>
        {
            new Cliente("Dona Gloria", "Amable", TipoCliente.Normal, "pan", 20,
                "Mijo, regálame un pancito, por favor.", 0),

            new Cliente("Don Jairo", "Regateador", TipoCliente.Sospechoso, "leche", 15,
                "Veci, hagame el cruce y me vende una leche, pues.", 1),

            new Cliente("Sara", "Universitaria afanada", TipoCliente.Apurado, "huevos", 25,
                "Hola, estoy súper corrida, me das unos huevos rapidito, porfa.", 2),

            new Cliente("Yuli", "Chismosa", TipoCliente.Normal, "pan", 18,
                "Ay, veci, mientras le cuento... me empaca un pan, ¿sí?", 3),

            new Cliente("Don Efrain", "Tacanio", TipoCliente.Pobre, "leche", 10,
                "No me vaya a tumbar, ¿oyó? Véndame una leche bien barata.", 4),

            new Cliente("Kevin", "Relajado", TipoCliente.Normal, "huevos", 30,
                "Quiubo, mi llave, me da unos huevos ahí bien melos.", 5),

            new Cliente("Dona Mercedes", "Exigente", TipoCliente.Normal, "leche", 25,
                "Buenas. Quiero una leche, pero me la da en buen estado, ¿sí?", 6),

            new Cliente("Fercho", "Motero directo", TipoCliente.Apurado, "pan", 20,
                "Parcero, de una, véndame un pan que voy volando.", 7),

            new Cliente("Lina", "Muy fina", TipoCliente.Normal, "huevos", 35,
                "Hola, ¿me regalas unos huevitos, por favor? Gracias.", 8),

            new Cliente("Don Anibal", "Desconfiado", TipoCliente.Sospechoso, "leche", 22,
                "A ver, socio, muéstreme bien esa leche y ahí sí se la pago.", 9)
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
