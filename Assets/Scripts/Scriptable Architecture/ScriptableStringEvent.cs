using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObjects/ScriptableStringEvent")]
public class ScriptableStringEvent : ScriptableObject
{
    private UnityEvent<string> _event;

    private void Awake()
    {
        _event = new UnityEvent<string>();
    }

    public void Register(UnityAction<string> action)
    {
        _event.AddListener(action);
    }

    public void Unregister(UnityAction<string> action)
    {
        _event.RemoveListener(action);
    }

    public void Raise(string str)
    {
        _event.Invoke(str);
    }
}