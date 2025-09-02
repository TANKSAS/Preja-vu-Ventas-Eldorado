using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobertaAnimationController : MonoBehaviour
{
    public Animator animator;
    public BoxCollider collider;
    [SerializeField]
    private List<string> facialStringAnimation = new List<string>();

    public void SetAnimation(float horizon, float rotation)
    {
        animator.SetFloat("Velocity", horizon);
        animator.SetFloat("Rotation", rotation);
        animator.SetBool("InGround", RobertaController.Instance.isGrounded);
        animator.SetBool("PowerOn", RobertaController.Instance.isPowerOn);
    }

    public void SetTalking(bool state)
    {
        animator.SetBool("isTalking", state);
    }

    public void SetHappy()
    {
        animator.SetTrigger("Happy");
    }

    public void SetProjecting(bool state)
    {
        animator.SetBool("isProjecting", state);
    }
    
    #region Facial Animations
    
    public void PlayAnimacion2D(string animationName)
    {
        if (animator != null && facialStringAnimation.Contains(animationName))
        {
            animator.Play(animationName);
        }
        else
        {
            Debug.LogWarning("La animación con el nombre " + animationName + " no existe en la lista.");
        }
    }
    
    #endregion


    void OnTriggerEnter(Collider other)
    {
        // Detecta si el objeto ha tocado el suelo
        if (other.CompareTag("Ground"))
        {
            collider.enabled = false;  // Deshabilitar el collider para evitar más interacciones

            // Si debe recargarse, activa la animación de recarga
            if (RobertaController.Instance.isRecharging)
            {
                animator.SetTrigger("Recharge");
                RobertaController.Instance.isPowerOn = !RobertaController.Instance.isPowerOn;
                RobertaController.Instance.isGrounded = !RobertaController.Instance.isGrounded;
                Debug.Log("Ground detected: Triggering Recharge animation.");
                RobertaController.Instance.isRecharging = false; // Resetea el estado de recarga
            }

            // Si debe apagarse, activa la animación de apagado
            if (RobertaController.Instance.isTurningOff)
            {
                RobertaController.Instance.isPowerOn = !RobertaController.Instance.isPowerOn;
                RobertaController.Instance.isTurningOff = false;
                Debug.Log("PowerOn set to" + RobertaController.Instance.isPowerOn + " and isGround is " + RobertaController.Instance.isGrounded);
            }
        }
    }
}
