using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EfectoMaquinaDeEscribir : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI textoDialogo;
    public GameObject botonFlecha;

    [Header("Botón Nueva Escena")]
    public CanvasGroup grupoBotonEscena;
    public string nombreEscenaACargar;

    [Header("Transición de Escena")]
    public CanvasGroup pantallaNegra; 
    public float velocidadTransicion = 2f; 

    [Header("Configuración")]
    [TextArea(3, 10)]
    public string[] parrafos;
    public float velocidadEscritura = 0.04f;
    public float velocidadFade = 1.5f;

    private int indiceActual = 0;
    private bool estaEscribiendo = false;
    private Coroutine corrutinaEscritura;

    void Start()
    {
        botonFlecha.SetActive(false);

        if (grupoBotonEscena != null)
        {
            grupoBotonEscena.alpha = 0;
            grupoBotonEscena.interactable = false;
            grupoBotonEscena.blocksRaycasts = false;
        }

        
        if (pantallaNegra != null)
        {
            pantallaNegra.alpha = 0;
            pantallaNegra.blocksRaycasts = false;
        }

        EmpezarDialogo();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            AvanzarOSaltarDialogo();
        }
    }

    public void EmpezarDialogo()
    {
        indiceActual = 0;

        if (corrutinaEscritura != null)
        {
            StopCoroutine(corrutinaEscritura);
        }
        corrutinaEscritura = StartCoroutine(EscribirTexto());
    }

    IEnumerator EscribirTexto()
    {
        estaEscribiendo = true;
        AudioManager.instancia.ReproducirDialogo();
        textoDialogo.text = parrafos[indiceActual];
        textoDialogo.maxVisibleCharacters = 0;
        botonFlecha.SetActive(false);

        textoDialogo.ForceMeshUpdate();
        int totalCaracteres = textoDialogo.textInfo.characterCount;

        for (int i = 0; i <= totalCaracteres; i++)
        {
            textoDialogo.maxVisibleCharacters = i;
            yield return new WaitForSeconds(velocidadEscritura);
        }
        AudioManager.instancia.DetenerDialogo();
        estaEscribiendo = false;
        MostrarBotonesFinales();
    }

    public void AvanzarOSaltarDialogo()
    {
        if (estaEscribiendo)
        {
            if (corrutinaEscritura != null)
            {
                StopCoroutine(corrutinaEscritura);
            }
            AudioManager.instancia.DetenerDialogo();
            textoDialogo.maxVisibleCharacters = textoDialogo.textInfo.characterCount;
            estaEscribiendo = false;
            MostrarBotonesFinales();
        }
        else if (indiceActual < parrafos.Length - 1)
        {
            indiceActual++;
            corrutinaEscritura = StartCoroutine(EscribirTexto());
        }
    }

    private void MostrarBotonesFinales()
    {
        if (indiceActual < parrafos.Length - 1)
        {
            botonFlecha.SetActive(true);
        }
        else
        {
            StartCoroutine(AparecerBotonEscena());
        }
    }

    IEnumerator AparecerBotonEscena()
    {
        grupoBotonEscena.gameObject.SetActive(true);
        grupoBotonEscena.interactable = true;
        grupoBotonEscena.blocksRaycasts = true;

        while (grupoBotonEscena.alpha < 1)
        {
            grupoBotonEscena.alpha += Time.deltaTime * velocidadFade;
            yield return null;
        }
    }

    
    public void CargarSiguienteEscena()
    {
        
        if (pantallaNegra != null)
        {
            StartCoroutine(TransicionYCambioDeEscena());
        }
        else
        {
            
            SceneManager.LoadScene(nombreEscenaACargar);
        }
    }

    IEnumerator TransicionYCambioDeEscena()
    {
        
        pantallaNegra.blocksRaycasts = true;

        
        while (pantallaNegra.alpha < 1)
        {
            pantallaNegra.alpha += Time.deltaTime * velocidadTransicion;
            yield return null;
        }

        
        yield return new WaitForSeconds(0.5f);

       
        SceneManager.LoadScene(nombreEscenaACargar);
    }
}