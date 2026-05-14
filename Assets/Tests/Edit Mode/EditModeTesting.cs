using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class EditModeTesting
{
    [Test]
    public void Producto_CreaValoresBaseYCategoriaPorDefecto()
    {
        Producto producto = new Producto("pan", 10, 5);

        Assert.AreEqual("pan", producto.Nombre);
        Assert.AreEqual(10, producto.Precio);
        Assert.AreEqual(5, producto.Cantidad);
        Assert.AreEqual(CategoriaProducto.Otro, producto.Categoria);
    }

    [Test]
    public void Producto_HayStock_ReflejaLaCantidad()
    {
        Producto conStock = new Producto("pan", 10, 1);
        Producto sinStock = new Producto("pan", 10, 0);

        Assert.IsTrue(conStock.HayStock());
        Assert.IsFalse(sinStock.HayStock());
    }

    [Test]
    public void Producto_ReducirStock_NoBajaDeCero()
    {
        Producto producto = new Producto("pan", 10, 1);

        producto.ReducirStock();
        producto.ReducirStock();

        Assert.AreEqual(0, producto.Cantidad);
    }

    [Test]
    public void Producto_AumentarStock_SumaCantidad()
    {
        Producto producto = new Producto("pan", 10, 3);

        producto.AumentarStock(4);

        Assert.AreEqual(7, producto.Cantidad);
    }

    [Test]
    public void Producto_EsDeCategoria_ComparaCorrectamente()
    {
        Producto producto = new Producto("leche", 15, 2, CategoriaProducto.Lacteo);

        Assert.IsTrue(producto.EsDeCategoria(CategoriaProducto.Lacteo));
        Assert.IsFalse(producto.EsDeCategoria(CategoriaProducto.Basico));
    }

    [Test]
    public void Inventario_TieneProducto_SoloSiExisteYHayStock()
    {
        List<Producto> productos = new List<Producto>
        {
            new Producto("pan", 10, 3),
            new Producto("leche", 12, 0)
        };

        Inventario inventario = new Inventario(productos);

        Assert.IsTrue(inventario.TieneProducto("pan"));
        Assert.IsFalse(inventario.TieneProducto("leche"));
        Assert.IsFalse(inventario.TieneProducto("huevos"));
    }

    [Test]
    public void Inventario_ObtenerProducto_RetornaNullSiNoExiste()
    {
        Inventario inventario = new Inventario(new List<Producto>());

        Assert.IsNull(inventario.ObtenerProducto("pan"));
    }

    [Test]
    public void Inventario_ObtenerProductosPorCategoria_FiltraCorrectamente()
    {
        Producto pan = new Producto("pan", 10, 3, CategoriaProducto.Basico);
        Producto leche = new Producto("leche", 12, 2, CategoriaProducto.Lacteo);
        Producto queso = new Producto("queso", 18, 1, CategoriaProducto.Lacteo);
        Inventario inventario = new Inventario(new List<Producto> { pan, leche, queso });

        List<Producto> lacteos = inventario.ObtenerProductosPorCategoria(CategoriaProducto.Lacteo);

        Assert.AreEqual(2, lacteos.Count);
        Assert.Contains(leche, lacteos);
        Assert.Contains(queso, lacteos);
        Assert.IsFalse(lacteos.Contains(pan));
    }

    [Test]
    public void Inventario_TieneProductosEnCategoria_RespetaElStock()
    {
        Inventario inventario = new Inventario(new List<Producto>
        {
            new Producto("pan", 10, 0, CategoriaProducto.Basico),
            new Producto("leche", 12, 2, CategoriaProducto.Lacteo)
        });

        Assert.IsFalse(inventario.TieneProductosEnCategoria(CategoriaProducto.Basico));
        Assert.IsTrue(inventario.TieneProductosEnCategoria(CategoriaProducto.Lacteo));
    }

    [Test]
    public void VentaService_RealizarVenta_ReduceStockCuandoHayProducto()
    {
        Producto producto = new Producto("pan", 10, 2);
        Inventario inventario = new Inventario(new List<Producto> { producto });
        VentaService ventaService = new VentaService(inventario);
        Cliente cliente = new Cliente(TipoCliente.Normal, "pan", 20);

        bool resultado = ventaService.RealizarVenta(cliente);

        Assert.IsTrue(resultado);
        Assert.AreEqual(1, producto.Cantidad);
    }

    [Test]
    public void VentaService_RealizarVenta_FalseSiNoExisteProducto()
    {
        Inventario inventario = new Inventario(new List<Producto>());
        VentaService ventaService = new VentaService(inventario);
        Cliente cliente = new Cliente(TipoCliente.Normal, "pan", 20);

        Assert.IsFalse(ventaService.RealizarVenta(cliente));
    }

    [Test]
    public void ReglaGobierno_PuedeVender_RespetaListaProhibida()
    {
        ReglaGobierno regla = new ReglaGobierno(new List<string> { "pan", "leche" });

        Assert.IsFalse(regla.PuedeVender("pan"));
        Assert.IsFalse(regla.PuedeVender("PAN"));
        Assert.IsTrue(regla.PuedeVender("huevos"));
    }

    [Test]
    public void ReglaGobierno_ObtenerListaProhibida_DevuelveTextoEsperado()
    {
        ReglaGobierno sinProhibidos = new ReglaGobierno(new List<string>());
        ReglaGobierno conProhibidos = new ReglaGobierno(new List<string> { "pan", "leche" });

        Assert.AreEqual("Nada", sinProhibidos.ObtenerListaProhibida());
        Assert.AreEqual("pan y leche", conProhibidos.ObtenerListaProhibida());
    }

    [Test]
    public void EventoAleatorio_GenerarProductoProhibido_RetornaProductoExistenteOAjoVacio()
    {
        EventoAleatorio evento = new EventoAleatorio();
        Inventario inventario = new Inventario(new List<Producto>
        {
            new Producto("pan", 10, 1),
            new Producto("leche", 12, 1),
            new Producto("huevos", 15, 1)
        });

        Random.InitState(1234);
        string resultado = evento.GenerarProductoProhibido(inventario);

        Assert.IsNotEmpty(resultado);
        Assert.Contains(resultado, new List<string> { "pan", "leche", "huevos" });
        Assert.AreEqual(string.Empty, evento.GenerarProductoProhibido(null));
        Assert.AreEqual(string.Empty, evento.GenerarProductoProhibido(new Inventario(new List<Producto>())));
    }

    [Test]
    public void Cliente_CreaValoresCorrectos()
    {
        Cliente cliente = new Cliente(TipoCliente.Sospechoso, "leche", 35);

        Assert.AreEqual("Cliente", cliente.Nombre);
        Assert.AreEqual(TipoCliente.Sospechoso, cliente.Tipo);
        Assert.AreEqual("leche", cliente.ProductoPedido);
        Assert.AreEqual(35, cliente.Dinero);
        Assert.IsTrue(cliente.FrasePedido.Contains("leche"));
        Assert.AreEqual(-1, cliente.SpriteIndex);
    }
}
