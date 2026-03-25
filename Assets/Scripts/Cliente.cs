using System;

public class Cliente
{
    public TipoCliente Tipo { get; private set; }
    public string ProductoPedido { get; private set; }
    public int Dinero { get; private set; }

    public Cliente(TipoCliente tipo, string productoPedido, int dinero)
    {
        Tipo = tipo;
        ProductoPedido = productoPedido;
        Dinero = dinero;
    }
}