using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class DialogueController : MonoBehaviour
{
    Queue<string> sentences;
    Dialogue newDialogue;

    public bool isPlaying;

    [SerializeField] TMP_Text textBox;

    void Start()
    {
        sentences = new Queue<string>();
        //select Language
    }

    public void StartNewDialogue(Dialogue dialogue)
    {
        newDialogue = dialogue;
        StartDialogue();
        isPlaying = true;
    }

    void StartDialogue()
    {
        sentences.Clear();

        newDialogue.sequences[0] = LanguageManager.Instance.GetStringValue(newDialogue.title);
        sentences.Enqueue(newDialogue.sequences[0]);

        DisplayNextDialogue();
    }

    void DisplayNextDialogue()
    {
        SoundManager.Instance?.PlayNewSound(newDialogue.titleVoice.source);

        string sentence = sentences.Dequeue();

        StopAllCoroutines();

        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        textBox.text = "";

        foreach(char letter in sentence.ToCharArray()) 
        { 
            textBox.text += letter;
            yield return new WaitForSeconds(0.03f);
        }

        isPlaying = false;
    }

    public void EndDialogue()
    {
        sentences.Clear();
        newDialogue = null;
    }
}