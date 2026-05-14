using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayModeTesting
{
    private readonly List<Object> objetosCreados = new List<Object>();

    private T Crear<T>(string nombre) where T : Component
    {
        GameObject objeto = new GameObject(nombre, typeof(RectTransform), typeof(CanvasRenderer));
        objetosCreados.Add(objeto);
        return objeto.AddComponent<T>();
    }

    private GameObject CrearGameObject(string nombre)
    {
        GameObject objeto = new GameObject(nombre);
        objetosCreados.Add(objeto);
        return objeto;
    }

    private ClienteData CrearClienteData(string nombre, string productoPedido, int dinero, TipoCliente tipo = TipoCliente.Normal)
    {
        ClienteData cliente = ScriptableObject.CreateInstance<ClienteData>();
        objetosCreados.Add(cliente);

        SetPrivateField(cliente, "nombre", nombre);
        SetPrivateField(cliente, "tipo", tipo);
        SetPrivateField(cliente, "productoPedido", productoPedido);
        SetPrivateField(cliente, "dinero", dinero);
        SetPrivateField(cliente, "spriteIndex", 0);
        SetPrivateField(cliente, "frasePedido", "Buenas, me das [producto].");
        SetPrivateField(cliente, "opcion1", "¡Claro!");
        SetPrivateField(cliente, "opcion2", "No sé...");
        SetPrivateField(cliente, "respuesta1", "Gracias.");
        SetPrivateField(cliente, "respuesta2", "Bueno...");

        return cliente;
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, "No se encontró el campo privado '" + fieldName + "' en " + target.GetType().Name);
        field.SetValue(target, value);
    }

    private static void SetStaticBackingField(string fieldName, object value)
    {
        FieldInfo field = typeof(GameManager).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
        if (field != null)
        {
            field.SetValue(null, value);
        }
    }

    [TearDown]
    public void Cleanup()
    {
        for (int i = objetosCreados.Count - 1; i >= 0; i--)
        {
            Object objeto = objetosCreados[i];
            if (objeto != null)
            {
                Object.DestroyImmediate(objeto);
            }
        }

        objetosCreados.Clear();
        SetStaticBackingField("<Instance>k__BackingField", null);
        PlayerPrefs.DeleteKey("VolumenJuego");
        AudioListener.volume = 1f;
    }

    [Test]
    public void GameManager_SeInicializaConInventarioBase()
    {
        GameObject objeto = CrearGameObject("GameManager");
        GameManager gameManager = objeto.AddComponent<GameManager>();

        Assert.AreSame(gameManager, GameManager.Instance);
        Assert.AreEqual(1500, gameManager.Dinero);
        Assert.AreEqual(0, gameManager.Dia);
        Assert.IsNotNull(gameManager.ObtenerInventario().ObtenerProducto("pan"));
        Assert.IsNotNull(gameManager.ObtenerInventario().ObtenerProducto("leche"));
        Assert.IsNotNull(gameManager.ObtenerInventario().ObtenerProducto("huevos"));
    }

    [Test]
    public void GameManager_ComprarArticulo_AumentaStockYRestaDinero()
    {
        GameObject objeto = CrearGameObject("GameManager");
        GameManager gameManager = objeto.AddComponent<GameManager>();

        Producto pan = gameManager.ObtenerInventario().ObtenerProducto("pan");
        int stockInicial = pan.Cantidad;
        int dineroInicial = gameManager.Dinero;

        bool resultado = gameManager.ComprarArticulo("pan", gameManager.CostoCompraPan);

        Assert.IsTrue(resultado);
        Assert.AreEqual(stockInicial + 1, pan.Cantidad);
        Assert.AreEqual(dineroInicial - gameManager.CostoCompraPan, gameManager.Dinero);
    }

    [Test]
    public void GameManager_IntentarVender_VendeYAvanzaTurno()
    {
        GameObject objeto = CrearGameObject("GameManager");
        GameManager gameManager = objeto.AddComponent<GameManager>();

        ClienteData cliente1 = CrearClienteData("Cliente 1", "pan", 100);
        ClienteData cliente2 = CrearClienteData("Cliente 2", "leche", 100);
        SetPrivateField(gameManager, "clientesDelDia", new List<ClienteData> { cliente1, cliente2 });
        SetPrivateField(gameManager, "clienteActual", cliente1);
        SetPrivateField(gameManager, "indiceClienteActualDelDia", 0);
        SetPrivateField(gameManager, "clientesAtendidosHoy", 0);
        SetPrivateField(gameManager, "reglaDelDia", new ReglaGobierno(new List<string> { "huevos" }));
        SetPrivateField(gameManager, "eventoDelDiaActual", "PROHIBIDO: HUEVOS");
        SetPrivateField(gameManager, "ultimoMensajeEvento", string.Empty);

        Producto pan = gameManager.ObtenerInventario().ObtenerProducto("pan");
        int stockInicial = pan.Cantidad;
        int dineroInicial = gameManager.Dinero;
        int precio = pan.Precio;

        gameManager.IntentarVender("pan");

        Assert.AreEqual(dineroInicial + precio, gameManager.Dinero);
        Assert.AreEqual(stockInicial - 1, pan.Cantidad);
        Assert.AreEqual(1, gameManager.ObtenerClientesAtendidosHoy());
        Assert.AreEqual("Venta exitosa.", gameManager.ObtenerUltimoMensajeEvento());
    }

    [Test]
    public void GameManager_BotonRechazar_NoSancionaClienteProhibido()
    {
        GameObject objeto = CrearGameObject("GameManager");
        GameManager gameManager = objeto.AddComponent<GameManager>();

        ClienteData cliente1 = CrearClienteData("Cliente 1", "trampa", 100);
        ClienteData cliente2 = CrearClienteData("Cliente 2", "pan", 100);
        SetPrivateField(gameManager, "clientesDelDia", new List<ClienteData> { cliente1, cliente2 });
        SetPrivateField(gameManager, "clienteActual", cliente1);
        SetPrivateField(gameManager, "indiceClienteActualDelDia", 0);
        SetPrivateField(gameManager, "clientesAtendidosHoy", 0);
        SetPrivateField(gameManager, "<Dia>k__BackingField", 1);
        SetPrivateField(gameManager, "reglaDelDia", new ReglaGobierno(new List<string> { "leche" }));
        SetPrivateField(gameManager, "eventoDelDiaActual", "PROHIBIDO: LECHE");
        SetPrivateField(gameManager, "ultimoMensajeEvento", string.Empty);

        gameManager.BotonRechazar();

        Assert.AreEqual(1, gameManager.ObtenerClientesAtendidosHoy());
        Assert.AreEqual(0, gameManager.Amonestaciones);
        Assert.AreEqual("Rechazo correcto.", gameManager.ObtenerUltimoMensajeEvento());
    }

    [Test]
    public void UIManager_ActualizarUI_ReflejaEstadoDelGameManager()
    {
        GameObject gameManagerObject = CrearGameObject("GameManager");
        GameManager gameManager = gameManagerObject.AddComponent<GameManager>();

        GameObject uiObject = CrearGameObject("UIManager");
        UIManager uiManager = uiObject.AddComponent<UIManager>();

        TextMeshProUGUI textoDinero = Crear<TMP_TextAdapter>("Dinero").Texto;
        TextMeshProUGUI textoDia = Crear<TMP_TextAdapter>("Dia").Texto;
        TextMeshProUGUI textoInventario = Crear<TMP_TextAdapter>("Inventario").Texto;
        TextMeshProUGUI textoEstado = Crear<TMP_TextAdapter>("Estado").Texto;
        TextMeshProUGUI textoAmonestaciones = Crear<TMP_TextAdapter>("Amonestaciones").Texto;
        TextMeshProUGUI textoMetaDiaria = Crear<TMP_TextAdapter>("Meta").Texto;
        TextMeshProUGUI textoEventoDelDia = Crear<TMP_TextAdapter>("Evento").Texto;

        SetPrivateField(uiManager, "textoDinero", textoDinero);
        SetPrivateField(uiManager, "textoDia", textoDia);
        SetPrivateField(uiManager, "textoInventario", textoInventario);
        SetPrivateField(uiManager, "textoEstado", textoEstado);
        SetPrivateField(uiManager, "textoAmonestaciones", textoAmonestaciones);
        SetPrivateField(uiManager, "textoMetaDiaria", textoMetaDiaria);
        SetPrivateField(uiManager, "textoEventoDelDia", textoEventoDelDia);

        SetPrivateField(gameManager, "<Dinero>k__BackingField", 2222);
        SetPrivateField(gameManager, "<Dia>k__BackingField", 3);
        SetPrivateField(gameManager, "<Amonestaciones>k__BackingField", 1);
        SetPrivateField(gameManager, "<DineroGanadoEnElDia>k__BackingField", 450);
        SetPrivateField(gameManager, "ultimoMensajeEvento", "Venta exitosa.");
        SetPrivateField(gameManager, "eventoDelDiaActual", "PROHIBIDO: PAN");

        uiManager.ActualizarUI();

        Assert.AreEqual("2222", textoDinero.text);
        Assert.AreEqual("Día: 3", textoDia.text);
        Assert.AreEqual("1/3", textoAmonestaciones.text);
        Assert.IsTrue(textoMetaDiaria.text.Contains("$450/$20"));
        Assert.IsTrue(textoMetaDiaria.text.Contains("Clientes: 0/5"));
        Assert.AreEqual("Venta exitosa.", textoEstado.text);
        Assert.AreEqual("Evento: PROHIBIDO: PAN", textoEventoDelDia.text);
        Assert.IsTrue(textoInventario.text.Contains("Inventario:"));
        Assert.IsTrue(textoInventario.text.Contains("pan | Stock:"));
    }

    [Test]
    public void UIManager_CambiarVolumen_ActualizaAudioListenerYPlayerPrefs()
    {
        GameObject uiObject = CrearGameObject("UIManager");
        UIManager uiManager = uiObject.AddComponent<UIManager>();

        uiManager.CambiarVolumen(0.35f);

        Assert.AreEqual(0.35f, AudioListener.volume, 0.0001f);
        Assert.AreEqual(0.35f, PlayerPrefs.GetFloat("VolumenJuego"), 0.0001f);
    }

    [Test]
    public void ClienteGenerator_ObtenerClientesDelDia_RespetaCantidadYBanco()
    {
        GameObject objeto = CrearGameObject("Generador");
        ClienteGenerator generador = objeto.AddComponent<ClienteGenerator>();

        ClienteData clienteA = CrearClienteData("A", "pan", 10);
        ClienteData clienteB = CrearClienteData("B", "leche", 20);
        ClienteData clienteC = CrearClienteData("C", "huevos", 30);

        generador.clientesPorDia = 2;
        generador.bancoDia1 = new List<ClienteData> { clienteA, clienteB, clienteC };

        List<ClienteData> seleccionados = generador.ObtenerClientesDelDia(1);

        Assert.AreEqual(2, seleccionados.Count);
        Assert.Contains(seleccionados[0], new List<ClienteData> { clienteA, clienteB, clienteC });
        Assert.Contains(seleccionados[1], new List<ClienteData> { clienteA, clienteB, clienteC });
        Assert.AreNotEqual(seleccionados[0], seleccionados[1]);
    }

    [Test]
    public void ControladorTransicion_ObtenerReglasSegunDia_DevuelveTextoEsperado()
    {
        GameObject objeto = CrearGameObject("Transicion");
        ControladorTransicion controlador = objeto.AddComponent<ControladorTransicion>();

        MethodInfo metodo = typeof(ControladorTransicion).GetMethod("ObtenerReglasSegunDia", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(metodo);
        Assert.AreEqual("Hoy las vacas decidieron irse de paseo... PROHIBIDO vender LECHE.", metodo.Invoke(controlador, new object[] { 1 }));
        Assert.AreEqual("¡Crisis nacional!... PROHIBIDO vender LECHE y HUEVOS.", metodo.Invoke(controlador, new object[] { 4 }));
        Assert.AreEqual("Sigue las normas del gobierno.", metodo.Invoke(controlador, new object[] { 99 }));
    }

    private class TMP_TextAdapter : MonoBehaviour
    {
        public TextMeshProUGUI Texto { get; private set; }

        private void Awake()
        {
            Texto = gameObject.AddComponent<TextMeshProUGUI>();
        }
    }
}