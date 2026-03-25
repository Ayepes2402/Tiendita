using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class EditModeTesting
{

    [Test]
    public void Producto_HayStock_TrueSiCantidadMayorACero()
    {
        Producto p = new Producto("pan", 10, 5);
        Assert.IsTrue(p.HayStock());
    }

    [Test]
    public void Producto_HayStock_FalseSiCantidadCero()
    {
        Producto p = new Producto("pan", 10, 0);
        Assert.IsFalse(p.HayStock());
    }

    [Test]
    public void Producto_ReducirStock_DisminuyeCantidad()
    {
        Producto p = new Producto("pan", 10, 5);
        p.ReducirStock();
        Assert.AreEqual(4, p.Cantidad);
    }

    [Test]
    public void Producto_ReducirStock_NoBajaDeCero()
    {
        Producto p = new Producto("pan", 10, 0);
        p.ReducirStock();
        Assert.AreEqual(0, p.Cantidad);
    }

    [Test]
    public void Inventario_TieneProducto_TrueSiExisteYHayStock()
    {
        List<Producto> lista = new List<Producto>()
        {
            new Producto("pan",10,5)
        };

        Inventario inv = new Inventario(lista);

        Assert.IsTrue(inv.TieneProducto("pan"));
    }

    [Test]
    public void Inventario_TieneProducto_FalseSiNoExiste()
    {
        List<Producto> lista = new List<Producto>()
        {
            new Producto("pan",10,5)
        };

        Inventario inv = new Inventario(lista);

        Assert.IsFalse(inv.TieneProducto("leche"));
    }

    [Test]
    public void Inventario_TieneProducto_FalseSiSinStock()
    {
        List<Producto> lista = new List<Producto>()
        {
            new Producto("pan",10,0)
        };

        Inventario inv = new Inventario(lista);

        Assert.IsFalse(inv.TieneProducto("pan"));
    }

    [Test]
    public void Inventario_ObtenerProducto_RetornaCorrecto()
    {
        Producto prod = new Producto("pan", 10, 5);

        List<Producto> lista = new List<Producto>() { prod };

        Inventario inv = new Inventario(lista);

        Assert.AreEqual(prod, inv.ObtenerProducto("pan"));
    }

    [Test]
    public void Inventario_ObtenerProducto_RetornaNullSiNoExiste()
    {
        Inventario inv = new Inventario(new List<Producto>());

        Assert.IsNull(inv.ObtenerProducto("pan"));
    }


    [Test]
    public void Cliente_SeCreaCorrectamente()
    {
        Cliente c = new Cliente(TipoCliente.Normal, "pan", 20);

        Assert.AreEqual("pan", c.ProductoPedido);
        Assert.AreEqual(20, c.Dinero);
        Assert.AreEqual(TipoCliente.Normal, c.Tipo);
    }


    [Test]
    public void VentaService_VentaExitosa()
    {
        Producto p = new Producto("pan", 10, 5);
        Inventario inv = new Inventario(new List<Producto>() { p });
        VentaService vs = new VentaService(inv);

        Cliente c = new Cliente(TipoCliente.Normal, "pan", 20);

        bool resultado = vs.RealizarVenta(c);

        Assert.IsTrue(resultado);
        Assert.AreEqual(4, p.Cantidad);
    }

    [Test]
    public void VentaService_FallaSiNoHayProducto()
    {
        Inventario inv = new Inventario(new List<Producto>());
        VentaService vs = new VentaService(inv);

        Cliente c = new Cliente(TipoCliente.Normal, "pan", 20);

        Assert.IsFalse(vs.RealizarVenta(c));
    }

    [Test]
    public void VentaService_FallaSiNoHayStock()
    {
        Producto p = new Producto("pan", 10, 0);
        Inventario inv = new Inventario(new List<Producto>() { p });
        VentaService vs = new VentaService(inv);

        Cliente c = new Cliente(TipoCliente.Normal, "pan", 20);

        Assert.IsFalse(vs.RealizarVenta(c));
    }

    [Test]
    public void VentaService_FallaSiNoTieneDinero()
    {
        Producto p = new Producto("pan", 10, 5);
        Inventario inv = new Inventario(new List<Producto>() { p });
        VentaService vs = new VentaService(inv);

        Cliente c = new Cliente(TipoCliente.Normal, "pan", 5);

        Assert.IsFalse(vs.RealizarVenta(c));
    }

    [Test]
    public void ReglaGobierno_NoPermiteProductoProhibido()
    {
        ReglaGobierno regla = new ReglaGobierno("pan");

        Assert.IsFalse(regla.PuedeVender("pan"));
    }

    [Test]
    public void ReglaGobierno_PermiteProductoDiferente()
    {
        ReglaGobierno regla = new ReglaGobierno("pan");

        Assert.IsTrue(regla.PuedeVender("leche"));
    }

    [Test]
    public void EventoAleatorio_GeneraEventoValido()
    {
        EventoAleatorio evento = new EventoAleatorio();

        TipoEvento resultado = evento.GenerarEvento();

        Assert.IsTrue(
            resultado == TipoEvento.Robo ||
            resultado == TipoEvento.Propina ||
            resultado == TipoEvento.Nada
        );
    }

    [Test]
    public void ClienteGenerator_GeneraClienteValido()
    {
        ClienteGenerator gen = new ClienteGenerator();

        Cliente c = gen.GenerarCliente();

        Assert.IsNotNull(c);
        Assert.IsNotNull(c.ProductoPedido);
        Assert.IsTrue(c.Dinero >= 5 && c.Dinero <= 30);
    }
}