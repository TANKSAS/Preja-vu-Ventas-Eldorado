using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.WindowsMR.Input;

public class ProjectingObject : MonoBehaviour
{
    public GameObject hologramObject;
    public GameObject solidObject;
    public Material hologramObjectMaterial;
    public Material solidObjectMaterial;
    public float slowTransitionDuration = 2.5f;
    public float fastTransitionDuration = 1.5f;
    public float hologramFilledFill = 5f;
    public float solidFilledFill = -1.5f;
    public float hologramEmptyFill = -0.75f;
    public float solidEmptyFill = 0.5f;
    public bool showing = false;

    void OnEnable()
    {
        hologramObjectMaterial = hologramObject.GetComponent<MeshRenderer>().material;
        solidObjectMaterial = solidObject.GetComponent<MeshRenderer>().material;
        StartCoroutine(ShowObject());
    }

    IEnumerator ShowObject()
    {
        showing = true;
        hologramObjectMaterial.SetFloat("_Cuttoff_height", hologramEmptyFill);
        solidObjectMaterial.SetVector("_DissolveOffest", new Vector3(0,solidEmptyFill,0));

        StartCoroutine(TransitionCutoffHeight(hologramEmptyFill, hologramFilledFill,fastTransitionDuration));
        StartCoroutine(TransitionDissolveOffest(solidEmptyFill, solidFilledFill,fastTransitionDuration));   

        yield return new WaitUntil(()=> !showing);

        yield return new WaitForSeconds(3f);
        StartCoroutine(TransitionCutoffHeight( hologramFilledFill, hologramEmptyFill, slowTransitionDuration));
        StartCoroutine(TransitionDissolveOffest( solidFilledFill, solidEmptyFill, slowTransitionDuration));
        yield return new WaitForSeconds(3f);
        Debug.Log("aquicerro");
        this.gameObject.SetActive(false);
    }

    IEnumerator TransitionCutoffHeight(float startValue, float endValue, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Interpola el valor de _Cuttoff_height de startValue a endValue en el tiempo definido por duration
            float newValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            hologramObjectMaterial.SetFloat("_Cuttoff_height", newValue);

            // Incrementa el tiempo transcurrido
            elapsedTime += Time.deltaTime;

            // Espera al siguiente frame antes de continuar
            yield return null;
        }

        // Asegúrate de que el valor final sea exactamente el valor final deseado
        hologramObjectMaterial.SetFloat("_Cuttoff_height", endValue);
        Debug.Log("end transition");
    }

    IEnumerator TransitionDissolveOffest(float startValue, float endValue, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Interpola el valor de _Cuttoff_height de startValue a endValue en el tiempo definido por duration
            float newValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            solidObjectMaterial.SetVector("_DissolveOffest", new Vector3(0, newValue, 0));

            // Incrementa el tiempo transcurrido
            elapsedTime += Time.deltaTime;

            // Espera al siguiente frame antes de continuar
            yield return null;
        }

        // Asegúrate de que el valor final sea exactamente el valor final deseado
        solidObjectMaterial.SetVector("_DissolveOffest", new Vector3(0, endValue, 0));
        Debug.Log("end transition");
    }

    public void EndShowing()
    {
        showing = false;
    }
}
