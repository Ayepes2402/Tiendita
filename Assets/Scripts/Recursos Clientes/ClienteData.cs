using UnityEngine;


[CreateAssetMenu(fileName = "NuevoCliente", menuName = "Tiendita/Cliente", order = 1)]
public class ClienteData : ScriptableObject
{
    [Header("Información Básica")]
    [SerializeField] private string nombre;
    [SerializeField] private TipoCliente tipo; 
    [SerializeField] private string productoPedido;
    [SerializeField] private int dinero;

    [Header("Aspecto Visual")]
    [SerializeField] private int spriteIndex;

    [Header("Diálogos")]
    [TextArea(2, 3)]
    [SerializeField] private string frasePedido;

    [SerializeField] private string opcion1 = "¡Claro!";
    [SerializeField] private string opcion2 = "No sé...";
    [SerializeField] private string respuesta1 = "Gracias.";
    [SerializeField] private string respuesta2 = "Bueno...";


    public string Nombre => nombre;
    public TipoCliente Tipo => tipo;
    public string ProductoPedido => productoPedido;
    public int Dinero => dinero;
    public int SpriteIndex => spriteIndex;
    public string FrasePedido => frasePedido;
    public string Opcion1 => opcion1;
    public string Opcion2 => opcion2;
    public string Respuesta1 => respuesta1;
    public string Respuesta2 => respuesta2;
}