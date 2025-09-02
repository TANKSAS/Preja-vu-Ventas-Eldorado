using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Canon : MonoBehaviour
{
    public float mDelay;
    public float frecuence;
    public Transform gun;
    public Transform canon;
    public GameObject projectilesHolder;
    public GameObject projectilePrefab;
    public int poolSize = 20;
    public List<GameObject> projectilePool = new List<GameObject>();

    void Start()
    {
        CreateProjectiles();
    }

    public void CreateProjectiles()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectilesHolder.transform, false);
            projectile.SetActive(false); // Los desactivamos inicialmente
            projectilePool.Add(projectile); // Los agregamos al pool
        }
    }

    public IEnumerator StartToFire()
    {
        mDelay = frecuence;
        yield return new WaitUntil(()=> GameManager.Instance.finalTestController.StartAssessmentModule);

        while (GameManager.Instance.finalTestController.StartAssessmentModule)
        {
            mDelay--;

            if (mDelay <= 0)
            {
                FireProjectile();
                mDelay = frecuence;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    void Update()
    {
        
    }

    void FireProjectile()
    {
        // Buscar un proyectil inactivo en el pool
        GameObject projectile = GetPooledProjectile();
        if (projectile != null)
        {
            projectile.transform.position = gun.transform.position;
            projectile.transform.rotation = canon.transform.rotation;
            projectile.SetActive(true); // Activar el proyectil para reutilizarlo
        }
    }

    GameObject GetPooledProjectile()
    {
        // Encontrar el primer proyectil inactivo en la lista
        foreach (GameObject proj in projectilePool)
        {
            if (!proj.activeInHierarchy)
            {
                return proj;
            }
        }

        return null; // Si no hay proyectiles disponibles
    }
}
