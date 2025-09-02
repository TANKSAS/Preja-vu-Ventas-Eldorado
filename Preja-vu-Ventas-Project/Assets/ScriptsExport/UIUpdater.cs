using UnityEngine;
using TMPro;  // Importamos el namespace de TextMeshPro

public class UIUpdater : Singleton<UIUpdater>
{
    public TextMeshProUGUI DangerZone;
    public TextMeshProUGUI SafeZone;
    public TextMeshProUGUI contador;
    public TextMeshProUGUI contadorM;
    public TextMeshProUGUI SafeZoneM;
    public TextMeshProUGUI DangerM;
    public TextMeshProUGUI GesturePositive;
    public TextMeshProUGUI GestureNegative;
    public TextMeshProUGUI GestureCount;

    // Update se llama una vez por frame
    void Update()
    {
        // Actualizar el texto de la UI
        contadorM.text = "Contador Movimiento: " + GameManager.Instance.trackingController.moveHandsCounter.ToString();
        SafeZoneM.text = "Movimiento SafeZone: " + GameManager.Instance.trackingController.handsSafeZonaMovCounter.ToString();
        DangerM.text = "Movimiento DangerZone: " + GameManager.Instance.trackingController.handsDangerMovCounter.ToString();

        GestureCount.text = "Contador Gestos: " + GameManager.Instance.trackingController.gestureHandsCounter.ToString();
        GesturePositive.text = "Gestos Positivos: " + GameManager.Instance.trackingController.handsPositiveGestureCounter.ToString();
        GestureNegative.text = "Gestos Negativos: " + GameManager.Instance.trackingController.handsNegativeGestureCounter.ToString();

        // Asegurarse de que raycastForward esté asignado
        if (GameManager.Instance.trackingController.raycastController != null)
        {
            // Actualizar el texto de la UI con el valor del contador
            contador.text = "visión contador: " + GameManager.Instance.trackingController.eyesContactCounter.ToString();
            DangerZone.text = "visión zona de peligro : " + GameManager.Instance.trackingController.eyesDangerZoneCounter.ToString();
            SafeZone.text = "visión zona segura: " + GameManager.Instance.trackingController.eyesSafeZoneCounter.ToString();
        }
    }
}
