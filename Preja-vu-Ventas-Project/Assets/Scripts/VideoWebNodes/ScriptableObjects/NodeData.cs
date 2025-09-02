using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NodeData", order = 1)]
public class NodeData : ScriptableObject
{
    public double recordingTime;
    public double recordingStartTime;
    public double recordingEndTime;
    public VideoClip video;
    public  List<float> parameters = new List<float>();
    public List<Neuron> pathReference = new List<Neuron>();
}
