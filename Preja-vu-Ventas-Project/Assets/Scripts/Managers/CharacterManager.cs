using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CharacterManager : Singleton<CharacterManager>
{
    public GameObject bodyCharacter;
    public ActionBasedSnapTurnProvider turnCharacter;
    public ActionBasedContinuousMoveProvider moveCharacter;
    public GameObject playerPointOfView;

    public void SetSettupCharacterController(int index)
    {
        if (bodyCharacter != null)
        {
            switch (index)
            {
                case 0:
                    moveCharacter.enabled = false;
                    Debug.Log("Move Desactived");
                    break;
                
                case 1:
                    moveCharacter.enabled = true;
                    Debug.Log("Move Activate");
                    break;
            }
        }
    }
}
