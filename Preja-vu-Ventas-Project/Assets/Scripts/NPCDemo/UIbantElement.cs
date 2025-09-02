using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIbantElement : MonoBehaviour
{
    public List<UpdateScrollValue> sliders;
    public int temporalBantValueB;
    public int temporalBantValueA;
    public int temporalBantValueN;
    public int temporalBantValueT;
    // Start is called before the first frame update
    public void ReadValueB(int value)
    {
        value = temporalBantValueB;
    }
      public void ReadValueA(int value)
    {
        value = temporalBantValueA;
    }
      public void ReadValueN(int value)
    {
        value = temporalBantValueN;
    }
      public void ReadValueT(int value)
    {
        value = temporalBantValueT;
    }

    public void ResetValues()
    {
        foreach(UpdateScrollValue element in sliders)
        {
            element.ResetValue();
        }
    }

     public void SetValueB(int value)
    {
        temporalBantValueB = value;
    }
      public void SetValueA(int value)
    {
        temporalBantValueA = value;
    }
      public void SetValueN(int value)
    {
        temporalBantValueN = value;
    }
      public void SetValueT(int value)
    {
        temporalBantValueT = value;  
    }

}
