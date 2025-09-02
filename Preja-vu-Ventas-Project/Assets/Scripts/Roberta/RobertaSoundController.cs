using System.Collections.Generic;
using UnityEngine;

public class RobertaSoundController : MonoBehaviour
{
    public List<Sound> SFX = new List<Sound>();
    public AudioSource soundSource;
    public AudioSource flySource;
    public AudioSource ThrustersSource;
    public float minVolume = 0.0001f;
    public float volumeIncreaseRate = 1.0f;  // Velocidad a la que el volumen aumenta
    public float volumeDecreaseRate = 1.0f;  // Velocidad a la que el volumen disminuye
    Vector3 previousPosition;

    public void PlayNewSFX(int indexSFX)
    {
        if (soundSource != null)
        {
            AudioClip newSFX = SFX[indexSFX].song;

            if (!soundSource.isPlaying)
            {
                soundSource.clip = newSFX;
                soundSource.Play();
            }
        }
    }

    public void AdjustThrustersVolume(float minDistanceValue, float maxVolume, Vector3 bodyPosition)
    {
        // Calcula la velocidad manualmente
        float speed = (bodyPosition - previousPosition).magnitude / Time.deltaTime;

        if (speed > minDistanceValue)
        {
            // Si el objeto se está moviendo, incrementa el volumen basado en la velocidad
            ThrustersSource.volume = Mathf.Lerp(ThrustersSource.volume, maxVolume, Time.deltaTime * volumeIncreaseRate);
        }
        else
        {
            // Si el objeto está quieto y el volumen es mayor que el mínimo, reduce el volumen al mínimo definido
            if (ThrustersSource.volume > 0.1f)
            {
                ThrustersSource.volume = Mathf.Lerp(ThrustersSource.volume, minVolume, Time.deltaTime * volumeDecreaseRate);
            }
            else
            {
                // Asegúrate de que el volumen esté dentro del rango [minVolume, maxVolume]
                ThrustersSource.volume = 0f/*Mathf.Clamp(ThrustersSource.volume, minVolume, maxVolume)*/;
            }
        }

        previousPosition = bodyPosition;
    }

    public void StartThrusters(float value)
    {
        if (value < 0)
        {
            ThrustersSource.volume = value;
        }
    }

    public void StartFly(bool state)
    {
        if (state)
        {
            flySource.Play();
        }
        else
        {
            flySource.Stop();
        }
    }

}
