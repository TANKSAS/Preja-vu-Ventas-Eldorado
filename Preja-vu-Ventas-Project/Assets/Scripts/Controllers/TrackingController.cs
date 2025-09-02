using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrackingController: MonoBehaviour
{
    public Canon canonController;
    public RaycastForward raycastController;
    public ZoneDetector zoneDetectorR;
    public ZoneDetector zoneDetectorL;
    public QuadScript quadController;
    public GameObject[] detectores;
    public SessionData newSession;
    public MeshRenderer[] handsDetecterMeshRenderers; 
    public int moveHandsCounter = 0;
    public int handsSafeZonaMovCounter = 0;
    public int handsDangerMovCounter = 0;
    public int gestureHandsCounter = 0;
    public int handsPositiveGestureCounter = 0;
    public int handsNegativeGestureCounter = 0;
    public int eyesContactCounter = 0;
    public int eyesSafeZoneCounter = 0;
    public int eyesDangerZoneCounter = 0;

    public bool inSaveZone = false; 
    public bool inDangerZone = false;
    public bool isHittingDangerZone = false;
    public bool isHittingSafeZone = false;
    public bool isGettingData;

    public void UpdateSessionData()
    {
        if (gestureHandsCounter < 1)
        {
            handsNegativeGestureCounter = 1;
            handsPositiveGestureCounter = 1;
            gestureHandsCounter = handsNegativeGestureCounter + handsPositiveGestureCounter;
        }

        newSession = new SessionData(moveHandsCounter, handsSafeZonaMovCounter, handsDangerMovCounter,
        gestureHandsCounter, handsPositiveGestureCounter, handsNegativeGestureCounter, eyesContactCounter,
        eyesSafeZoneCounter, eyesDangerZoneCounter, GameManager.Instance.screenshotController.filePath);
        GameManager.Instance.playerStats.sessions.Add(newSession);
        Debug.Log("Guardo");
    }

    public void EnableHandsDetecterMeshRenderers()
    {
        for (int i = 0; i < handsDetecterMeshRenderers.Length; i++)
        {
            handsDetecterMeshRenderers[i].enabled = true;
        }
    }

    void DisableHandsDetecterMeshRenderers()
    {
        for (int i = 0; i < handsDetecterMeshRenderers.Length; i++)
        {
            handsDetecterMeshRenderers[i].enabled = false;
        }
    }


    public void StartTrainingSession(float sessionData)
    {
        moveHandsCounter = 0;
        handsSafeZonaMovCounter = 0;
        handsDangerMovCounter = 0;
        gestureHandsCounter = 0;
        handsPositiveGestureCounter = 0;
        handsNegativeGestureCounter = 0;
        eyesContactCounter = 0;
        eyesSafeZoneCounter = 0;
        eyesDangerZoneCounter = 0;
        
        for (int i = 0; i < detectores.Length; i++)
        {
            detectores[i].SetActive(true);
        }
        
        quadController.SetSize((int)sessionData);
        zoneDetectorL.gameObject.SetActive(true);
        zoneDetectorL.gameObject.SetActive(true);
        canonController.gameObject.SetActive(true);
        raycastController.gameObject.SetActive(true);
        StartCoroutine(canonController.StartToFire());
        isGettingData = true; 
        StartCoroutine(GetSessionData());
    }

    public void EndSession()
    {
        DisableHandsDetecterMeshRenderers();
        
        for (int i = 0; i < detectores.Length; i++)
        {
            detectores[i].SetActive(false);
        }

        canonController.gameObject.SetActive(false);
        raycastController.gameObject.SetActive(false);
        zoneDetectorL.gameObject.SetActive(false);
        zoneDetectorL.gameObject.SetActive(false);
        
        isGettingData = false;
        Debug.Log("EndSession");
    }

    public IEnumerator GetSessionData()
    {
        while (GameManager.Instance.finalTestController.StartAssessmentModule)
        {
            if (inSaveZone)
            {
                OnHitCounterMSafeZone();
            }
            else
            {
                OnHitCounterMDangerZone();
            }

            if (isHittingSafeZone)
            {
                EyesOnHitCounterSafeZone();
            }
            else
            {
                EyesOnHitCounterDangerZone();
            }

            yield return new WaitForSeconds(1f);
            
            moveHandsCounter = handsSafeZonaMovCounter + handsDangerMovCounter;
            eyesContactCounter = eyesSafeZoneCounter + eyesDangerZoneCounter;
            gestureHandsCounter =  handsPositiveGestureCounter + handsNegativeGestureCounter;
        }

        GameManager.Instance.screenshotController.TakeScreenShot();
        EndSession();
    }

    void EyesOnHitCounterDangerZone()
    {
        eyesDangerZoneCounter++;
    }

    void EyesOnHitCounterSafeZone()
    {
        eyesSafeZoneCounter++;
    }

    void OnHitCounterMDangerZone()
    {
        handsDangerMovCounter++;
    }

    void OnHitCounterMSafeZone()
    {
        handsSafeZonaMovCounter++;
    }

    public void PositiveGestureDetection()
    {
        handsPositiveGestureCounter++;
    }

    public void NegativeGestureDetection()
    {
        handsNegativeGestureCounter++;
    }
}
