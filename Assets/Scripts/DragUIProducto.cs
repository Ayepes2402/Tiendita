using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DragUICategoria : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Configuración Visual")]
    [Tooltip("Arrastre aquí el texto (TextMeshPro) que va a mostrar el numerito")]
    public TextMeshProUGUI textoContador;

    [Tooltip("Ponga acá EXACTAMENTE el nombre: pan, leche o huevos (en minúscula)")]
    public string nombreProducto = "pan";

    private GameObject fantasma;
    private RectTransform fantasmaRect;
    private Canvas canvasPrincipal;
    private Image miImagen;
    private GameManager gameManager;

    private void Start()
    {
        canvasPrincipal = GetComponentInParent<Canvas>();
        miImagen = GetComponent<Image>();
        gameManager = FindObjectOfType<GameManager>();

        // Le damos una micro-fracción de segundo pa' que el GameManager 
        // alcance a crear el Inventario antes de que la repisa intente leerlo.
        Invoke("ActualizarTextoDesdeBackend", 0.1f);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Revisamos el inventario real del sistema antes de dejarlo agarrar nada
        int unidadesReales = ObtenerUnidadesReales();
        if (unidadesReales <= 0) return;

        fantasma = new GameObject("FantasmaDrag");
        fantasma.transform.SetParent(canvasPrincipal.transform, false);
        fantasma.transform.SetAsLastSibling();

        Image imagenFantasma = fantasma.AddComponent<Image>();
        imagenFantasma.sprite = miImagen.sprite;
        imagenFantasma.color = new Color(1f, 1f, 1f, 0.7f);
        imagenFantasma.raycastTarget = false;

        fantasmaRect = fantasma.GetComponent<RectTransform>();
        fantasmaRect.sizeDelta = GetComponent<RectTransform>().sizeDelta;
        fantasmaRect.position = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (fantasma != null)
        {
            fantasmaRect.anchoredPosition += eventData.delta / canvasPrincipal.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (fantasma != null)
        {
            Destroy(fantasma);
        }
    }

    // El DropUICliente sigue llamando a esto, pero ahora solo actualiza la foto
    // porque el GameManager ya se encargó de restar la mercancía en su código
    public void RestarInventario()
    {
        ActualizarTextoDesdeBackend();
    }

    // Esta es la función que se asoma a la base de datos real
    public void ActualizarTextoDesdeBackend()
    {
        if (gameManager == null) return;

        int unidadesReales = ObtenerUnidadesReales();

        if (textoContador != null)
        {
            textoContador.text = unidadesReales.ToString();
        }

        // Si se nos acabó la vaina en la base de datos, ponemos el dibujo gris
        if (unidadesReales <= 0)
        {
            miImagen.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }

    // Busca el número real metiéndose por las tripas del GameManager
    public int ObtenerUnidadesReales()
    {
        if (gameManager != null && gameManager.ObtenerInventario() != null)
        {
            var producto = gameManager.ObtenerInventario().ObtenerProducto(nombreProducto);
            if (producto != null) return producto.Cantidad;
        }
        return 0; // Si no lo encuentra, devuelve 0 por seguridad
    }
}