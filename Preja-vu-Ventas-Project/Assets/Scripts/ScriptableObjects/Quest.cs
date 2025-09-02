using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "ScriptableObjects/Quest", order = 5)]
public class Quest : ScriptableObject
{
    public Dialogue question;
    [TextArea(3, 10)]
    public string[] answers;
    public int trueAnswer;
    public bool isAnswerCorrect;
}

