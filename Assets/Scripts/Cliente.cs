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

    public Cliente(TipoCliente tipo, string productoPedido, int dinero)
        : this("Cliente", tipo, productoPedido, dinero,
               "Buenas, me das " + productoPedido + ".", -1)
    {
    }

    public Cliente(string nombre, TipoCliente tipo, string productoPedido,
                   int dinero, string frasePedido, int spriteIndex)
    {
        Nombre = nombre;
        Tipo = tipo;
        ProductoPedido = productoPedido;
        Dinero = dinero;
        FrasePedido = frasePedido;
        SpriteIndex = spriteIndex;
    }
}