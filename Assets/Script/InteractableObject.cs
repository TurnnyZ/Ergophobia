using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    public Outline outline;
    public UnityEvent onFocusEnter;
    public UnityEvent onFocusExit;
    public UnityEvent onInteract;

    void Start()
    {
        if (outline != null)
            outline.enabled = false;
    }

    public void FocusEnter()
    {
        if (outline != null)
            outline.enabled = true;

        onFocusEnter?.Invoke();
    }

    public void FocusExit()
    {
        if (outline != null)
            outline.enabled = false;

        onFocusExit?.Invoke();
    }

    public void Interact()
    {
        onInteract?.Invoke();
    }
}
