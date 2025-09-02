using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ProductInfo", menuName = "ScriptableObjects/Quest", order = 6)]

public class ProductInfo : ScriptableObject
{
    public string objectName;
    [TextArea] public string objectDescription;
    public Sprite objectSprite;
}
