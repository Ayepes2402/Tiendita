using System.Collections.Generic;
using UnityEngine;

public class ClienteGenerator
{
    public List<Cliente> GenerarPoolDeClientes() => new List<Cliente>();

    public List<Cliente> ObtenerClientesDelDia(int numeroDia, List<string> productosProhibidos)
    {
        List<Cliente> clientesDelDia = new List<Cliente>();

        string trampaPrincipal = productosProhibidos.Count > 0 ? productosProhibidos[0] : "pan";
        string trampaSecundaria = productosProhibidos.Count > 1 ? productosProhibidos[1] : trampaPrincipal;

        switch (numeroDia)
        {
            case 1:
                // DIA 1 (Prohibido: Leche). Solo 1 cliente pide leche, los demás dan plata.
                clientesDelDia.Add(CrearMiguel()); // Pide Pan (Legal)
                clientesDelDia.Add(CrearJosh());   // Pide Pan (Legal)
                clientesDelDia.Add(CrearJustin()); // Pide Huevos (Legal)
                clientesDelDia.Add(CrearFercho()); // Pide Pan (Legal)
                clientesDelDia.Add(CrearCam());    // Pide Leche (Prohibido - Trampa normal)
                break;
            case 2:
                // DIA 2 (Prohibido: Huevos). Puros clientes de pan y leche.
                clientesDelDia.Add(CrearMiguel()); // Pide Pan (Legal)
                clientesDelDia.Add(CrearJeronimo(trampaPrincipal)); // Trampa: Pide Huevos
                clientesDelDia.Add(CrearDonaMercedes()); // Pide Leche (Legal)
                clientesDelDia.Add(CrearFercho()); // Pide Pan (Legal)
                clientesDelDia.Add(CrearCam()); // Pide Leche (Legal)
                break;
            case 3:
                // DIA 3 (Prohibido: Pan).
                clientesDelDia.Add(CrearWisin(trampaPrincipal)); // Trampa: Pide Pan
                clientesDelDia.Add(CrearDonaMercedes()); // Pide Leche (Legal)
                clientesDelDia.Add(CrearJustin()); // Pide Huevos (Legal)
                clientesDelDia.Add(CrearBob()); // Pide Huevos (Legal)
                clientesDelDia.Add(CrearCam()); // Pide Leche (Legal)
                break;
            case 4:
                // DIA 4 (Prohibido: Leche y Huevos). Solo el pan es seguro.
                clientesDelDia.Add(CrearJeronimo(trampaPrincipal)); // Trampa Leche
                clientesDelDia.Add(CrearYandel(trampaSecundaria)); // Trampa Huevos
                clientesDelDia.Add(CrearMiguel()); // Pide Pan (Legal)
                clientesDelDia.Add(CrearJosh()); // Pide Pan (Legal)
                clientesDelDia.Add(CrearFercho()); // Pide Pan (Legal)
                break;
            case 5:
                // DIA 5 (Prohibido: Huevos y Pan). Solo la leche es segura.
                clientesDelDia.Add(CrearWisin(trampaPrincipal)); // Trampa Huevos
                clientesDelDia.Add(CrearYandel(trampaSecundaria)); // Trampa Pan
                clientesDelDia.Add(CrearCam()); // Pide Leche (Legal)
                clientesDelDia.Add(CrearDonaMercedes()); // Pide Leche (Legal)
                clientesDelDia.Add(CrearJeronimo(trampaPrincipal)); // Trampa Huevos
                break;
        }
        return clientesDelDia;
    }

    // --- Personajes Originales ---
    private Cliente CrearMiguel() => new Cliente("Miguel", TipoCliente.Normal, "pan", 20, "Glup, glup, pan, glup, glup", 0);
    private Cliente CrearBob() => new Cliente("Bob", TipoCliente.Apurado, "huevos", 25, "¡Bello! Mi want huevo, tankiu", 2);
    private Cliente CrearJosh() => new Cliente("Josh", TipoCliente.Normal, "pan", 18, "Katniss sobrevivió… yo sobreviví… ahora solo necesito pan.", 3);
    private Cliente CrearCam() => new Cliente("Cam", TipoCliente.Pobre, "leche", 10, "Sí, he subido un par de kilos... pero ¿me puedes dar leche porfi?", 4);
    private Cliente CrearJustin() => new Cliente("Justin", TipoCliente.Normal, "huevos", 30, "BAAABY, dame un huevo, porfi", 5);
    private Cliente CrearDonaMercedes() => new Cliente("Dona Mercedes", TipoCliente.Normal, "leche", 25, "Buenas. Quiero una leche, pero me la da en buen estado, ¿sí?", 6);
    private Cliente CrearFercho() => new Cliente("Fercho", TipoCliente.Apurado, "pan", 20, "Regáleme ahí un pan, por favor", 7);

    // --- Sospechosos ---
    private Cliente CrearJeronimo(string p) => new Cliente("Jerónimo", TipoCliente.Sospechoso, p, 15, "Che, ¿me das por favor " + p + "? es que tengo siete cafeteras en casa", 1);
    private Cliente CrearWisin(string p) => new Cliente("Wisin", TipoCliente.Sospechoso, p, 22, "W, el machucapan. Mera, dame " + p + " caleta", 8);
    private Cliente CrearYandel(string p) => new Cliente("Yandel", TipoCliente.Sospechoso, p, 22, "Rakata rakata, dame, dame, dame " + p, 9);
}