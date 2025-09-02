using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileObject : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public Rigidbody rig;

    void OnEnable()
    {
        // Reiniciar la velocidad y rotación del rigidbody
        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;

        // Aplicar la fuerza inicial para que el proyectil se mueva
        rig.AddRelativeForce(Vector3.forward * speed, ForceMode.VelocityChange);
        // Desactiva el proyectil después de un cierto tiempo
        Invoke("Disable", lifeTime);
    }

    void Disable()
    {
        gameObject.SetActive(false); // En lugar de destruir, desactivar
    }

    void OnDisable()
    {
        CancelInvoke(); // Para evitar que llame a Disable de nuevo si es reactivado
    }
}
