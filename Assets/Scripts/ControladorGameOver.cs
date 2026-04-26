using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorGameOver : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string nombreEscenaMenu = "Escena Menu De Inicio";
    // Usa este si el botón es para volver a jugar desde el Día 1
    public void BotonReintentar()
    {
        if (GameManager.Instance != null)
        {
            // 1. Destruimos el GameManager viejo que tiene el día en el que perdiste
            Destroy(GameManager.Instance.gameObject);

            // 2. ¡LA SOLUCIÓN! Vaciamos la variable Instance para que el próximo GameManager nazca fresco
            GameManager.Instance = null;
        }

        // 3. Cargamos la escena de la tienda. Al no haber GameManager, se creará uno nuevo desde el Día 0.
        SceneManager.LoadScene("Escena_Tienda");
    }

    // Usa este si el botón "Salir" es para ir al Menú Principal
    public void BotonMenuPrincipal()
    {
        if (GameManager.Instance != null)
        {
            // Matamos al GameManager viejo para que no guarde los días
            Destroy(GameManager.Instance.gameObject);

            // Hacemos el mismo truco de seguridad aquí
            GameManager.Instance = null;
        }

        // Vamos a tu escena de historia / menú principal
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}