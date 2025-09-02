using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineController : MonoBehaviour
{
    public List<PlayableDirector> containersEsp = new List<PlayableDirector>();
    public List<PlayableDirector> containersIng = new List<PlayableDirector>();
    public List<PlayableDirector> containersPort = new List<PlayableDirector>();
    
    [SerializeField] List<PlayableDirector> playableDirectors = new List<PlayableDirector>();
    public PlayableDirector currentPlayableDirector;

    private void Awake()
    {
        GameManager.Instance.timeLineController = this;
    }

    public void SetCinematics(Language currentLenguaje)
    {
        playableDirectors.Clear();

        switch (currentLenguaje)
        {
            case Language.Español:
                playableDirectors = new List<PlayableDirector>(containersEsp);
                break;

            case Language.Ingles:
                playableDirectors = new List<PlayableDirector>(containersIng);
                break;

            case Language.Portugues:
                playableDirectors = new List<PlayableDirector>(containersPort);
                break;
        }   
    }

    public bool StatePlayable()
    {
        if (currentPlayableDirector.state == PlayState.Playing)
        {
            return true;
        }
        else
        {
            if (currentPlayableDirector.state == PlayState.Paused)
            {
                if (currentPlayableDirector.time != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public void SetPlayableDirector(int index)
    {
        currentPlayableDirector = playableDirectors[index];
    }

    public void Play()
    {
        if (currentPlayableDirector != null)
        {
            currentPlayableDirector.Play();
        }
    }

    public void Pause()
    {
        if (currentPlayableDirector.state == PlayState.Playing)
        {
            currentPlayableDirector.Pause();
        }
    }

    public void Resume()
    {
        if (currentPlayableDirector.state != PlayState.Playing)
        {
            currentPlayableDirector.Resume();
        }
    }

    public void End()
    {
        if (currentPlayableDirector.state == PlayState.Playing)
        {
            currentPlayableDirector.Stop();
        }
    }
}
