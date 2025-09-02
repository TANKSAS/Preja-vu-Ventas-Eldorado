using UnityEngine;

public class ZoneDetector : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("M_SafeZone"))
        {
            GameManager.Instance.trackingController.inSaveZone = true;
            
        }
        //else if (other.CompareTag("M_DangerZone"))
        //{
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("M_SafeZone"))
        {
            GameManager.Instance.trackingController.inSaveZone = false;
        }
        //else if (other.CompareTag("M_DangerZone"))
        //{
        //}
    }
}
