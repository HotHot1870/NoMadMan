using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Dialog", menuName = "Scriptable/Dialog", order = 5)]
public class DialogScriptable : ScriptableObject
{
    public int Id;
    public string Content;
    public List<int> NextId = new List<int>();
}
