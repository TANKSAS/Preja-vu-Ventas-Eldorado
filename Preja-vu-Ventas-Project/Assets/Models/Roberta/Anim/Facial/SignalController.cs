using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SignalController : MonoBehaviour
{
    // Referencia al Animator
    private Animator animator;

    // Lista de nombres de animaciones
    [SerializeField]
    private List<string> animaciones;

    [SerializeField]
    private List<ParticleSystem> particles;

    [SerializeField]
    private GameObject proyector;

    void Start()
    {
        // Obtener el componente Animator
        animator = GetComponent<Animator>();

        // Apagar todas las partículas al inicio
        ApagarTodasLasParticulas();

        DeactivateProyection();

    }

    public void PlayParticle(int indiceParticula)
    {
        if (particles != null)
        {

            if (indiceParticula >= 0 && indiceParticula < particles.Count)
            {
                IniciarParticula(indiceParticula);
            }
        }
        else
        {
            Debug.LogWarning("La particula con el nombre " + indiceParticula + " no existe en la lista.");
        }
    }

    // Activa y desactiva las particuals de proyección
    public void ActivateProyection()
    {
        proyector.SetActive(true);
         StartCoroutine(DetenerProyeccion());
    }

    public void DeactivateProyection()
    {
        proyector.SetActive(false);
    }


    // Apaga todas las partículas al inicio
    private void ApagarTodasLasParticulas()
    {
        foreach (var particula in particles)
        {
            if (particula != null)
            {
                particula.Stop();
            }
        }
    }

    // Función para iniciar una partícula
    private void IniciarParticula(int indice)
    {
        if (particles[indice] != null)
        {
            particles[indice].Play();
            StartCoroutine(DetenerParticula(indice));
        }
    }

    // Corrutina para detener una partícula una vez que termina
    private IEnumerator DetenerParticula(int indice)
    {
        if (particles[indice] != null)
        {
            yield return new WaitForSeconds(particles[indice].main.duration);
            particles[indice].Stop();
        }
    }
    public IEnumerator DetenerProyeccion()

    {
        yield return new WaitForSeconds(3);
        DeactivateProyection();
    }


    // Función para reproducir una animación por su nombre
    public void PlayAnimacion2D(string nombreAnimacion)
    {
        if (animator != null && animaciones.Contains(nombreAnimacion))
        {
            animator.Play(nombreAnimacion);
        }
        else
        {
            Debug.LogWarning("La animación con el nombre " + nombreAnimacion + " no existe en la lista.");
        }
    }


    // Función para reproducir la segunda animación
    public void Eye_PowerUP()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[0]);
        }
    }
    public void Eye_Blink()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[1]);
        }
    }
    public void Talking()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[2]);
        }
    }
    public void Eye_Timer()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[3]);
        }
    }
    public void Eye_Angry()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[4]);
        }
    }
    public void Eye_Angry_Glitch()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[5]);
        }
    }
    public void Eye_Blink2()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[6]);
        }
    }
    public void Eye_Charging()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[7]);
        }
    }
    public void Eye_Glitch()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[8]);
        }
    }
    public void Eye_Love()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[9]);
        }
    }
    public void Eye_PowerDown()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[10]);
        }
    }
    public void Eye_Proyecting()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[11]);
        }
    }
    public void Eye_Sad()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[12]);
        }
    }
    public void Eye_Sad_Talk()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[13]);
        }
    }
    public void Eye_Smile()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[14]);
        }
    }
    public void Eye_Smile_Glitch()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[15]);
        }
    }
    public void Eye_Smile_Talk()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[16]);
        }
    }
    public void Eye_Talk_Glitch()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[17]);
        }
    }


    // Animaciones Corporales

    public void CelebrationMovement()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[18]);
        }
    }
    public void CrazyMovement()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[19]);
        }
    }
    public void SadMovement()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[20]);
        }
    }
    public void SpeakingMovement()
    {
        if (animator != null)
        {
            PlayAnimacion2D(animaciones[21]);
        }
    }


    // Particulas
    public void Confetti()
    {
        if (particles != null)
        {
            IniciarParticula(0);
        }
    }
    public void Stars()
    {
        if (particles != null)
        {
            IniciarParticula(1);
        }
    }
    public void Rain()
    {
        if (particles != null)
        {
            IniciarParticula(2);
        }
    }
}
