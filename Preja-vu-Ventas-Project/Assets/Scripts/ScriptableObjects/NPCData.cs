using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC Data", menuName = "ScriptableObjects/NPCData", order = 6)]
public class NPCData : ScriptableObject
{
    public string name = "NPC Name";
    public bool validate = false;
    public int BantValueB = 0;
    public int BantValueA = 0;
    public int BantValueN = 0;
    public int BantValueT = 0;
    public int BantTemporalValueB = 0;
    public int BantTemporalValueA = 0;
    public int BantTemporalValueN = 0;
    public int BantTemporalValueT = 0;
}

