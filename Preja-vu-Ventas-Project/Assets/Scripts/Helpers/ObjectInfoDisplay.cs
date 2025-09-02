using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ObjectInfoDisplay : MonoBehaviour
{
    public XRRayInteractor xrRayInteractor;  // 🔹 Referencia al XR Ray Interactor
    [SerializeField] private ProductReference lastObjectLookedAt;
    private Coroutine hideCoroutine;

    void Update()
    {
        if (xrRayInteractor == null) return;

        RaycastHit hit;
        bool hasHit = xrRayInteractor.TryGetCurrent3DRaycastHit(out hit);

        if (hasHit)
        {
            ProductReference reference = hit.collider.GetComponent<ProductReference>();

            if (reference != null)
            {
                reference.ShowUI();

                if (lastObjectLookedAt != reference)
                {
                    lastObjectLookedAt = reference;
                    if (hideCoroutine != null)
                    {
                        StopCoroutine(hideCoroutine);
                        hideCoroutine = null;
                    }
                }
            }
        }
        else
        {
            if (lastObjectLookedAt != null && hideCoroutine == null)
            {
                hideCoroutine = StartCoroutine(HideUIAfterDelay(lastObjectLookedAt, 2f));
            }
        }
    }

    private IEnumerator HideUIAfterDelay(ProductReference product, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (product != null && product.gameObject.activeInHierarchy)
        {
            product.HideUI();
        }

        hideCoroutine = null;
    }
}