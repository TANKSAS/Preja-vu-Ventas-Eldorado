using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LineRendererController : MonoBehaviour
{
    public RectTransform rectTransform;
    private LineRenderer lineRenderer;
    public float zAdjusment;
    public ScrollRect scrollRect;
    public Vector3 bottomStartEdgePosition;
    public Vector3 offset;
    private Vector2 previousScrollPosition;

    void Start()
    {
        // Obtener la referencia al LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        bottomStartEdgePosition = rectTransform.TransformPoint(new Vector3(rectTransform.rect.xMin, rectTransform.rect.yMin, -0.12f));
        previousScrollPosition = scrollRect.normalizedPosition;
        //Vector3 bottomStartEdgePosition = rectTransform.TransformPoint(new Vector3(rectTransform.rect.xMin, rectTransform.rect.yMin, -0.12f));
        //Vector3 bottomEndEdgePosition = rectTransform.TransformPoint(new Vector3(rectTransform.rect.xMax, rectTransform.rect.yMin, -0.12f));
        //lineRenderer.SetPosition(0, bottomStartEdgePosition);
        //lineRenderer.SetPosition(lineRenderer.positionCount - 1, bottomEndEdgePosition);
    }

    void LateUpdate()
    {
        // Comprueba si la posición actual del ScrollRect es diferente a la posición anterior
        if (scrollRect.normalizedPosition != previousScrollPosition)
        {
            Debug.Log("El ScrollRect se está moviendo.");
            Vector3 bottomStartEdgePosition2 = rectTransform.TransformPoint(new Vector3(rectTransform.rect.xMin, rectTransform.rect.yMin, -0.12f));
            float yDifference = /*Mathf.Abs(bottomStartEdgePosition.y - bottomStartEdgePosition2.y)*/ Vector3.Distance(bottomStartEdgePosition,bottomStartEdgePosition2);
            Vector3 adjusment = new Vector3(lineRenderer.GetPosition(0).x, lineRenderer.GetPosition(0).y, lineRenderer.GetPosition(0).z);
            adjusment.y = adjusment.y + yDifference;
            lineRenderer.SetPosition(0, adjusment);
            previousScrollPosition = scrollRect.normalizedPosition;
        }
        else
        {
        }
    }
}
