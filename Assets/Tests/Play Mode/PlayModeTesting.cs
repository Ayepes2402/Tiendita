using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;

public class PlayModeTesting
{

    [UnityTest]
    public IEnumerator Producto_HayStock_True()
    {
        Producto p = new Producto("pan", 10, 5);
        yield return null;
        Assert.IsTrue(p.HayStock());
    }

    [UnityTest]
    public IEnumerator Producto_HayStock_False()
    {
        Producto p = new Producto("pan", 10, 0);
        yield return null;
        Assert.IsFalse(p.HayStock());
    }

    [UnityTest]
    public IEnumerator Producto_ReducirStock()
    {
        Producto p = new Producto("pan", 10, 3);
        p.ReducirStock();
        yield return null;
        Assert.AreEqual(2, p.Cantidad);
    }

    [UnityTest]
    public IEnumerator Producto_NoBajaDeCero()
    {
        Producto p = new Producto("pan", 10, 0);
        p.ReducirStock();
        yield return null;
        Assert.AreEqual(0, p.Cantidad);
    }


    [UnityTest]
    public IEnumerator Inventario_TieneProducto_True()
    {
        var lista = new List<Producto>()
        {
            new Producto("pan",10,5)
        };

        Inventario inv = new Inventario(lista);

        yield return null;

        Assert.IsTrue(inv.TieneProducto("pan"));
    }

    [UnityTest]
    public IEnumerator Inventario_TieneProducto_False_NoExiste()
    {
        Inventario inv = new Inventario(new List<Producto>());
        yield return null;
        Assert.IsFalse(inv.TieneProducto("leche"));
    }

    [UnityTest]
    public IEnumerator Inventario_TieneProducto_False_SinStock()
    {
        var lista = new List<Producto>()
        {
            new Producto("pan",10,0)
        };

        Inventario inv = new Inventario(lista);

        yield return null;

        Assert.IsFalse(inv.TieneProducto("pan"));
    }

    [UnityTest]
    public IEnumerator Inventario_ObtenerProducto()
    {
        Producto p = new Producto("pan", 10, 5);
        Inventario inv = new Inventario(new List<Producto>() { p });

        yield return null;

        Assert.AreEqual(p, inv.ObtenerProducto("pan"));
    }

    [UnityTest]
    public IEnumerator Inventario_ObtenerProducto_Null()
    {
        Inventario inv = new Inventario(new List<Producto>());
        yield return null;
        Assert.IsNull(inv.ObtenerProducto("pan"));
    }

    [UnityTest]
    public IEnumerator Cliente_CreacionCorrecta()
    {
        Cliente c = new Cliente(TipoCliente.Normal, "pan", 20);

        yield return null;

        Assert.AreEqual("pan", c.ProductoPedido);
        Assert.AreEqual(20, c.Dinero);
        Assert.AreEqual(TipoCliente.Normal, c.Tipo);
    }

    [UnityTest]
    public IEnumerator VentaService_VentaExitosa()
    {
        Producto p = new Producto("pan", 10, 5);
        Inventario inv = new Inventario(new List<Producto>() { p });
        VentaService vs = new VentaService(inv);

        Cliente c = new Cliente(TipoCliente.Normal, "pan", 20);

        bool resultado = vs.RealizarVenta(c);

        yield return null;

        Assert.IsTrue(resultado);
        Assert.AreEqual(4, p.Cantidad);
    }

    [UnityTest]
    public IEnumerator VentaService_Falla_SinProducto()
    {
        Inventario inv = new Inventario(new List<Producto>());
        VentaService vs = new VentaService(inv);

        Cliente c = new Cliente(TipoCliente.Normal, "pan", 20);

        yield return null;

        Assert.IsFalse(vs.RealizarVenta(c));
    }

    [UnityTest]
    public IEnumerator VentaService_Falla_SinStock()
    {
        Producto p = new Producto("pan", 10, 0);
        Inventario inv = new Inventario(new List<Producto>() { p });
        VentaService vs = new VentaService(inv);

        Cliente c = new Cliente(TipoCliente.Normal, "pan", 20);

        yield return null;

        Assert.IsFalse(vs.RealizarVenta(c));
    }

    [UnityTest]
    public IEnumerator VentaService_Falla_SinDinero()
    {
        Producto p = new Producto("pan", 10, 5);
        Inventario inv = new Inventario(new List<Producto>() { p });
        VentaService vs = new VentaService(inv);

        Cliente c = new Cliente(TipoCliente.Normal, "pan", 5);

        yield return null;

        Assert.IsFalse(vs.RealizarVenta(c));
    }

    [UnityTest]
    public IEnumerator ReglaGobierno_NoPermite()
    {
        ReglaGobierno regla = new ReglaGobierno("pan");
        yield return null;
        Assert.IsFalse(regla.PuedeVender("pan"));
    }

    [UnityTest]
    public IEnumerator ReglaGobierno_Permite()
    {
        ReglaGobierno regla = new ReglaGobierno("pan");
        yield return null;
        Assert.IsTrue(regla.PuedeVender("leche"));
    }

    [UnityTest]
    public IEnumerator EventoAleatorio_EventoValido()
    {
        EventoAleatorio evento = new EventoAleatorio();
        TipoEvento resultado = evento.GenerarEvento();

        yield return null;

        Assert.IsTrue(
            resultado == TipoEvento.Robo ||
            resultado == TipoEvento.Propina ||
            resultado == TipoEvento.Nada
        );
    }

    [UnityTest]
    public IEnumerator ClienteGenerator_GeneraClienteValido()
    {
        ClienteGenerator gen = new ClienteGenerator();
        Cliente c = gen.GenerarCliente();

        yield return null;

        Assert.IsNotNull(c);
        Assert.IsNotNull(c.ProductoPedido);
        Assert.IsTrue(c.Dinero >= 5 && c.Dinero <= 30);
    }

    [UnityTest]
    public IEnumerator UIManager_ActualizaUI_TMP()
    {

        GameObject uiObj = new GameObject();
        UIManager ui = uiObj.AddComponent<UIManager>();


        GameObject txt1 = new GameObject();
        GameObject txt2 = new GameObject();
        GameObject txt3 = new GameObject();

        txt1.AddComponent<CanvasRenderer>();
        txt2.AddComponent<CanvasRenderer>();
        txt3.AddComponent<CanvasRenderer>();

        ui.textoCliente = txt1.AddComponent<TextMeshProUGUI>();
        ui.textoDinero = txt2.AddComponent<TextMeshProUGUI>();
        ui.textoDia = txt3.AddComponent<TextMeshProUGUI>();

        GameObject gmObj = new GameObject();
        GameManager gm = gmObj.AddComponent<GameManager>();

        yield return null; 

        gm.dinero = 100;
        gm.dia = 2;

        ui.ActualizarUI();

        yield return null;

        Assert.IsTrue(ui.textoDinero.text.Contains("100"));
        Assert.IsTrue(ui.textoDia.text.Contains("2"));
    }


    [UnityTest]
    public IEnumerator GameManager_VentaAumentaDinero()
    {

        GameObject uiObj = new GameObject();
        UIManager ui = uiObj.AddComponent<UIManager>();

        GameObject txt1 = new GameObject();
        GameObject txt2 = new GameObject();
        GameObject txt3 = new GameObject();

        txt1.AddComponent<CanvasRenderer>();
        txt2.AddComponent<CanvasRenderer>();
        txt3.AddComponent<CanvasRenderer>();

        ui.textoCliente = txt1.AddComponent<TextMeshProUGUI>();
        ui.textoDinero = txt2.AddComponent<TextMeshProUGUI>();
        ui.textoDia = txt3.AddComponent<TextMeshProUGUI>();

        GameObject gmObj = new GameObject();
        GameManager gm = gmObj.AddComponent<GameManager>();

        yield return null;

        int dineroInicial = gm.dinero;

        Cliente cliente = new Cliente(TipoCliente.Normal, "pan", 100);

        gm.AtenderCliente(cliente);

        yield return null;

        Assert.IsTrue(gm.dinero > dineroInicial);
    }

    [UnityTest]
    public IEnumerator GameManager_FinDelDia_AumentaDia()
    {

        GameObject uiObj = new GameObject();
        UIManager ui = uiObj.AddComponent<UIManager>();

        GameObject txt1 = new GameObject();
        GameObject txt2 = new GameObject();
        GameObject txt3 = new GameObject();

        txt1.AddComponent<CanvasRenderer>();
        txt2.AddComponent<CanvasRenderer>();
        txt3.AddComponent<CanvasRenderer>();

        ui.textoCliente = txt1.AddComponent<TextMeshProUGUI>();
        ui.textoDinero = txt2.AddComponent<TextMeshProUGUI>();
        ui.textoDia = txt3.AddComponent<TextMeshProUGUI>();

        GameObject gmObj = new GameObject();
        GameManager gm = gmObj.AddComponent<GameManager>();

        yield return null;

        int diaInicial = gm.dia;

        gm.FinDelDia();

        yield return null;

        Assert.AreEqual(diaInicial + 1, gm.dia);
    }
}