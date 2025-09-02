using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    [SerializeField]
    List <ParticleSystem> characterEffects = new List<ParticleSystem>();
    int lastParticle;

    public void StopParticle()
    {
        characterEffects[lastParticle].Stop();
        characterEffects[lastParticle].gameObject.SetActive(false);
    }
   
    public  void StartParticle()
    {
        characterEffects[lastParticle].gameObject.SetActive(true);
        characterEffects[lastParticle].Play();
    }

    public void TurnOffAllParticles()
    {
        foreach (var particle in characterEffects)
        {
            if (particle != null)
            {
                particle.Stop();
            }
        }
    }

    public void SetParticlesIndex(int newIndex)
    {
        lastParticle = newIndex;
    }
}
