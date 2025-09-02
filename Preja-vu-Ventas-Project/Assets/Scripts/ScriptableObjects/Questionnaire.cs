using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Questionnaire", menuName = "ScriptableObjects/Questionnaire", order = 3)]
public class Questionnaire : ScriptableObject
{
    public List<Quest> questions = new List<Quest>();
}
