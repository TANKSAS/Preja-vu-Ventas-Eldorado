using UnityEngine;
using UnityEditor;
using System.Collections;

public class ColorChanger : MonoBehaviour
{
    public Material material;
    public Color startColor = Color.white;
    public Color endColor = Color.red;
    [Range(0, 1)] public float startAlpha = 1.0f;
    [Range(0, 1)] public float endAlpha = 1.0f;
    public float transitionTime = 1.0f;

    private bool isChanging = false;
    private Coroutine colorChangeCoroutine;

    // Función para iniciar el cambio de color
    public void StartColorChange()
    {
        if (material != null && !isChanging)
        {
            if (colorChangeCoroutine != null)
                StopCoroutine(colorChangeCoroutine);

            colorChangeCoroutine = StartCoroutine(ChangeColor(material.color, endColor, startAlpha, endAlpha));
        }
    }

    // Función para revertir el cambio de color
    public void ReverseColorChange()
    {
        if (material != null && !isChanging)
        {
            if (colorChangeCoroutine != null)
                StopCoroutine(colorChangeCoroutine);

            colorChangeCoroutine = StartCoroutine(ChangeColor(material.color, startColor, endAlpha, startAlpha));
        }
    }

    // Coroutine para realizar la transición suave de color y alpha
    private IEnumerator ChangeColor(Color fromColor, Color toColor, float fromAlpha, float toAlpha)
    {
        isChanging = true;
        float elapsedTime = 0f;

        while (elapsedTime < transitionTime)
        {
            Color lerpedColor = Color.Lerp(fromColor, toColor, elapsedTime / transitionTime);
            lerpedColor.a = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / transitionTime);
            material.color = lerpedColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        toColor.a = toAlpha;
        material.color = toColor;
        isChanging = false;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ColorChanger))]
public class ColorChangerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ColorChanger colorChanger = (ColorChanger)target;

        if (GUILayout.Button("Iniciar Cambio de Color"))
        {
            colorChanger.StartColorChange();
        }

        if (GUILayout.Button("Revertir Cambio de Color"))
        {
            colorChanger.ReverseColorChange();
        }
    }
}
#endif