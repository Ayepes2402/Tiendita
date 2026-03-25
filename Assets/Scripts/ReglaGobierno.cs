using System;


public class ReglaGobierno
{
    public string ProductoProhibido { get; private set; }

    public ReglaGobierno(string productoProhibido)
    {
        ProductoProhibido = productoProhibido;
    }

    public bool PuedeVender(string producto)
    {
        return producto != ProductoProhibido;
    }
}