using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Necesario pa' manejar la imagen y el texto viejo

public class DragUICategoria : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Inventario y Estadísticas")]
    public int unidadesDisponibles = 10;

    [Tooltip("Arrastre aquí el texto que va a mostrar el numerito")]
    public Text textoContador;

    [Tooltip("Ponga acá EXACTAMENTE el nombre: pan, leche o huevos (en minúscula)")]
    public string nombreProducto = "pan";

    private GameObject fantasma;
    private RectTransform fantasmaRect;
    private Canvas canvasPrincipal;
    private Image miImagen;

    private void Start()
    {
        canvasPrincipal = GetComponentInParent<Canvas>();
        miImagen = GetComponent<Image>();
        ActualizarTexto();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Si no hay productos, paila, no deja arrastrar ni mierda
        if (unidadesDisponibles <= 0) return;

        // Creamos un objeto vacío en la pura raíz del Canvas pa' que pase por encima de todo
        fantasma = new GameObject("FantasmaDrag");
        fantasma.transform.SetParent(canvasPrincipal.transform, false);
        fantasma.transform.SetAsLastSibling();

        // Le clonamos la imagen pa' que se vea igualitico al de la repisa
        Image imagenFantasma = fantasma.AddComponent<Image>();
        imagenFantasma.sprite = miImagen.sprite;
        imagenFantasma.color = new Color(1f, 1f, 1f, 0.7f); // Medio transparente pa' que se note que es un agarre

        // ¡CLAVE! El fantasma no puede tener raycast o bloquea la soltada en el cliente
        imagenFantasma.raycastTarget = false;

        fantasmaRect = fantasma.GetComponent<RectTransform>();
        fantasmaRect.sizeDelta = GetComponent<RectTransform>().sizeDelta;

        // Lo ponemos en la misma posición del mouse al arrancar
        fantasmaRect.position = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (fantasma != null)
        {
            // Mueve el fantasma, no el objeto original de la repisa
            fantasmaRect.anchoredPosition += eventData.delta / canvasPrincipal.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (fantasma != null)
        {
            // Suelte donde suelte, el fantasma desaparece. 
            // El DropUICliente es el que decide si fue venta o no.
            Destroy(fantasma);
        }
    }

    // El cliente llama a esta vuelta si le tiran el producto encima
    public void RestarInventario()
    {
        if (unidadesDisponibles > 0)
        {
            unidadesDisponibles--;
            ActualizarTexto();

            if (unidadesDisponibles <= 0)
            {
                // Opcional: Ponga la imagen gris si se quedó sin surtido
                miImagen.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }
    }

    private void ActualizarTexto()
    {
        if (textoContador != null)
        {
            textoContador.text = unidadesDisponibles.ToString();
        }
    }
}