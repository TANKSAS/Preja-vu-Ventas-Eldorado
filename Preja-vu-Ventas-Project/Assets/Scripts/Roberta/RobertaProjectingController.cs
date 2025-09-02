using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobertaProjectingController : MonoBehaviour
{
    // Lista de objetos a proyectar
    public List<GameObject> projecingObjects = new List<GameObject>();
    public List<GameObject> projectingPoints = new List<GameObject>();
    // Objeto Actual a mostrar
    public int currentProjectingPositionIndex;
    public int currentObjectIndex;
    public bool isProjecting;

    //Funcion que cambia el objeto a proyectar
    public void SelectNewProjectingPosition(int index)
    {
        // Verificar si el índice es válido
        if (index >= 0 && index < projectingPoints.Count)
        {
            // Activar la proyección del nuevo objeto si el índice es válido
            currentProjectingPositionIndex = index;
        }
        else
        {
            Debug.LogError("Índice fuera de rango en ChooseNextObject");
        }
    }

    public void SelectNewProjectingObject(int index)
    {
        Debug.Log(10);
        // Verificar si el índice es válido
        if (index >= 0 && index < projecingObjects.Count)
        {
            // Activar la proyección del nuevo objeto si el índice es válido
            currentObjectIndex = index;
        }
        else
        {
            Debug.LogError("Índice fuera de rango en ChooseNextObject");
        }
    }

    #region PROJECTING
    public void ActivateProjection() //Reemplazar por el objeto a proyectar
    {
        StartCoroutine(EnableProjecting());
    }

    private IEnumerator EnableProjecting()
    {
        isProjecting = true;
        // Posicionar y rotar el objeto seleccionado
        GameObject selectedObject = projecingObjects[currentObjectIndex]; //currentProjectingIndex
        GameObject targetPoint = projectingPoints[currentProjectingPositionIndex];

        selectedObject.transform.position = targetPoint.transform.localPosition;
        //selectedObject.transform.rotation = targetPoint.transform.rotation;

        yield return new WaitForSeconds(1f);
        // Activar el objeto en la posición indexPos
        selectedObject.SetActive(true);
    }

    // Método para desactivar la proyección en un objeto
    public void DeactivateProjection()
    {
        isProjecting = false;

        if (projecingObjects[currentObjectIndex].GetComponent<ProjectingObject>())
        {
            projecingObjects[currentObjectIndex].GetComponent<ProjectingObject>().EndShowing();
        }
        else
        {
            Debug.Log("No es un objecto para proyectar");
            projecingObjects[currentObjectIndex].gameObject.SetActive(false);
        }
    }

    #endregion

}
