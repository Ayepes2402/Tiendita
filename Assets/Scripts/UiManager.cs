using System.Text;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI textoCliente;
    public TextMeshProUGUI textoDinero;
    public TextMeshProUGUI textoDia;
    public TextMeshProUGUI textoInventario;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        ActualizarUI();
    }

    public void MostrarCliente(Cliente cliente)
    {
        if (textoCliente != null)
        {
            textoCliente.text = "Cliente: Quiero " + cliente.ProductoPedido;
        }
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
}
