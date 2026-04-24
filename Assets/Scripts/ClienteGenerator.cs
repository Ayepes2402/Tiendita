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
                // DIA 1 (Prohibido: Leche).
                clientesDelDia.Add(CrearMiguel());
                clientesDelDia.Add(CrearMaluma());   // Nuevo (Pan - Legal)
                clientesDelDia.Add(CrearJustin());
                clientesDelDia.Add(CrearElon());     // Nuevo (Leche - TRAMPA)
                clientesDelDia.Add(CrearCam());
                break;
            case 2:
                // DIA 2 (Prohibido: Huevos).
                clientesDelDia.Add(CrearMessi());    // Nuevo (Leche - Legal)
                clientesDelDia.Add(CrearJeronimo(trampaPrincipal));
                clientesDelDia.Add(CrearFlorinda()); // Nuevo (Huevos - TRAMPA)
                clientesDelDia.Add(CrearFercho());
                clientesDelDia.Add(CrearDonaMercedes());
                break;
            case 3:
                // DIA 3 (Prohibido: Pan).
                clientesDelDia.Add(CrearWisin(trampaPrincipal));
                clientesDelDia.Add(CrearShakira());  // Nuevo (Pan - TRAMPA)
                clientesDelDia.Add(CrearJustin());
                clientesDelDia.Add(CrearElon());     // Nuevo (Leche - Legal)
                clientesDelDia.Add(CrearBob());
                break;
            case 4:
                // DIA 4 (Prohibido: Leche y Huevos).
                clientesDelDia.Add(CrearMaluma());   // Nuevo (Pan - Legal)
                clientesDelDia.Add(CrearYandel(trampaSecundaria));
                clientesDelDia.Add(CrearMessi());    // Nuevo (Leche - TRAMPA)
                clientesDelDia.Add(CrearJosh());
                clientesDelDia.Add(CrearFlorinda()); // Nuevo (Huevos - TRAMPA)
                break;
            case 5:
                // DIA 5 (Prohibido: Huevos y Pan).
                clientesDelDia.Add(CrearWisin(trampaPrincipal));
                clientesDelDia.Add(CrearShakira());  // Nuevo (Pan - TRAMPA)
                clientesDelDia.Add(CrearCam());
                clientesDelDia.Add(CrearElon());     // Nuevo (Leche - Legal)
                clientesDelDia.Add(CrearJeronimo(trampaPrincipal));
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

    // --- Nuevos Personajes ---
    private Cliente CrearMaluma() => new Cliente("Maluma", TipoCliente.Normal, "pan", 22, "Ey parcero, un pancito que hoy me siento 'Papi Juancho'.", 10);
    private Cliente CrearElon() => new Cliente("Elon", TipoCliente.Normal, "leche", 40, "¿Aceptas Dogecoin? No, mentira... dame una leche para ir a Marte.", 11);
    private Cliente CrearFlorinda() => new Cliente("Doña Florinda", TipoCliente.Apurado, "huevos", 25, "No me junte con la chusma... y deme una cubeta de huevos.", 12);
    private Cliente CrearMessi() => new Cliente("Messi", TipoCliente.Normal, "leche", 30, "Vení, dame una leche... ¿qué mirás bobo? Andá para allá.", 13);
    private Cliente CrearShakira() => new Cliente("Shakira", TipoCliente.Apurado, "pan", 28, "Las mujeres ya no lloran, las mujeres facturan... y comen pan.", 14);

    // --- Sospechosos ---
    private Cliente CrearJeronimo(string p) => new Cliente("Jerónimo", TipoCliente.Sospechoso, p, 15, "Che, ¿me das por favor " + p + "? es que tengo siete cafeteras en casa", 1);
    private Cliente CrearWisin(string p) => new Cliente("Wisin", TipoCliente.Sospechoso, p, 22, "W, el machucapan. Mera, dame " + p + " caleta", 8);
    private Cliente CrearYandel(string p) => new Cliente("Yandel", TipoCliente.Sospechoso, p, 22, "Rakata rakata, dame, dame, dame " + p, 9);
}