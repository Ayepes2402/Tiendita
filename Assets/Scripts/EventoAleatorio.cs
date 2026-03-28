using System.Collections.Generic;
using UnityEngine;

public class EventoAleatorio
{
    public TipoEvento GenerarEvento(Inventario inventario)
    {
        List<TipoEvento> eventosPosibles = new List<TipoEvento>
        {
            TipoEvento.Robo,
            TipoEvento.Propina,
            TipoEvento.Nada,
            TipoEvento.InspeccionSanitaria,
            TipoEvento.MultaPorDesorden
        };

        if (inventario != null)
        {
            bool hayProductoConPocoStock = false;
            bool hayProductoConMuchoStock = false;

            foreach (Producto producto in inventario.ObtenerProductos())
            {
                if (producto.Cantidad > 0 && producto.Cantidad <= 2)
                {
                    hayProductoConPocoStock = true;
                }

                if (producto.Cantidad >= 5)
                {
                    hayProductoConMuchoStock = true;
                }
            }

            if (hayProductoConPocoStock)
            {
                eventosPosibles.Add(TipoEvento.ClienteMolestoPorFaltaDeStock);
            }

            if (hayProductoConMuchoStock)
            {
                eventosPosibles.Add(TipoEvento.DescuentoPorProductoPorVencer);
                eventosPosibles.Add(TipoEvento.BonificacionProveedor);
            }
        }

        int indice = Random.Range(0, eventosPosibles.Count);
        return eventosPosibles[indice];
    }
}