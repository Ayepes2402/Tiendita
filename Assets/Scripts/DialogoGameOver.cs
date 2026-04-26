using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EfectoDialogoComplejo : MonoBehaviour
{
    [Header("Referencias UI - Texto FIJO")]
    public TextMeshProUGUI textoFijo;
    public float velocidadFadeInFijo = 1f;

    [Header("Referencias UI - Texto ANIMADO")]
    public TextMeshProUGUI textoDialogoAnimado;
    public GameObject botonFlecha;

    [Header("Botón Nueva Escena")]
    public CanvasGroup grupoBotonEscena;
    public string nombreEscenaACargar;

    [Header("Transición de Escena (Pantalla Negra)")]
    public CanvasGroup pantallaNegra;
    public float velocidadTransicionNegra = 2f;

    [Header("Configuración Máquina de Escribir")]
    [TextArea(3, 10)]
    public string[] parrafos;
    public float velocidadEscritura = 0.04f;
    public float velocidadFadeBoton = 1.5f;

    [Header("Sonidos")]
    public AudioSource audioSourceEfectos; 
    public AudioClip sonidoClickNormal;    

    private int indiceActual = 0;
    private bool estaEscribiendo = false;
    private bool textoFijoCompletado = false;
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

        textoDialogoAnimado.text = "";

        if (textoFijo != null)
        {
            Color colorFijo = textoFijo.color;
            colorFijo.a = 0;
            textoFijo.color = colorFijo;
        }

        StartCoroutine(SecuenciaIntro());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (textoFijoCompletado)
            {
                AvanzarOSaltarDialogo();
            }
        }
    }

    IEnumerator SecuenciaIntro()
    {
        if (textoFijo != null)
        {
            while (textoFijo.color.a < 1)
            {
                Color colorFijo = textoFijo.color;
                colorFijo.a += Time.deltaTime * velocidadFadeInFijo;
                textoFijo.color = colorFijo;
                yield return null;
            }
            Color colorFinalFijo = textoFijo.color;
            colorFinalFijo.a = 1f;
            textoFijo.color = colorFinalFijo;
        }

        textoFijoCompletado = true;
        EmpezarDialogo();
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

       
        if (AudioManager.instancia != null) AudioManager.instancia.ReproducirDialogo();

        textoDialogoAnimado.text = parrafos[indiceActual];
        textoDialogoAnimado.maxVisibleCharacters = 0;
        botonFlecha.SetActive(false);

        textoDialogoAnimado.ForceMeshUpdate();
        int totalCaracteres = textoDialogoAnimado.textInfo.characterCount;

        for (int i = 0; i <= totalCaracteres; i++)
        {
            textoDialogoAnimado.maxVisibleCharacters = i;
            yield return new WaitForSeconds(velocidadEscritura);
        }

       
        if (AudioManager.instancia != null) AudioManager.instancia.DetenerDialogo();

        estaEscribiendo = false;
        MostrarBotonesFinales();
    }

    public void AvanzarOSaltarDialogo()
    {
        
        if (audioSourceEfectos != null && sonidoClickNormal != null)
        {
            audioSourceEfectos.PlayOneShot(sonidoClickNormal);
        }

        if (estaEscribiendo)
        {
            if (corrutinaEscritura != null)
            {
                StopCoroutine(corrutinaEscritura);
            }

            
            if (AudioManager.instancia != null) AudioManager.instancia.DetenerDialogo();

            textoDialogoAnimado.maxVisibleCharacters = textoDialogoAnimado.textInfo.characterCount;
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
        if (grupoBotonEscena != null)
        {
            grupoBotonEscena.gameObject.SetActive(true);
            grupoBotonEscena.interactable = true;
            grupoBotonEscena.blocksRaycasts = true;

            while (grupoBotonEscena.alpha < 1)
            {
                grupoBotonEscena.alpha += Time.deltaTime * velocidadFadeBoton;
                yield return null;
            }
        }
    }

   
    public void CargarSiguienteEscena()
    {
        
        if (audioSourceEfectos != null && sonidoClickNormal != null)
        {
            audioSourceEfectos.PlayOneShot(sonidoClickNormal);
        }

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

        float tiempoDesvanecimiento = 0f;
        while (tiempoDesvanecimiento < 1f)
        {
            tiempoDesvanecimiento += Time.deltaTime * velocidadTransicionNegra;

            if (textoFijo != null)
            {
                Color c = textoFijo.color;
                c.a = Mathf.Lerp(1, 0, tiempoDesvanecimiento);
                textoFijo.color = c;
            }

            if (textoDialogoAnimado != null)
            {
                Color c2 = textoDialogoAnimado.color;
                c2.a = Mathf.Lerp(1, 0, tiempoDesvanecimiento);
                textoDialogoAnimado.color = c2;
            }

            if (grupoBotonEscena != null)
            {
                grupoBotonEscena.alpha = Mathf.Lerp(1, 0, tiempoDesvanecimiento);
            }

            yield return null;
        }

        while (pantallaNegra.alpha < 1)
        {
            pantallaNegra.alpha += Time.deltaTime * velocidadTransicionNegra;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(nombreEscenaACargar);
    }
}