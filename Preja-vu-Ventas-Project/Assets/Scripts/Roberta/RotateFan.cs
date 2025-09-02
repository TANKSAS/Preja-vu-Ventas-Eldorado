using UnityEngine;

public class RotateFan : MonoBehaviour
{
    // Velocidad de rotación en grados por segundo.
    public float rotationSpeed = 100.0f;
    public bool power;

    // Eje de rotación (puedes cambiar el valor a (0, 1, 0) para el eje Y, por ejemplo).
    public Vector3 rotationAxis = Vector3.up;

    // Update se llama una vez por frame.
    void Update()
    {
        if (power)
        {
            // Calcula la cantidad de rotación para este frame.
            float rotationAmount = rotationSpeed * Time.deltaTime;

            // Aplica la rotación al objeto.
            transform.Rotate(rotationAxis, rotationAmount);
        }
    }

    void PowerOff()
    {
        power = false;
    }
    void PowerOn()
    {
        power = true;
    }
}

