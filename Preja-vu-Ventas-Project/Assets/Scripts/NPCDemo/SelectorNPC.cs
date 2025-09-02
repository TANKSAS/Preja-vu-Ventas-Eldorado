using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNPC : MonoBehaviour
{
    public List<GameObject> NPCs;

    public void Awake()
    {
        CurrentNpc(0);
    }

    public void CurrentNpc(int index)
    {
        Debug.Log ("el NPC seleccionado "+ index);
        if (NPCs == null || NPCs.Count == 0)
        {
            Debug.LogWarning("La lista de NPCs está vacía o no ha sido asignada.");
            return;
        }

        if (index < 0 || index >= NPCs.Count)
        {
            Debug.LogWarning("Índice fuera de rango: " + index);
            return;
        }

        // Desactiva todos los NPCs
        foreach (GameObject npc in NPCs)
        {
            if (npc != null)
                npc.SetActive(false);
        }

        // Activa solo el NPC en el índice dado
        if (NPCs[index] != null)
        {
            NPCs[index].SetActive(true);
            BantController.Instance.selectedNpc = NPCs[index];
            BantController.Instance.characterdata = NPCs[index].GetComponent<convaiEventsTrigger>().data;
        }
            
    }
}