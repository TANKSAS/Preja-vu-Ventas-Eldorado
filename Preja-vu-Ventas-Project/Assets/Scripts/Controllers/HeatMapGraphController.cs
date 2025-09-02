using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatMapGraphController : Graph
{
    public HeatMapGraphHolder heatMapGraphHolder;
    public Color originalColor;
    public Sprite originalSprite;

    public override void SetGraphSettings(GraphHolder newGraphHolder)
    {
        heatMapGraphHolder = ChooseGraph<HeatMapGraphHolder>(newGraphHolder);
    }

    public override IEnumerator ResetGraphHolderValues(GraphHolder graphHolder)
    {
        heatMapGraphHolder = ChooseGraph<HeatMapGraphHolder>(graphHolder);
        heatMapGraphHolder.heatMapImage.GetComponent<Image>().color = originalColor;
        heatMapGraphHolder.heatMapImage.GetComponent<Image>().sprite = originalSprite;
        heatMapGraphHolder.heatMapMaterial = null;
        heatMapGraphHolder.heatMapSprite = null;
        heatMapGraphHolder = null;
        yield return null;
    }
    
    public void SetGraphImage(int pathSessionIndex)
    {
        GameManager.Instance.screenshotController.LoadImage(GameManager.Instance.playerStats.sessions[pathSessionIndex].imagePath);
        heatMapGraphHolder.heatMapMaterial = GameManager.Instance.screenshotController.GetHeatMapMaterial();
        heatMapGraphHolder.heatMapSprite = GameManager.Instance.screenshotController.GetHeatMapSprite();
    }

    public override IEnumerator GraphMaker()
    {
        originalColor = heatMapGraphHolder.heatMapImage.GetComponent<Image>().color;
        originalSprite = heatMapGraphHolder.heatMapImage.GetComponent<Image>().sprite;
        heatMapGraphHolder.heatMapImage.GetComponent<Image>().color = Color.white;
        heatMapGraphHolder.heatMapImage.GetComponent<Image>().sprite = heatMapGraphHolder.heatMapSprite;
        yield return null;
    }

    public override void EndGraph()
    {
        originalSprite = null;
        originalColor = Color.white;
    }

}
