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
        // Verificar si el �ndice es v�lido
        if (index >= 0 && index < projectingPoints.Count)
        {
            // Activar la proyecci�n del nuevo objeto si el �ndice es v�lido
            currentProjectingPositionIndex = index;
        }
        else
        {
            Debug.LogError("�ndice fuera de rango en ChooseNextObject");
        }
    }

    public void SelectNewProjectingObject(int index)
    {
        Debug.Log(10);
        // Verificar si el �ndice es v�lido
        if (index >= 0 && index < projecingObjects.Count)
        {
            // Activar la proyecci�n del nuevo objeto si el �ndice es v�lido
            currentObjectIndex = index;
        }
        else
        {
            Debug.LogError("�ndice fuera de rango en ChooseNextObject");
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
        // Activar el objeto en la posici�n indexPos
        selectedObject.SetActive(true);
    }

    // M�todo para desactivar la proyecci�n en un objeto
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
