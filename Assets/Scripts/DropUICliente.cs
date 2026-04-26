using UnityEngine;
using UnityEngine.EventSystems;

public class DropUICliente : MonoBehaviour, IDropHandler
{
    private GameManager gameManager;

    private void Start()
    {
        
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DragUICategoria categoria = eventData.pointerDrag.GetComponent<DragUICategoria>();

            
            if (categoria != null && categoria.ObtenerUnidadesReales() > 0 && gameManager != null)
            {
                
                gameManager.IntentarVender(categoria.nombreProducto);

                
                categoria.RestarInventario();
            }
        }
    }
}