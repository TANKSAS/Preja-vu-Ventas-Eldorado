using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject GetOrCreateObjectPooled(int lastIndex, List<GameObject> objectPoolList, GameObject parent, GameObject template)
    {
        if (objectPoolList == null || objectPoolList.Count == 0)
        {
            Debug.LogError("La lista de objetos del pool está vacía o es nula.");
            return null;
        }

        // Asegúrate de que lastIndex esté dentro del rango válido
        lastIndex = Mathf.Clamp(lastIndex, 0, objectPoolList.Count - 1);

        // Bucle circular que busca el siguiente objeto inactivo a partir de lastIndex
        for (int i = 0; i < objectPoolList.Count; i++)
        {
            GameObject pooledObject = objectPoolList[lastIndex];

            if (!pooledObject.activeSelf)
            {
                return pooledObject;
            }

            lastIndex = (lastIndex + 1) % objectPoolList.Count;
        }

        // Si no hay objetos inactivos, crea uno nuevo a partir del template
        Debug.Log("Instanciando uno nuevo a partir del template");

        // Instancia un nuevo objeto basado en el template
        GameObject newObject = Instantiate(template, parent.transform, false);
        newObject.name = "NewObjectPooled_" + objectPoolList.Count; // Opcional: un nombre único
        objectPoolList.Add(newObject);
        return newObject;
    }
}
