using System.Collections;
using UnityEngine;
using TMPro;

public class EfectoGanancia : MonoBehaviour
{
    public float velocidadSubida = 100f; // Súbele un poco para que se note el movimiento
    public float tiempoVida = 1.2f;

    private TextMeshProUGUI textoTMP;
    private Color colorOriginal;

    void Awake()
    {
        textoTMP = GetComponent<TextMeshProUGUI>();
        if (textoTMP != null) colorOriginal = textoTMP.color;
    }

    public void IniciarEfecto(int cantidad)
    {
        if (textoTMP == null) textoTMP = GetComponent<TextMeshProUGUI>();

        textoTMP.text = "+" + cantidad;
        // Lo mandamos al frente de todo para que nada lo tape
        transform.SetAsLastSibling();
        StartCoroutine(AnimarYDestruir());
    }

    IEnumerator AnimarYDestruir()
    {
        float tiempoPasado = 0f;
        RectTransform rect = GetComponent<RectTransform>();
        Vector2 posicionInicial = rect.anchoredPosition;

        while (tiempoPasado < tiempoVida)
        {
            tiempoPasado += Time.deltaTime;

            // Calculamos la nueva posición sumando la subida
            // Usamos una suma directa para que no se quede "pegado"
            float desplazamiento = velocidadSubida * (tiempoPasado / tiempoVida);
            rect.anchoredPosition = posicionInicial + new Vector2(0, desplazamiento);

            // Desvanecimiento suave
            if (textoTMP != null)
            {
                float alfa = Mathf.Lerp(1f, 0f, tiempoPasado / tiempoVida);
                textoTMP.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alfa);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}