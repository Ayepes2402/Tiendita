using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 


[RequireComponent(typeof(Image))] 
[RequireComponent(typeof(CanvasGroup))] 
public class EfectoHoverOpcion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Referencias UI")]
    public GameObject flechitaIndicadora; 

    [Header("Configuración Opacidad (0 a 1)")]
    public float opacidadNormal = 0.5f; 
    public float opacidadHover = 1.0f;   
    public float velocidadFade = 10f;    

    private CanvasGroup canvasGroup;
    private bool mouseEncima = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (flechitaIndicadora != null)
        {
            flechitaIndicadora.SetActive(false); 
        }

        
        canvasGroup.alpha = opacidadNormal;
    }

    void Update()
    {
       
        float opacidadObjetivo = mouseEncima ? opacidadHover : opacidadNormal;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, opacidadObjetivo, Time.deltaTime * velocidadFade);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseEncima = true;
        if (flechitaIndicadora != null)
        {
            flechitaIndicadora.SetActive(true); 
        }
    }

    
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseEncima = false;
        if (flechitaIndicadora != null)
        {
            flechitaIndicadora.SetActive(false); 
        }
    }
}