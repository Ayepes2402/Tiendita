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
                clientesDelDia.Add(CrearMiguel());
                clientesDelDia.Add(CrearCam());
                clientesDelDia.Add(CrearDonaMercedes());
                clientesDelDia.Add(CrearJosh());
                clientesDelDia.Add(CrearFercho());
                break;

            case 2: 
                clientesDelDia.Add(CrearMaluma());
                clientesDelDia.Add(CrearJeronimo(trampaPrincipal));
                clientesDelDia.Add(CrearMessi());
                clientesDelDia.Add(CrearMiguel());
                clientesDelDia.Add(CrearFlorinda());
                clientesDelDia.Add(CrearJustin());
                clientesDelDia.Add(CrearBob());
              
                break;

            case 3: // Día 3: 
                clientesDelDia.Add(CrearWisin(trampaPrincipal));
                clientesDelDia.Add(CrearShakira());
              
                clientesDelDia.Add(CrearElon());
                clientesDelDia.Add(CrearCam());
                clientesDelDia.Add(CrearDonaMercedes());
                clientesDelDia.Add(CrearFercho());
                clientesDelDia.Add(CrearJosh());
                clientesDelDia.Add(CrearBob());
                break;

            case 4: // Día 4: 
                clientesDelDia.Add(CrearMaluma());
                clientesDelDia.Add(CrearYandel(trampaSecundaria));
                clientesDelDia.Add(CrearJustin());
                clientesDelDia.Add(CrearJeronimo(trampaPrincipal));
                clientesDelDia.Add(CrearFlorinda());
                clientesDelDia.Add(CrearMessi());
                clientesDelDia.Add(CrearShakira());
            
                clientesDelDia.Add(CrearMiguel());
               
                break;

            case 5: // 
                clientesDelDia.Add(CrearElon());
                clientesDelDia.Add(CrearWisin(trampaPrincipal));
                clientesDelDia.Add(CrearShakira());
                clientesDelDia.Add(CrearBob());
                clientesDelDia.Add(CrearYandel(trampaSecundaria));
                clientesDelDia.Add(CrearMaluma());
                clientesDelDia.Add(CrearJeronimo(trampaPrincipal));
                clientesDelDia.Add(CrearDonaMercedes());
                clientesDelDia.Add(CrearMessi());
                clientesDelDia.Add(CrearFercho());
                clientesDelDia.Add(CrearJustin());
                break;
        }
        return clientesDelDia;
    }

    // --- Personajes Originales (Con opciones de respuesta) ---
    private Cliente CrearMiguel() => new Cliente("Miguel", TipoCliente.Normal, "pan", 20, "Glup, glup, pan, glup, glup", 0,
        "Toma tu pan acuático", "¿Por qué haces ese ruido?", "¡Glup, gracias!", "Glup... no me entiendes.");

    private Cliente CrearBob() => new Cliente("Bob", TipoCliente.Apurado, "huevos", 25, "¡Bello! Mi want huevo, tankiu", 2,
        "¡Bananas y huevos listos!", "No hablo Minion", "¡Papaguena! Jeje.", "¡Fuchi! Qué aburrido.");

    private Cliente CrearJosh() => new Cliente("Josh", TipoCliente.Normal, "pan", 18, "Katniss sobrevivió… yo sobreviví… ahora solo necesito pan.", 3,
        "Pan de supervivencia listo.", "Los juegos del hambre acabaron.", "Tengo esperanza de nuevo.", "El Capitolio te vigila.");

    private Cliente CrearCam() => new Cliente("Cam", TipoCliente.Pobre, "leche", 10, "Sí, he subido un par de kilos... pero ¿me puedes dar leche porfi?", 4,
        "Estás perfecto así.", "Ponte a hacer ejercicio.", "¡Aww, eres muy amable!", "¡Qué grosero! Igual dame leche.");

    private Cliente CrearJustin() => new Cliente("Justin", TipoCliente.Normal, "huevos", 30, "BAAABY, dame un huevo, porfi", 5,
        "Baby, baby, toma.", "Canta otra cosa primero.", "You know you love me.", "Hater... dámelo rápido.");

    private Cliente CrearDonaMercedes() => new Cliente("Dona Mercedes", TipoCliente.Normal, "leche", 25, "Buenas. Quiero una leche, pero me la da en buen estado, ¿sí?", 6,
        "Fresquita, doña.", "Revisela usted misma.", "Dios te pague, mijo.", "Mmm, más le vale jovencito.");

    private Cliente CrearFercho() => new Cliente("Fercho", TipoCliente.Apurado, "pan", 20, "Regáleme ahí un pan, por favor", 7,
        "De una, mi so.", "Espere su turno.", "¡Melo caramelo!", "Qué demora, parce...");

    // --- Nuevos Personajes ---
    private Cliente CrearMaluma() => new Cliente("Maluma", TipoCliente.Normal, "pan", 22, "Ey parcero, un pancito que hoy me siento 'Papi Juancho'.", 10,
        "Pan pa'l Pretty Boy.", "Bájale al ego.", "¡Mua! Las nenas lo aprueban.", "Uy quieto, qué agresividad.");

    private Cliente CrearElon() => new Cliente("Elon", TipoCliente.Normal, "leche", 40, "¿Aceptas Dogecoin? No, mentira... dame una leche para ir a Marte.", 11,
        "Listos para el despegue.", "Solo acepto billetes.", "¡To the moon! 🚀", "Compraré esta tienda mañana.");

    private Cliente CrearFlorinda() => new Cliente("Doña Florinda", TipoCliente.Apurado, "huevos", 25, "No me junte con la chusma... y deme una cubeta de huevos.", 12,
        "Pase por acá, señora.", "Todos somos iguales aquí.", "Qué tiendita tan decente.", "¡Tesoro, vámonos de aquí!");

    private Cliente CrearMessi() => new Cliente("Messi", TipoCliente.Normal, "leche", 30, "Vení, dame una leche... ¿qué mirás bobo? Andá para allá.", 13,
        "Toma, campeón del mundo.", "No me hable así.", "Gracias, coronamos.", "Perdoná, es la costumbre.");

    private Cliente CrearShakira() => new Cliente("Shakira", TipoCliente.Apurado, "pan", 28, "Las mujeres ya no lloran, las mujeres facturan... y comen pan.", 14,
        "Claro, loba. Toma tu pan.", "Salpique de aquí.", "¡Auuu! Gracias.", "Te felicito, qué bien actúas.");

    // --- Sospechosos ---
    private Cliente CrearJeronimo(string p) => new Cliente("Jerónimo", TipoCliente.Sospechoso, p, 15, "Che, ¿me das por favor " + p + "? es que tengo siete cafeteras en casa", 1,
        "Raro, pero toma.", "Llamaré a la policía.", "¡Sos un crack!", "Che, no seas ortiva.");

    private Cliente CrearWisin(string p) => new Cliente("Wisin", TipoCliente.Sospechoso, p, 22, "W, el machucapan. Mera, dame " + p + " caleta", 8,
        "Tome su caleta.", "Hable bien, señor.", "Doble U y Yandel, duro.", "Afloja, afloja.");

    private Cliente CrearYandel(string p) => new Cliente("Yandel", TipoCliente.Sospechoso, p, 22, "Rakata rakata, dame, dame, dame " + p, 9,
        "Rakata, tome.", "Mucho ruido aquí.", "¡Los extraterrestres!", "Nos fuimos al garete.");
}