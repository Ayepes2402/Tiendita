using UnityEngine;
using UnityEngine.EventSystems;

public class RepisaHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración de la Repisa")]
    public RectTransform repisaRect;

    [Tooltip("Posición en X cuando está abierta (ej: 0)")]
    public float posicionAbierta = 0f;

    [Tooltip("Posición en X cuando está escondida (ej: -200)")]
    public float posicionCerrada = -200f;

    [Tooltip("Qué tan rápido sale y se esconde")]
    public float velocidad = 10f;

    private float posicionObjetivo;

    private void Start()
    {
        
        if (repisaRect == null)
            repisaRect = GetComponent<RectTransform>();

       
        posicionObjetivo = posicionCerrada;
        repisaRect.anchoredPosition = new Vector2(posicionCerrada, repisaRect.anchoredPosition.y);
    }

    private void Update()
    {
        
        float nuevaX = Mathf.Lerp(repisaRect.anchoredPosition.x, posicionObjetivo, Time.deltaTime * velocidad);
        repisaRect.anchoredPosition = new Vector2(nuevaX, repisaRect.anchoredPosition.y);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        posicionObjetivo = posicionAbierta;
    }

  
    public void OnPointerExit(PointerEventData eventData)
    {
        posicionObjetivo = posicionCerrada;
    }
}