
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // necesario para manipular imágenes de iconos

public class ComparativeGraphController : Graph
{
    public ComparativeGraphicHolder comparativeFirstTryGraphicHolder;

    public float overallPositiveFirstScore;
    public float overallNegativeFirstScore;
    public float overallPositiveSecondScore;
    public float overallNegativeSecondScore;
    float overallPositiveScore;
    float overallNegativeScore;
    public TMP_Text finalVerdictText;

    SessionData firstData;
    SessionData secondData;

    //  KINESTESIA (UI: textos e icono que ya creaste en Unity)
    public TMP_Text kinesthesiaProgressText;    // muestra "+12.3%" o "-5.2%"
    public TMP_Text kinesthesiaFeedbackText;    // mensaje: Mejoró / Empeoró / Se mantuvo
    public GameObject kinesthesiaPositiveIcon;
    public GameObject kinesthesiaNegativeIcon;
    public GameObject kinesthesiaEqualIcon;
    public float kinesthesiaResult;

    //  VOICE
    public TMP_Text voiceProgressText;
    public TMP_Text voiceFeedbackText;
    public GameObject voicePositiveIcon;
    public GameObject voiceNegativeIcon;
    public GameObject voiceEqualIcon;
    public float voiceResult;

    //  HEATMAP (visión)
    public TMP_Text heatmapProgressText;
    public TMP_Text heatmapFeedbackText;
    public GameObject heatmapPositiveIcon;
    public GameObject heatmapNegativeIcon;
    public GameObject heatmapEqualIcon;
    public float heatmapResult;

    #region Original - Setters y Reset (sin modificación funcional)

    public override void SetGraphSettings(GraphHolder graphHolder)
    {
        var chosenGraph = ChooseGraph<ComparativeGraphicHolder>(graphHolder);

        if (chosenGraph == null)
            return;

        if (comparativeFirstTryGraphicHolder == null)
        {
            comparativeFirstTryGraphicHolder = chosenGraph;
            Debug.Log("Asignado FIRST TRY holder");
            return;
        }

        Debug.LogWarning("Ambos ComparativeGraphicHolder ya están asignados");
    }

    public override IEnumerator ResetGraphHolderValues(GraphHolder graphHolder)
    {
        ComparativeGraphicHolder comparativeHolder = ChooseGraph<ComparativeGraphicHolder>(graphHolder);

        comparativeHolder.finalGradeText.text = "";
        comparativeHolder.handsMovementValueText.text = "";
        comparativeHolder.eyesVisualMovValueText.text = "";

        //comparativeHolder.handsMovementHolder.gameObject.SetActive(false);
        //comparativeHolder.eyesVisualMovHolder.gameObject.SetActive(false);

        yield return null;
    }

    public override IEnumerator GraphMaker()
    {
        // Example delay between showing sections, adjust as needed
        yield return StartCoroutine(AnimateComparativeHolder(comparativeFirstTryGraphicHolder));

        StartCoroutine(LlamarFeedback());
    }

    void SetFinalVerdict()
    {
        int score = 0;
        score += ConvertResultToScore(kinesthesiaResult);
        score += ConvertResultToScore(voiceResult);
        score += ConvertResultToScore(heatmapResult);

        if (score > 0)
        {
            finalVerdictText.text = LanguageManager.Instance.GetStringValue("IncreasedText");
            finalVerdictText.color = Color.green;
        }
        else if (score < 0)
        {
            finalVerdictText.text = LanguageManager.Instance.GetStringValue("DecreasedText");
            finalVerdictText.color = Color.red;
        }
        else
        {
            finalVerdictText.text = LanguageManager.Instance.GetStringValue("NoChangeText");
            finalVerdictText.color = Color.white;
        }
        //if (secondPercentage > firstPercentage)
        //{
        //    comparativeSecondTryGraphicHolder.finalVerdictText.text = LanguageManager.Instance.GetStringValue("IncreasedText"); // Cambiar texto
        //    comparativeSecondTryGraphicHolder.finalVerdictText.color = Color.green; // Cambiar a color verde
        //}
        //else if (secondPercentage < firstPercentage)
        //{
        //    comparativeSecondTryGraphicHolder.finalVerdictText.text = LanguageManager.Instance.GetStringValue("DecreasedText"); // Cambiar texto
        //    comparativeSecondTryGraphicHolder.finalVerdictText.color = Color.red; // Cambiar a color rojo
        //}
        //else
        //{
        //    comparativeSecondTryGraphicHolder.finalVerdictText.text = LanguageManager.Instance.GetStringValue("NoChangeText"); // Cambiar texto si son iguales
        //    comparativeSecondTryGraphicHolder.finalVerdictText.color = Color.white; // Color neutral
        //}
    }

    void CalculateOverallScore(
        float handsSafeTime, float handsDangerTime,
        float eyesSafeTime, float eyesDangerTime,
        /*int handsPositiveGestures, int handsNegativeGestures,*/
        TMP_Text valueText)
    {
        // Calcular totales por métrica
        float moveHandsTotal = handsSafeTime + handsDangerTime;
        float eyesContactTotal = eyesSafeTime + eyesDangerTime;
        //int gesturesTotal = handsPositiveGestures + handsNegativeGestures;

        // Evitar división por cero
        if (moveHandsTotal == 0 || eyesContactTotal == 0 /*|| gesturesTotal == 0*/)
            throw new ArgumentException("Uno de los totales es cero, no se puede calcular el porcentaje.");

        // Calcular porcentajes positivos y negativos
        float handsPositivePercentage = (handsSafeTime / moveHandsTotal) * 100;
        float handsNegativePercentage = (handsDangerTime / moveHandsTotal) * 100;

        float eyesPositivePercentage = (eyesSafeTime / eyesContactTotal) * 100;
        float eyesNegativePercentage = (eyesDangerTime / eyesContactTotal) * 100;

        Debug.Log("El resultado positivo es: " + handsPositivePercentage);
        Debug.Log("El resultado negativo es: " + handsNegativePercentage);

        // Calcular calificación general positiva
        overallPositiveScore = Mathf.Round((handsPositivePercentage + eyesPositivePercentage) / 2/*+ gesturesPositivePercentage) / 3*/);
        // Opcional: Calcular calificación general negativa
        overallNegativeScore = Mathf.Round((handsNegativePercentage + eyesNegativePercentage) / 2/*+ gesturesNegativePercentage) / 3*/);

        Debug.Log($"Calificación general positiva: {overallPositiveScore}%");
        Debug.Log($"Calificación general negativa: {overallNegativeScore}%");

        //valueText.text = ($"{overallPositiveScore} / 100");
    }

    void SetTextAndIndicators(
        TMP_Text firstText, /*TMP_Text secondText*/
        int positiveCountFirst, int positiveCountSecond,
        int negativeCountFirst, int negativeCountSecond)
        //GameObject positiveIndicator, GameObject negativeIndicator, GameObject iqualIndicadtor)
    {
        // Calcular totales por métrica
        float totalFirst = positiveCountFirst + negativeCountFirst;
        float totalSecond = positiveCountSecond + negativeCountSecond;

        // Evitar división por cero
        if (totalFirst == 0 || totalSecond == 0)
            throw new ArgumentException("Uno de los totales es cero, no se puede calcular el porcentaje.");

        float positivePercentageFirst = (positiveCountFirst / totalFirst) * 100;
        float positivePercentageSecond = (positiveCountSecond / totalSecond) * 100;




        //// Verifica si los porcentajes son iguales
        //bool areEqual = Mathf.Approximately(positivePercentageFirst, positivePercentageSecond);

        //if (areEqual)
        //    return;

        // Activa o desactiva indicadores basados en los valores
        //positiveIndicator.SetActive(positivePercentageSecond >= positivePercentageFirst);
        //negativeIndicator.SetActive(positivePercentageSecond < positivePercentageFirst);
        //iqualIndicadtor.SetActive(positivePercentageSecond == positivePercentageFirst);
    }

    void SetIndicators(
    MetricResult result,
    GameObject positive,
    GameObject negative,
    GameObject equal)
    {
        positive.SetActive(result == MetricResult.Improved);
        negative.SetActive(result == MetricResult.Worse);
        equal.SetActive(result == MetricResult.Equal);
    }

    IEnumerator AnimateComparativeHolder(ComparativeGraphicHolder holder)
    {
        // Activate/deactivate components gradually for animation
        //holder.finalGradeText.gameObject.SetActive(true);
        //yield return new WaitForSeconds(0.8f);

        //holder.handsMovementHolder.gameObject.SetActive(true);
        //yield return new WaitForSeconds(0.8f);

        //holder.eyesVisualMovHolder.gameObject.SetActive(true);
        //yield return new WaitForSeconds(0.8f);

        yield return holder;

        // Additional animations could be added here if needed
    }

    #endregion

    // -----------------------------
    //  UTIL: Cálculo de mejora (%)
    // -----------------------------
    /// <summary>
    /// Calcula la mejora relativa en porcentaje entre dos valores.
    /// - first y second deben estar en la misma unidad (ambos ratios 0..1 o ambos % 0..100).
    /// - Si first == 0 y second > 0 y treatZeroAsFull==true devuelve 100.
    /// </summary>
    //float CalculateImprovement(float first, float second, bool treatZeroAsFull = true)
    //{
    //    // proteger contra división por cero y floting point
    //    if (Mathf.Approximately(first, 0f) && Mathf.Approximately(second, 0f))
    //        return 0f;

    //    if (Mathf.Approximately(first, 0f))
    //        return treatZeroAsFull ? 100f : (second - first) * 100f;

    //    return ((second - first) / Mathf.Abs(first)) * 100f;
    //}

    // -----------------------------
    //  BLOQUE: KINESTESIA (Manos)
    // -----------------------------
    /// <summary>
    /// Calcula la mejora de kinestesia entre firstData y secondData
    /// y actualiza los textos/iconos ya existentes en la UI.
    /// Usa SOLO: firstData.safeMovZone y firstData.dangerMovZone (y los del segundo)
    /// </summary>
    void ProcessKinesthesia()
    {
        
        if (firstData == null || secondData == null) return;

        float totalFirst = firstData.safeMovZone + firstData.dangerMovZone;
        float totalSecond = secondData.safeMovZone + secondData.dangerMovZone;

        if (totalFirst <= 0f && totalSecond <= 0f)
        {
            if (kinesthesiaProgressText != null)
                kinesthesiaProgressText.text = "N/A";

            if (kinesthesiaFeedbackText != null)
                kinesthesiaFeedbackText.text = LanguageManager.Instance.GetStringValue("General_NoData");

            //if (kinesthesiaProgressIcon != null)
            //    kinesthesiaProgressIcon.SetActive(false);

            return;
        }

        // 1️⃣ Convertir a porcentaje (0–100)
        float firstPct = totalFirst > 0f
            ? Mathf.Clamp((firstData.safeMovZone / totalFirst) * 100f, 0f, 100f)
            : 0f;

        float secondPct = totalSecond > 0f
            ? Mathf.Clamp((secondData.safeMovZone / totalSecond) * 100f, 0f, 100f)
            : 0f;

        // 2️⃣ Diferencia REAL (resultado del entrenamiento)
        float difference = secondPct - firstPct;
        kinesthesiaResult = difference;

        // 3️⃣ TEXTO NUMÉRICO (SOLO porcentaje)
        if (kinesthesiaProgressText != null)    
        {
            if (difference > 0f)
                kinesthesiaProgressText.text = $"+{difference:F1}%";
            else
                kinesthesiaProgressText.text = $"{difference:F1}%";
        }

        // 4️⃣ FEEDBACK (solo texto, con KEYS)
        if (kinesthesiaFeedbackText != null)
        {
            if (difference > 0f)
                kinesthesiaFeedbackText.text =
                    LanguageManager.Instance.GetStringValue("Kinesthesia_Feedback_Improved");

            else if (difference < 0f)
                kinesthesiaFeedbackText.text =
                    LanguageManager.Instance.GetStringValue("Kinesthesia_Feedback_Decreased");
            else
                kinesthesiaFeedbackText.text =
                    LanguageManager.Instance.GetStringValue("Kinesthesia_Feedback_Same");
        }

        // 5️⃣ ICONO
        if (kinesthesiaPositiveIcon != null)
            kinesthesiaPositiveIcon.SetActive(difference > 0f);

        if (kinesthesiaNegativeIcon != null)
            kinesthesiaNegativeIcon.SetActive(difference < 0f);

        if (kinesthesiaEqualIcon != null)
            kinesthesiaEqualIcon.SetActive(Mathf.Approximately(difference, 0f));
    }


    // -----------------------------
    //  BLOQUE: HEATMAP (Visión)
    // -----------------------------
    /// <summary>
    /// Calcula la mejora del heatmap (tiempo en zona segura) usando:
    /// firstData.visualSafeZone, firstData.visualDangerZone
    /// </summary>
    void ProcessHeatmap()
    {
        if (firstData == null || secondData == null) return;

        float totalFirst = firstData.visualSafeZone + firstData.visualDangerZone;
        float totalSecond = secondData.visualSafeZone + secondData.visualDangerZone;

        if (totalFirst <= 0f && totalSecond <= 0f)
        {
            if (heatmapProgressText != null)
                heatmapProgressText.text = "N/A";

            if (heatmapFeedbackText != null)
                heatmapFeedbackText.text = LanguageManager.Instance.GetStringValue("General_NoData");

            //if (heatmapProgressIcon != null)
            //    heatmapProgressIcon.SetActive(false);

            return;
        }

        // 1️⃣ Porcentaje de zona segura
        float firstPct = totalFirst > 0f
            ? Mathf.Clamp((firstData.visualSafeZone / totalFirst) * 100f, 0f, 100f)
            : 0f;

        float secondPct = totalSecond > 0f
            ? Mathf.Clamp((secondData.visualSafeZone / totalSecond) * 100f, 0f, 100f)
            : 0f;

        // 2️⃣ Diferencia real
        float difference = secondPct - firstPct;
        heatmapResult = difference;

        // 3️⃣ Texto numérico
        if (heatmapProgressText != null)
        {
            if (difference > 0f)
                heatmapProgressText.text = $"+{difference:F1}%";
            else
                heatmapProgressText.text = $"{difference:F1}%";
        }

        // 4️⃣ Feedback localizado
        if (heatmapFeedbackText != null)
        {
            if (difference > 0f)
                heatmapFeedbackText.text =
                    LanguageManager.Instance.GetStringValue("Heatmap_Feedback_Improved");
            else if (difference < 0f)
                heatmapFeedbackText.text =
                    LanguageManager.Instance.GetStringValue("Heatmap_Feedback_Decreased");
            else
                heatmapFeedbackText.text =
                    LanguageManager.Instance.GetStringValue("Heatmap_Feedback_Same");
        }

        // 5️⃣ Icono
        if (heatmapPositiveIcon != null)
            heatmapPositiveIcon.SetActive(difference > 0f);

        if (heatmapNegativeIcon != null)
            heatmapNegativeIcon.SetActive(difference < 0f);

        if (heatmapEqualIcon != null)
            heatmapEqualIcon.SetActive(Mathf.Approximately(difference, 0f));
    }



    // -----------------------------
    //  BLOQUE: VOICE (Tono / Frecuencia)
    // -----------------------------
    /// <summary>
    /// Intenta calcular mejora de voz a partir del rating almacenado en SessionData.
    /// Si SessionData tiene un campo enum 'toneOfVoiceRating' lo usará (convierte a int).
    /// </summary>
    void ProcessVoice()
    {
        if (firstData == null || secondData == null) return;

        int firstRating = Convert.ToInt32(firstData.toneOfVoiceRating);
        int secondRating = Convert.ToInt32(secondData.toneOfVoiceRating);

        // 🚫 Sin datos reales
        if (firstRating == 0 && secondRating == 0)
        {
            if (voiceProgressText != null)
                voiceProgressText.text = "N/A";

            if (voiceFeedbackText != null)
                voiceFeedbackText.text =
                    LanguageManager.Instance.GetStringValue("General_NoData");

             return;
        }

        // 1️⃣ Convertimos a porcentaje (0–100)
        float firstPct = Mathf.Clamp((firstRating / 3f) * 100f, 0f, 100f);
        float secondPct = Mathf.Clamp((secondRating / 3f) * 100f, 0f, 100f);

        // 2️⃣ Diferencia real después del entrenamiento
        float difference = secondPct - firstPct;
        voiceResult = difference;

        // 3️⃣ Texto numérico
        if (voiceProgressText != null)
        {
            if (difference > 0f)
                voiceProgressText.text = $"+{difference:F1}%";
            else
                voiceProgressText.text = $"{difference:F1}%";
        }

        // 4️⃣ Feedback localizado
        if (voiceFeedbackText != null)
        {
            if (difference > 0f)
                voiceFeedbackText.text =
                    LanguageManager.Instance.GetStringValue("Voice_Feedback_Improved");
            else if (difference < 0f)
                voiceFeedbackText.text =
                    LanguageManager.Instance.GetStringValue("Voice_Feedback_Decreased");
            else
                voiceFeedbackText.text =
                    LanguageManager.Instance.GetStringValue("Voice_Feedback_Same");
        }

        // 5️⃣ Icono
        if(voicePositiveIcon != null)
    voicePositiveIcon.SetActive(difference > 0f);

        if (voiceNegativeIcon != null)
            voiceNegativeIcon.SetActive(difference < 0f);

        if (voiceEqualIcon != null)
            voiceEqualIcon.SetActive(Mathf.Approximately(difference, 0f));
    }




    // ==========================================================
    // ==  MÉTODO PRINCIPAL: FINISH & MOSTRAR RESULTADOS       ==
    // ==========================================================
    public override void EndGraph()
    {
        firstData = null;
        secondData = null;
    }

    public override void ShowQualificationTag()
    {
        throw new NotImplementedException();
    }

    public override void SetGraphParameters(int index = 0)
    {
        // Cargar sesiones (primer y segundo intento)
        GraphManager.Instance.currentSessionIndex = GameManager.Instance.playerStats.GetLastSessionIndex(KindOfAssessment.Diagnosis);
        firstData = GameManager.Instance.playerStats.sessions[GraphManager.Instance.currentSessionIndex];

        GraphManager.Instance.currentSessionIndex = GameManager.Instance.playerStats.GetLastSessionIndex(KindOfAssessment.Interview);
        secondData = GameManager.Instance.playerStats.sessions[GraphManager.Instance.currentSessionIndex];

        // ---------------------------------------------------------------------
        // Comparaciones por métrica (se usa la función existente SetTextAndIndicators)
        // ---------------------------------------------------------------------

        //Hands movement comparison(actualiza textos de los holders y los indicadores visuales)
        SetTextAndIndicators(
            comparativeFirstTryGraphicHolder.handsMovementValueText,
           // comparativeSecondTryGraphicHolder.handsMovementValueText,
            firstData.safeMovZone, secondData.safeMovZone,
            firstData.dangerMovZone, secondData.dangerMovZone
            //comparativeSecondTryGraphicHolder.handsMovementPositiveIndicator,
            //comparativeSecondTryGraphicHolder.handsMovementNegativeIndicator,
            //comparativeSecondTryGraphicHolder.handsMovementIqualIndicator
        );

        //Eyes visual movement comparison
        SetTextAndIndicators(
            comparativeFirstTryGraphicHolder.eyesVisualMovValueText,
          // comparativeSecondTryGraphicHolder.eyesVisualMovValueText,
            firstData.visualSafeZone, secondData.visualSafeZone,
            firstData.visualDangerZone, secondData.visualDangerZone
        );

        //Eyes visual movement comparison
        SetTextAndIndicators(
            comparativeFirstTryGraphicHolder.eyesVisualMovValueText,
         // comparativeSecondTryGraphicHolder.eyesVisualMovValueText,
            firstData.visualSafeZone, secondData.visualSafeZone,
            firstData.visualDangerZone, secondData.visualDangerZone
        );

        // Final first grade comparison
        CalculateOverallScore(
           firstData.safeMovZone, firstData.dangerMovZone,
           firstData.visualSafeZone, firstData.visualDangerZone,
           comparativeFirstTryGraphicHolder.finalGradeText
           );

        overallPositiveFirstScore = overallPositiveScore;
        overallNegativeFirstScore = overallNegativeScore;


        CalculateOverallScore(
           secondData.safeMovZone, secondData.dangerMovZone,
           secondData.visualSafeZone, secondData.visualDangerZone,
           comparativeFirstTryGraphicHolder.finalGradeText
           );

        overallPositiveSecondScore = overallPositiveScore;
        overallNegativeSecondScore = overallNegativeScore;

       // SetFinalVerdict(overallPositiveFirstScore, overallPositiveSecondScore);

        // ==========================================================
        // ===  NUEVO: PROCESAR Y MOSTRAR PROGRESO POR CADA MÉTRICA
        // ==========================================================



    }


    MetricResult ComparePercentages(
    float positiveFirst, float negativeFirst,
    float positiveSecond, float negativeSecond)
    {
        float totalFirst = positiveFirst + negativeFirst;
        float totalSecond = positiveSecond + negativeSecond;

        if (totalFirst <= 0f || totalSecond <= 0f)
            return MetricResult.NoData;

        float pctFirst = (positiveFirst / totalFirst) * 100f;
        float pctSecond = (positiveSecond / totalSecond) * 100f;

        if (Mathf.Approximately(pctFirst, pctSecond))
            return MetricResult.Equal;

        return pctSecond > pctFirst
            ? MetricResult.Improved
            : MetricResult.Worse;
    }

    int ConvertResultToScore(float result)
    {
        if (result > 0f) return 1;
        if (result < 0f) return -1;
        return 0;
    }

    MetricResult ComparePercentages(
    int positiveFirst, int negativeFirst,
    int positiveSecond, int negativeSecond)
    {
        float totalFirst = positiveFirst + negativeFirst;
        float totalSecond = positiveSecond + negativeSecond;

        if (totalFirst == 0 || totalSecond == 0)
            return MetricResult.NoData;

        float pctFirst = (positiveFirst / totalFirst) * 100f;
        float pctSecond = (positiveSecond / totalSecond) * 100f;

        if (Mathf.Approximately(pctFirst, pctSecond))
            return MetricResult.Equal;

        return pctSecond > pctFirst
            ? MetricResult.Improved
            : MetricResult.Worse;
    }

    IEnumerator LlamarFeedback()
    {
        yield return new WaitForSeconds(1.5f);
        ProcessKinesthesia();
        yield return new WaitForSeconds(1.5f);
        // espera 1 segundo
        ProcessVoice();
        yield return new WaitForSeconds(1.5f);
        // espera otro segundo
        ProcessHeatmap();
        yield return new WaitForSeconds(1.5f);

        SetFinalVerdict();
    }
}
