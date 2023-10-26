using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/IntVariable")]
public class IntVariable : ScriptableObject
{
    private int _value;
    
    public int Value { get; set; }
}
