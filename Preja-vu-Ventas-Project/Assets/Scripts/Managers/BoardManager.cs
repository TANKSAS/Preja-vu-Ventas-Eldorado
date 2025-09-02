using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
     // Listas de objetos
    public List<GameObject> proceedObjects;
    public List<GameObject> doObjects;
    public List<GameObject> dontObjects;

    // Puntos de spawn para cada lista
    public Transform proceedSpawnPoint;
    public Transform doSpawnPoint;
    public Transform dontSpawnPoint;

    private GameObject currentProceedObject;
    private GameObject currentDoObject;
    private GameObject currentDontObject;

    void Start()
    {
        // Inicializar todos los objetos como apagados al inicio
        DeactivateAllObjects();
    }

    // Función para desactivar todos los objetos de las tres listas
    public void DeactivateAllObjects()
    {
        foreach (GameObject obj in proceedObjects)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in doObjects)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in dontObjects)
        {
            obj.SetActive(false);
        }
    }

    // Activar objeto en la lista proceedObjects
    public void ActivateProceedObject(int index)
    {
        // Desactivar el objeto actual si existe
        if (currentProceedObject != null)
        {
            currentProceedObject.SetActive(false);
        }

        // Activar el nuevo objeto
        if (index >= 0 && index < proceedObjects.Count)
        {
            currentProceedObject = proceedObjects[index];
            currentProceedObject.transform.position = proceedSpawnPoint.position;
            currentProceedObject.SetActive(true);
        }
    }

    // Activar objeto en la lista doObjects
    public void ActivateDoObject(int index)
    {
        // Desactivar el objeto actual si existe
        if (currentDoObject != null)
        {
            currentDoObject.SetActive(false);
        }

        // Activar el nuevo objeto
        if (index >= 0 && index < doObjects.Count)
        {
            currentDoObject = doObjects[index];
            currentDoObject.transform.position = doSpawnPoint.position;
            currentDoObject.SetActive(true);
        }
    }

    // Activar objeto en la lista dontObjects
    public void ActivateDontObject(int index)
    {
        // Desactivar el objeto actual si existe
        if (currentDontObject != null)
        {
            currentDontObject.SetActive(false);
        }

        // Activar el nuevo objeto
        if (index >= 0 && index < dontObjects.Count)
        {
            currentDontObject = dontObjects[index];
            currentDontObject.transform.position = dontSpawnPoint.position;
            currentDontObject.SetActive(true);
        }
    }

    // Función para cambiar el objeto proceed
    public void ChangeProceedObject(int newIndex)
    {
        ActivateProceedObject(newIndex);
    }

    // Función para cambiar el objeto do
    public void ChangeDoObject(int newIndex)
    {
        ActivateDoObject(newIndex);
    }

    // Función para cambiar el objeto dont
    public void ChangeDontObject(int newIndex)
    {
        ActivateDontObject(newIndex);
    }
}
