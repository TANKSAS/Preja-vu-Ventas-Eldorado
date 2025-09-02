using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ShaderPropertyModifier : MonoBehaviour
{
    public Material targetMaterial; // Material que tiene el shader vinculado
    public string shaderProperty = "_MyProperty"; // Nombre de la propiedad del shader a modificar
    public float startValue = 0f; // Valor inicial
    public float endValue = 1f; // Valor final
    public float duration = 2f; // Duración de la interpolación

    private float elapsedTime = 0f;
    private bool isAppearing = false;
    private bool isDisappearing = false;

    void Update()
    {
        if (isAppearing)
        {
            PerformSlerp(startValue, endValue);
        }
        else if (isDisappearing)
        {
            PerformSlerp(endValue, startValue);
        }
    }

    // Función para "aparecer" (interpolar de startValue a endValue)
    public void Appear()
    {
        elapsedTime = 0f;
        isAppearing = true;
        isDisappearing = false;
    }

    // Función para "desaparecer" (interpolar de endValue a startValue)
    public void Disappear()
    {
        elapsedTime = 0f;
        isAppearing = false;
        isDisappearing = true;
    }

    // Función que realiza la interpolación
    private void PerformSlerp(float fromValue, float toValue)
    {
        if (elapsedTime < duration)
        {
            // Interpolación esférica entre los valores proporcionados
            float currentValue = Mathf.Lerp(fromValue, toValue, elapsedTime / duration);

            // Asigna el valor interpolado a la propiedad del shader
            targetMaterial.SetFloat(shaderProperty, currentValue);

            // Incrementa el tiempo transcurrido
            elapsedTime += Time.deltaTime;
        }
        else
        {
            // Detener la interpolación cuando se completa
            isAppearing = false;
            isDisappearing = false;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ShaderPropertyModifier))]
public class ShaderPropertyModifierEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShaderPropertyModifier myScript = (ShaderPropertyModifier)target;

        if (GUILayout.Button("Appear"))
        {
            myScript.Appear();
        }

        if (GUILayout.Button("Disappear"))
        {
            myScript.Disappear();
        }
    }
}
#endif
