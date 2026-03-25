using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI textoCliente;
    public TextMeshProUGUI textoDinero;
    public TextMeshProUGUI textoDia;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        ActualizarUI();
    }

    public void MostrarCliente(Cliente cliente)
    {
        textoCliente.text = "Cliente: Quiero " + cliente.ProductoPedido;
    }

    public void ActualizarUI()
    {
        textoDinero.text = "Dinero: $" + gameManager.dinero;
        textoDia.text = "Día: " + gameManager.dia;
    }
}