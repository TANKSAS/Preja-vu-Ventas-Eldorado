using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BantController : Singleton<BantController>
{
    public GameObject selectedNpc;
    private UIbantElement UIselectionNpc;
    public NPCData characterdata;

    public void Start()
    {
        UIselectionNpc = GameManager.Instance.handBANTUI.GetComponent<UIbantElement>();
        characterdata = selectedNpc.GetComponent<convaiEventsTrigger>().data;
    }

    public void resetData()
    {
        characterdata.validate = false;
        UIselectionNpc.ResetValues();
    }
    public void loadData()
    {
        Debug.Log ("data cargada");
        if(characterdata.validate == true)
        {
        UIselectionNpc.SetValueB(characterdata.BantTemporalValueB);
        UIselectionNpc.SetValueA(characterdata.BantTemporalValueA);
        UIselectionNpc.SetValueN(characterdata.BantTemporalValueN);
        UIselectionNpc.SetValueT(characterdata.BantTemporalValueT);
        }
        else
        {
        UIselectionNpc.SetValueB(0);
        UIselectionNpc.SetValueA(0);
        UIselectionNpc.SetValueN(0);
        UIselectionNpc.SetValueT(0);
        }
    }
    public void OverrideData()
    {
        characterdata = selectedNpc.GetComponent<convaiEventsTrigger>().data;
        UIselectionNpc.ReadValueB(characterdata.BantTemporalValueB);
        UIselectionNpc.ReadValueA(characterdata.BantTemporalValueA);
        UIselectionNpc.ReadValueN(characterdata.BantTemporalValueN);
        UIselectionNpc.ReadValueT(characterdata.BantTemporalValueT);
        characterdata.validate = true;
    }
}
