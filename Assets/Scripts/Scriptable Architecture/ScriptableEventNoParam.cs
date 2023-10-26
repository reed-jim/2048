using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObjects/ScriptableEventNoParam")]
public class ScriptableEventNoParam : ScriptableObject
{
    private UnityEvent _event;

    private void Awake()
    {
        _event = new UnityEvent();
    }

    public void Register(UnityAction action)
    {
        _event.AddListener(action);
    }

    public void Unregister(UnityAction action)
    {
        _event.RemoveListener(action);
    }

    public void Raise()
    {
        _event.Invoke();
    }
}
