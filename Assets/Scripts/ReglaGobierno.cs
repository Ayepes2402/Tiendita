using System;
using System.Collections.Generic;

public class ReglaGobierno
{
    private List<string> productosProhibidos;

    public ReglaGobierno(List<string> productos)
    {
        productosProhibidos = productos;
    }

    public bool PuedeVender(string producto)
    {
        
        foreach (string prohibido in productosProhibidos)
        {
            if (string.Equals(producto, prohibido, StringComparison.OrdinalIgnoreCase))
                return false;
        }
        return true;
    }

    public string ObtenerListaProhibida()
    {
        if (productosProhibidos.Count == 0) return "Nada";
        return string.Join(" y ", productosProhibidos);
    }
}