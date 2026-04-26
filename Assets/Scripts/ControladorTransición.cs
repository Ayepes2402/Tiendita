using System.Collections;
using UnityEngine;
using TMPro;

public class ControladorTransicion : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TextMeshProUGUI textoTituloDiaSiguiente;
    public TextMeshProUGUI textoReglasNuevas;
    public TextMeshProUGUI textoResumenDinero;
    public GameObject botonContinuar;

    [Header("Configuración de Texto")]
    public float velocidadEscritura = 0.04f;
    private string historiaDelDia = "";

    void Start()
    {
        if (botonContinuar != null) botonContinuar.SetActive(false);

        if (GameManager.Instance != null)
        {
            int proximoDia = GameManager.Instance.dia + 1;

            textoTituloDiaSiguiente.text = "DÍA " + proximoDia;

            
            textoResumenDinero.text =
                "Dinero actual: $" + GameManager.Instance.dinero +
                "\nCuota Pendiente: $" + GameManager.Instance.deuda +
                "\n\nPrecios de Hoy: \n" +
                "- Pan: $" + GameManager.Instance.costoCompraPan +
                " - Leche: $" + GameManager.Instance.costoCompraLeche +
                " - Huevos: $" + GameManager.Instance.costoCompraHuevos;

            historiaDelDia = ObtenerReglasSegunDia(proximoDia);
            StartCoroutine(EscribirReglaAnimada());
        }
        else
        {
            textoTituloDiaSiguiente.text = "ERROR";
            textoResumenDinero.text = "GameManager no encontrado.\nInicia desde la escena principal.";
            historiaDelDia = "No se pudieron cargar las reglas.";
            StartCoroutine(EscribirReglaAnimada());
        }
    }

   

    IEnumerator EscribirReglaAnimada()
    {
        textoReglasNuevas.text = historiaDelDia;
        textoReglasNuevas.maxVisibleCharacters = 0;
        textoReglasNuevas.ForceMeshUpdate();

        int totalCaracteres = textoReglasNuevas.textInfo.characterCount;

        for (int i = 0; i <= totalCaracteres; i++)
        {
            textoReglasNuevas.maxVisibleCharacters = i;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        if (botonContinuar != null) botonContinuar.SetActive(true);
    }

    string ObtenerReglasSegunDia(int d)
    {
        switch (d)
        {
            case 1: return "Hoy las vacas decidieron irse de paseo... PROHIBIDO vender LECHE.";
            case 2: return "Las gallinas han formado un sindicato... PROHIBIDO vender HUEVOS.";
            case 3: return "Desastre en el molino... PROHIBIDO vender PAN.";
            case 4: return "¡Crisis nacional!... PROHIBIDO vender LECHE y HUEVOS.";
            case 5: return "Último día... PROHIBIDO vender HUEVOS y PAN. ¡Paga tu deuda!";
            default: return "Sigue las normas del gobierno.";
        }
    }

    public void BotonEmpezarSiguienteDia()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.IniciarSiguienteDia();
        }
    }
}