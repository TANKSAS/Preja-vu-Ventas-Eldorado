using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ComparativeGraphController : Graph
{
    public ComparativeGraphicHolder comparativeFirstTryGraphicHolder;
    public ComparativeGraphicHolder comparativeSecondTryGraphicHolder;

    public float overallPositiveFirstScore;
    public float overallNegativeFirstScore;
    public float overallPositiveSecondScore;
    public float overallNegativeSecondScore;
    float overallPositiveScore;
    float overallNegativeScore;
    
    SessionData firstData;
    SessionData secondData;
    
    public override void SetGraphSettings(GraphHolder graphHolder)
    {
        var chosenGraph = ChooseGraph<ComparativeGraphicHolder>(graphHolder);

        if (comparativeFirstTryGraphicHolder.finalGradeText == null && chosenGraph != null)
        {
            Debug.Log("Entro el primero");
            comparativeFirstTryGraphicHolder = chosenGraph;
        }
        else if (comparativeSecondTryGraphicHolder.finalGradeText == null && chosenGraph != null)
        {
            Debug.Log("Entro el segundo");
            comparativeSecondTryGraphicHolder = chosenGraph;
        }
        else
        {
            Debug.Log("Holders llenos");
        }
    }

    public override IEnumerator ResetGraphHolderValues(GraphHolder graphHolder)
    {
        ComparativeGraphicHolder comparativeHolder = ChooseGraph<ComparativeGraphicHolder>(graphHolder);

        comparativeHolder.finalGradeText.text = "";
        comparativeHolder.handsGesturesValueText.text = "";
        comparativeHolder.handsMovementValueText.text = "";
        comparativeHolder.eyesVisualMovValueText.text = "";

        comparativeHolder.handsGesturesHolder.gameObject.SetActive(false);
        comparativeHolder.handsMovementHolder.gameObject.SetActive(false);
        comparativeHolder.eyesVisualMovHolder.gameObject.SetActive(false);

        comparativeHolder.finalGradePositiveIndicator.SetActive(false);
        comparativeHolder.finalGradeNegativeIndicator.SetActive(false);
        comparativeHolder.handsGesturesPositiveIndicator.SetActive(false);
        comparativeHolder.handsGesturesNegativeIndicator.SetActive(false);
        comparativeHolder.handsMovementPositiveIndicator.SetActive(false);
        comparativeHolder.handsMovementNegativeIndicator.SetActive(false);
        comparativeHolder.eyesVisualMovPositiveIndicator.SetActive(false);
        comparativeHolder.eyesVisualMovNegativeIndicator.SetActive(false); ;
        yield return null;
    }

    public void SetValues()
    {
        firstData = GameManager.Instance.playerStats.sessions[GameManager.Instance.playerStats.sessions.Count - 2];
        secondData = GameManager.Instance.playerStats.sessions[GameManager.Instance.playerStats.sessions.Count - 1];
  
        // Hands movement comparison
        SetTextAndIndicators(
            comparativeFirstTryGraphicHolder.handsMovementValueText,comparativeSecondTryGraphicHolder.handsMovementValueText,
            firstData.safeMovZone,secondData.safeMovZone,
            firstData.dangerMovZone,secondData.dangerMovZone,
            comparativeSecondTryGraphicHolder.handsMovementPositiveIndicator,
            comparativeSecondTryGraphicHolder.handsMovementNegativeIndicator,
            comparativeSecondTryGraphicHolder.handsMovementIqualIndicator
        );

        // Eyes visual movement comparison
        SetTextAndIndicators(
            comparativeFirstTryGraphicHolder.eyesVisualMovValueText,
            comparativeSecondTryGraphicHolder.eyesVisualMovValueText,
            firstData.visualSafeZone, secondData.visualSafeZone,
            firstData.visualDangerZone, secondData.visualDangerZone,
            comparativeSecondTryGraphicHolder.eyesVisualMovPositiveIndicator,
            comparativeSecondTryGraphicHolder.eyesVisualMovNegativeIndicator,
            comparativeSecondTryGraphicHolder.eyesVisualMovIqualIndicator
        );

        // Hands gestures comparison
        SetTextAndIndicators(
            comparativeFirstTryGraphicHolder.handsGesturesValueText,
            comparativeSecondTryGraphicHolder.handsGesturesValueText,
            firstData.positiveGesture, secondData.positiveGesture,
            firstData.negativeGesture, secondData.negativeGesture,
            comparativeSecondTryGraphicHolder.handsGesturesPositiveIndicator,
            comparativeSecondTryGraphicHolder.handsGesturesNegativeIndicator,
            comparativeSecondTryGraphicHolder.handsGesturesIqualIndicator
        );

        // Final first grade comparison
        CalculateOverallScore(
           firstData.safeMovZone, firstData.dangerMovZone,
           firstData.visualSafeZone, firstData.visualDangerZone,
           firstData.positiveGesture, firstData.negativeGesture,
           comparativeFirstTryGraphicHolder.finalGradeText
           );

        overallPositiveFirstScore = overallPositiveScore;
        overallNegativeFirstScore = overallNegativeScore;


        CalculateOverallScore(
           secondData.safeMovZone, secondData.dangerMovZone,
           secondData.visualSafeZone, secondData.visualDangerZone,
           secondData.positiveGesture, secondData.negativeGesture,
           comparativeSecondTryGraphicHolder.finalGradeText
           );

        overallPositiveSecondScore = overallPositiveScore;
        overallNegativeSecondScore = overallNegativeScore;

        SetFinalVerdict(overallPositiveFirstScore, overallPositiveSecondScore);

    }
    
    public override IEnumerator GraphMaker()
    {
        // Example delay between showing sections, adjust as needed
        yield return StartCoroutine(AnimateComparativeHolder(comparativeFirstTryGraphicHolder));
        yield return StartCoroutine(AnimateComparativeHolder(comparativeSecondTryGraphicHolder));
    }

    void SetFinalVerdict(float firstPercentage, float secondPercentage)
    {
        if (secondPercentage > firstPercentage)
        {
            comparativeSecondTryGraphicHolder.finalVerdictText.text = LanguageManager.Instance.GetStringValue("IncreasedText"); // Cambiar texto
            comparativeSecondTryGraphicHolder.finalVerdictText.color = Color.green; // Cambiar a color verde
        }
        else if (secondPercentage < firstPercentage)
        {
            comparativeSecondTryGraphicHolder.finalVerdictText.text = LanguageManager.Instance.GetStringValue("DecreasedText"); // Cambiar texto
            comparativeSecondTryGraphicHolder.finalVerdictText.color = Color.red; // Cambiar a color rojo
        }
        else
        {
            comparativeSecondTryGraphicHolder.finalVerdictText.text = LanguageManager.Instance.GetStringValue("NoChangeText"); // Cambiar texto si son iguales
            comparativeSecondTryGraphicHolder.finalVerdictText.color = Color.white; // Color neutral
        }
    }

    void CalculateOverallScore(
    float handsSafeTime, float handsDangerTime,
    float eyesSafeTime, float eyesDangerTime,
    int handsPositiveGestures, int handsNegativeGestures,
    TMP_Text valueText)
    {
        // Calcular totales por métrica
        float moveHandsTotal = handsSafeTime + handsDangerTime;
        float eyesContactTotal = eyesSafeTime + eyesDangerTime;
        int gesturesTotal = handsPositiveGestures + handsNegativeGestures;

        // Evitar división por cero
        if (moveHandsTotal == 0 || eyesContactTotal == 0 || gesturesTotal == 0)
            throw new ArgumentException("Uno de los totales es cero, no se puede calcular el porcentaje.");

        // Calcular porcentajes positivos y negativos
        float handsPositivePercentage = (handsSafeTime / moveHandsTotal) * 100;
        float handsNegativePercentage = (handsDangerTime / moveHandsTotal) * 100;

        float eyesPositivePercentage = (eyesSafeTime / eyesContactTotal) * 100;
        float eyesNegativePercentage = (eyesDangerTime / eyesContactTotal) * 100;

        float gesturesPositivePercentage = ((float)handsPositiveGestures / gesturesTotal) * 100;
        float gesturesNegativePercentage = ((float)handsNegativeGestures / gesturesTotal) * 100;

        // Calcular calificación general positiva
        overallPositiveScore = Mathf.Round((handsPositivePercentage + eyesPositivePercentage + gesturesPositivePercentage) / 3);
        // Opcional: Calcular calificación general negativa
        overallNegativeScore = Mathf.Round((handsNegativePercentage + eyesNegativePercentage + gesturesNegativePercentage) / 3);

        Debug.Log($"Calificación general positiva: {overallPositiveScore}%");
        Debug.Log($"Calificación general negativa: {overallNegativeScore}%");

        //valueText.text = ($"{overallPositiveScore} / 100");
    }

    void SetTextAndIndicators(
        TMP_Text firstText, TMP_Text secondText,
        int positiveCountFirst, int positiveCountSecond,
        int negativeCountFirst, int negativeCountSecond,
        GameObject positiveIndicator, GameObject negativeIndicator, GameObject iqualIndicadtor)
    {
        // Calcular totales por métrica
        float totalFirst = positiveCountFirst + negativeCountFirst;
        float totalSecond = positiveCountSecond + negativeCountSecond;

        // Evitar división por cero
        if (totalFirst == 0|| totalSecond == 0)
            throw new ArgumentException("Uno de los totales es cero, no se puede calcular el porcentaje.");

        float positivePercentageFirst = (positiveCountFirst/ totalFirst) * 100;
        float positivePercentageSecond = (positiveCountSecond / totalSecond) * 100;

        // Actualiza los textos con los porcentajes
        firstText.text = $"{Mathf.Round(positiveCountFirst)} pts";
        secondText.text = $"{Mathf.Round(positiveCountSecond)} pts";

        // Verifica si los porcentajes son iguales
        bool areEqual = Mathf.Approximately(positivePercentageFirst, positivePercentageSecond);

        if (areEqual) 
            return;

        // Activa o desactiva indicadores basados en los valores
        positiveIndicator.SetActive(positivePercentageSecond >= positivePercentageFirst);
        negativeIndicator.SetActive(positivePercentageSecond < positivePercentageFirst);
        iqualIndicadtor.SetActive(positivePercentageSecond == positivePercentageFirst);
    }

    IEnumerator AnimateComparativeHolder(ComparativeGraphicHolder holder)
    {
        // Activate/deactivate components gradually for animation
        holder.finalGradeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);

        holder.handsGesturesHolder.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);

        holder.handsMovementHolder.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);

        holder.eyesVisualMovHolder.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);

        // Additional animations could be added here if needed
    }

    public override void EndGraph()
    {
        firstData = null;
        secondData = null;
    }
}
