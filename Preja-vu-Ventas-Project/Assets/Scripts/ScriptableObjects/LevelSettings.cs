using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewLevelSetting", menuName = "ScriptableObjects/LevelSetting", order = 4)]
public class LevelSettings : ScriptableObject
{
    public Button buttonAction;
    public GameObject blockPanel;
    public GameObject progressInfoPanel;
    public Image iconPanel;
    public TMP_Text buttonText;
    public List<Animator> startsAnimators = new List<Animator>(3);
    public ButtonStyle buttonStyle;
}

[Serializable]
public class ButtonStyle
{
    public Sprite icon;
    public string keyName;
}
