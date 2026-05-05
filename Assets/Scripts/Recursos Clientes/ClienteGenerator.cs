using System.Collections.Generic;
using UnityEngine;

public class ClienteGenerator : MonoBehaviour
{
    [Header("Bancos de Clientes por Día")]
   
    public List<ClienteData> bancoDia1;
    public List<ClienteData> bancoDia2;
    public List<ClienteData> bancoDia3;
    public List<ClienteData> bancoDia4;
    public List<ClienteData> bancoDia5;


    public int clientesPorDia = 5;

   
    public List<ClienteData> ObtenerClientesDelDia(int numeroDia)
    {
        List<ClienteData> bancoSeleccionado = new List<ClienteData>();

      
        switch (numeroDia)
        {
            case 1: bancoSeleccionado = bancoDia1; break;
            case 2: bancoSeleccionado = bancoDia2; break;
            case 3: bancoSeleccionado = bancoDia3; break;
            case 4: bancoSeleccionado = bancoDia4; break;
            case 5: bancoSeleccionado = bancoDia5; break;
            default: bancoSeleccionado = bancoDia1; break; 
        }

        return SeleccionarClientesAleatorios(bancoSeleccionado);
    }

  
    private List<ClienteData> SeleccionarClientesAleatorios(List<ClienteData> banco)
    {
        List<ClienteData> clientesElegidos = new List<ClienteData>();
        List<ClienteData> copiaBanco = new List<ClienteData>(banco);

        for (int i = 0; i < clientesPorDia; i++)
        {
            if (copiaBanco.Count == 0) break;

            int indiceAleatorio = Random.Range(0, copiaBanco.Count);
            clientesElegidos.Add(copiaBanco[indiceAleatorio]);
            copiaBanco.RemoveAt(indiceAleatorio); 
        }

        return clientesElegidos;
    }
}