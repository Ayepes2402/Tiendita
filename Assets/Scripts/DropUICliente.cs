using UnityEngine;
using UnityEngine.EventSystems;

public class DropUICliente : MonoBehaviour, IDropHandler
{
    private GameManager gameManager;

    private void Start()
    {
        // Busca al jefe pa' no tener que arrastrar chimbadas en el Inspector
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DragUICategoria categoria = eventData.pointerDrag.GetComponent<DragUICategoria>();

            if (categoria != null && categoria.unidadesDisponibles > 0 && gameManager != null)
            {
                // Le pasamos el chisme al GameManager de qué fue lo que le tiraron
                gameManager.IntentarVender(categoria.nombreProducto);

                // Restamos la vuelta de la repisa
                categoria.RestarInventario();
            }
        }
    }
}