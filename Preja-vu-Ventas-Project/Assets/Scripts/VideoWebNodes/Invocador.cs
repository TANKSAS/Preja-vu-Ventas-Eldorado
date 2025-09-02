using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//INVOCADOR
public class Invocador 
{
    Stack<INode> _nodeList;

    public Invocador()
    {
        _nodeList = new Stack<INode>();
    }
    public void AddNode(INode newNode)
    {
        newNode.Play();
        Debug.Log("Nodo Agrgado");
        _nodeList.Push(newNode);
    }

    public void RemoveNode()
    {
        if (_nodeList.Count > 0)
        {
            INode lateestNode = _nodeList.Pop();
            lateestNode.Undo();
        }
    }
}
