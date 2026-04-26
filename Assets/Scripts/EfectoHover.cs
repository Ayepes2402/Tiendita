using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Importante para detectar el mouse

// Este script debe ir en cada botón u opción
[RequireComponent(typeof(Image))] // Necesita una imagen para la opacidad
[RequireComponent(typeof(CanvasGroup))] // CanvasGroup es el rey para la opacidad
public class EfectoHoverOpcion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Referencias UI")]
    public GameObject flechitaIndicadora; // Arrastra el objeto de la flecha que está DENTRO de esta opción

    [Header("Configuración Opacidad (0 a 1)")]
    public float opacidadNormal = 0.5f; // Opacidad cuando el mouse NO está encima
    public float opacidadHover = 1.0f;   // Opacidad (oscuridad) cuando el mouse SÍ está encima
    public float velocidadFade = 10f;    // Qué tan rápido cambia la opacidad

    private CanvasGroup canvasGroup;
    private bool mouseEncima = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (flechitaIndicadora != null)
        {
            flechitaIndicadora.SetActive(false); // La flecha empieza apagada
        }

        // Seteamos la opacidad inicial
        canvasGroup.alpha = opacidadNormal;
    }

    void Update()
    {
        // Usamos Lerp para un fade suave de la opacidad en el CanvasGroup
        float opacidadObjetivo = mouseEncima ? opacidadHover : opacidadNormal;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, opacidadObjetivo, Time.deltaTime * velocidadFade);
    }

    // Se ejecuta cuando el mouse ENTRA en el área del botón
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseEncima = true;
        if (flechitaIndicadora != null)
        {
            flechitaIndicadora.SetActive(true); // Prende la flecha
        }
    }

    // Se ejecuta cuando el mouse SALE del área del botón
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseEncima = false;
        if (flechitaIndicadora != null)
        {
            flechitaIndicadora.SetActive(false); // Apaga la flecha
        }
    }
}