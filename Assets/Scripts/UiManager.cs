using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI textoCliente;
    public TextMeshProUGUI textoDinero;
    public TextMeshProUGUI textoDia;
    public TextMeshProUGUI textoInventario;
    public TextMeshProUGUI textoEstado;
    public TextMeshProUGUI textoAmonestaciones;
    public TextMeshProUGUI textoMetaDiaria;
    public TextMeshProUGUI textoEventoDelDia;
    public TextMeshProUGUI textoAvisoDia;
    public GameObject panelCambioDia;
    public TextMeshProUGUI textoCambioDia;
    public GameObject panelGameOver;
    public TextMeshProUGUI textoGameOver;
    public Image imagenCliente;
    public Sprite[] spritesClientes;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }

        if (panelCambioDia != null)
        {
            panelCambioDia.SetActive(false);
        }

        ActualizarUI();
    }

    public void MostrarCliente(Cliente cliente)
    {
        if (textoCliente != null)
        {
            if (cliente == null)
            {
                textoCliente.text = "No hay cliente actual";
            }
            else
            {
                textoCliente.text = cliente.Nombre + " - " + cliente.Personalidad + "\n\"" + cliente.FrasePedido + "\"";
            }
        }

        if (imagenCliente != null)
        {
            if (cliente != null && spritesClientes != null &&
                cliente.SpriteIndex >= 0 && cliente.SpriteIndex < spritesClientes.Length)
            {
                imagenCliente.enabled = true;
                imagenCliente.sprite = spritesClientes[cliente.SpriteIndex];
            }
            else
            {
                imagenCliente.enabled = false;
            }
        }
    }

    public void MostrarEstado(string mensaje)
    {
        if (textoEstado != null)
        {
            textoEstado.text = mensaje;
        }
    }

    public void MostrarEventoDelDia(string mensaje)
    {
        if (textoEventoDelDia != null)
        {
            textoEventoDelDia.text = "Evento del día: " + mensaje;
        }
    }

    public void MostrarAvisoDia(string mensaje)
    {
        if (textoAvisoDia != null)
        {
            textoAvisoDia.text = mensaje;
        }
    }


    public void MostrarPanelCambioDia(string mensaje)
    {
        if (panelCambioDia != null)
        {
            panelCambioDia.SetActive(true);
        }

        if (textoCambioDia != null)
        {
            textoCambioDia.text = mensaje;
        }
    }

    public void OcultarPanelCambioDia()
    {
        if (panelCambioDia != null)
        {
            panelCambioDia.SetActive(false);
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
            textoDia.text = "Día: " + gameManager.dia;
        }

        if (textoAmonestaciones != null)
        {
            textoAmonestaciones.text = "Amonestaciones: " + gameManager.amonestaciones + "/" + gameManager.maxAmonestaciones;
        }

        if (textoMetaDiaria != null)
        {
            textoMetaDiaria.text = "Clientes atendidos: " + gameManager.ObtenerClientesAtendidosHoy() +
                                   "/" + gameManager.ObtenerClientesPorDia() +
                                   " | Ganado hoy: $" + gameManager.ObtenerDineroGanadoEnElDia();
        }

        MostrarEstado(gameManager.ObtenerUltimoMensajeEvento());
        MostrarEventoDelDia(gameManager.ObtenerEventoDelDiaActual());
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

    public void BotonContinuarDia()
    {
        if (gameManager != null)
        {
            gameManager.BotonContinuarDia();
        }
    }

    public void BotonRestart()
    {
        if (gameManager != null)
        {
            gameManager.ReiniciarJuego();
        }
    }
}
