using System.Text;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI textoCliente;
    public TextMeshProUGUI textoDinero;
    public TextMeshProUGUI textoDia;
    public TextMeshProUGUI textoInventario;
    public TextMeshProUGUI textoEstado;
    public TextMeshProUGUI textoAmonestaciones;
    public TextMeshProUGUI textoMetaDiaria;
    public GameObject panelGameOver;
    public TextMeshProUGUI textoGameOver;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }

        ActualizarUI();
    }

    public void MostrarCliente(Cliente cliente)
    {
        if (textoCliente != null && cliente != null)
        {
            textoCliente.text = "Cliente: Quiero " + cliente.ProductoPedido;
        }
    }

    public void MostrarEstado(string mensaje)
    {
        if (textoEstado != null)
        {
            textoEstado.text = mensaje;
        }
    }

    public void MostrarGameOver(string mensaje)
    {
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }

        if (textoGameOver != null)
        {
            textoGameOver.text = mensaje + "\nPresiona Restart para volver a jugar.";
        }

        MostrarEstado(mensaje);
    }

    public void ActualizarUI()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        if (gameManager == null)
        {
            return;
        }

        if (textoDinero != null)
        {
            textoDinero.text = "Dinero: $" + gameManager.dinero;
        }

        if (textoDia != null)
        {
            textoDia.text = "Dia: " + gameManager.dia;
        }

        if (textoAmonestaciones != null)
        {
            textoAmonestaciones.text = "Amonestaciones: " + gameManager.amonestaciones + "/" + gameManager.maxAmonestaciones;
        }

        if (textoMetaDiaria != null)
        {
            textoMetaDiaria.text = "Meta diaria: $" + gameManager.ObtenerDineroGanadoEnElDia() +
                                   "/$" + gameManager.ObtenerMetaMinimaDiaria();
        }

        MostrarEstado(gameManager.ObtenerUltimoMensajeEvento());
        ActualizarInventarioUI();
    }

    private void ActualizarInventarioUI()
    {
        if (textoInventario == null)
        {
            return;
        }

        Inventario inventario = gameManager.ObtenerInventario();

        if (inventario == null)
        {
            textoInventario.text = "Inventario no disponible";
            return;
        }

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Inventario:");

        foreach (Producto producto in inventario.ObtenerProductos())
        {
            builder.AppendLine(producto.Nombre + ": " + producto.Cantidad);
        }

        textoInventario.text = builder.ToString();
    }

    public void BotonRestart()
    {
        if (gameManager != null)
        {
            gameManager.ReiniciarJuego();
        }
    }
}