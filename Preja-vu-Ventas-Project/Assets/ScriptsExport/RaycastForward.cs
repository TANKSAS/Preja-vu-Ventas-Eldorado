using UnityEngine;
public class RaycastForward : Singleton<RaycastForward>
{
    [SerializeField] private float rayDistance;
    [SerializeField] private Color rayColor = Color.red;
    public GameObject punteroPrefab;
    private GameObject punteroActual = null;
    public LayerMask layerMask;

    void Update()
    {
        if (!GameManager.Instance.finalTestController.StartAssessmentModule)
            return;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, forward * rayDistance, rayColor);
        
        RaycastHit hit;

        if (Physics.Raycast(transform.position, forward, out hit, rayDistance, layerMask))
        {
            if (hit.collider.CompareTag("DangerZone"))
            {
                if (!GameManager.Instance.trackingController.isHittingDangerZone)
                {
                    GameManager.Instance.trackingController.isHittingDangerZone = true;
                }
            }
            else if (GameManager.Instance.trackingController.isHittingDangerZone)
            {
                GameManager.Instance.trackingController.isHittingDangerZone = false;
            }

            if (hit.collider.CompareTag("SafeZone"))
            {
                if (!GameManager.Instance.trackingController.isHittingSafeZone)
                {
                    GameManager.Instance.trackingController.isHittingSafeZone = true;
                }
            }
            else if (GameManager.Instance.trackingController.isHittingSafeZone)
            {
                GameManager.Instance.trackingController.isHittingSafeZone = false;
            }

            if (punteroActual == null)
            {
                punteroActual = Instantiate(punteroPrefab, hit.point, Quaternion.identity);
            }
            else
            {
                punteroActual.transform.position = hit.point;
            }
        }
        else
        {
            if (GameManager.Instance.trackingController.isHittingDangerZone)
            {
                GameManager.Instance.trackingController.isHittingDangerZone = false;
            }
            if (GameManager.Instance.trackingController.isHittingSafeZone)
            {
            }
        }
    }
}
