using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Neuron : MonoBehaviour, INode
{
    [SerializeField] double recordingTime;
    [SerializeField] double recordingStartTime;
    [SerializeField] double recordingEndTime;
    [SerializeField] VideoClip video;
    [SerializeField] List<float> parameters;
    [SerializeField] List<Neuron> pathReference;
    [SerializeField] NodeData data;
    public void Start()
    {
        recordingTime = data.recordingTime;
        recordingStartTime = data.recordingStartTime;
        recordingEndTime = data.recordingEndTime;
        video = data.video;
        parameters = data.parameters;
        pathReference = data.pathReference;
    }
    public void Update()
    {

    }
    public void Undo()
    {

    }
    public void Play()
    {

    }
    public void Stop()
    {

    }
}
