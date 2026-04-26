using System;

[Serializable]
public class Cliente
{
    public string Nombre { get; private set; }
    public TipoCliente Tipo { get; private set; }
    public string ProductoPedido { get; private set; }
    public int Dinero { get; private set; }
    public string FrasePedido { get; private set; }
    public int SpriteIndex { get; private set; }

    // --- NUEVAS VARIABLES DE DIÁLOGO ---
    public string Opcion1 { get; private set; }
    public string Opcion2 { get; private set; }
    public string Respuesta1 { get; private set; }
    public string Respuesta2 { get; private set; }

    // Constructor básico (intacto)
    public Cliente(TipoCliente tipo, string productoPedido, int dinero)
        : this("Cliente", tipo, productoPedido, dinero,
               "Buenas, me das " + productoPedido + ".", -1)
    {
    }

    // Constructor principal (actualizado con opciones de diálogo)
    public Cliente(string nombre, TipoCliente tipo, string productoPedido,
                   int dinero, string frasePedido, int spriteIndex,
                   string op1 = "¡Claro!", string op2 = "No sé...",
                   string res1 = "Gracias.", string res2 = "Bueno...")
    {
        Nombre = nombre;
        Tipo = tipo;
        ProductoPedido = productoPedido;
        Dinero = dinero;
        FrasePedido = frasePedido;
        SpriteIndex = spriteIndex;

        // Guardamos los diálogos
        Opcion1 = op1;
        Opcion2 = op2;
        Respuesta1 = res1;
        Respuesta2 = res2;
    }
}