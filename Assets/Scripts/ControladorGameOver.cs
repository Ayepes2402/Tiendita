using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorGameOver : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string nombreEscenaMenu = "Escena Menu De Inicio";

    public void BotonReintentar()
    {
        if (GameManager.Instance != null)
        {
            
            Destroy(GameManager.Instance.gameObject);
        }

        SceneManager.LoadScene("Escena_Tienda");
    }

    public void BotonMenuPrincipal()
    {
        if (GameManager.Instance != null)
        {
          
            Destroy(GameManager.Instance.gameObject);
        }

        SceneManager.LoadScene(nombreEscenaMenu);
    }
}