using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformController : MonoBehaviour
{
    // Asigna estos GameObjects desde el Inspector
    public GameObject MovementAndroid;
    public GameObject MovementEditor;

    // Start is called before the first frame update
    void Awake()
    {
        
#if UNITY_ANDROID && !UNITY_EDITOR
        if (MovementAndroid != null) MovementAndroid.SetActive(true);
        if (MovementEditor != null) MovementEditor.SetActive(false);
#else
        if (MovementAndroid != null) MovementAndroid.SetActive(false);
        if (MovementEditor != null) MovementEditor.SetActive(true);
#endif
    }

}
