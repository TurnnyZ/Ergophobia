using UnityEngine;
using UnityEngine.UI;

public class MessageDisplay : MonoBehaviour
{
    public Text messageText;

    public bool IsShowing { get; private set; }

    public void ShowMessage(string msg, float duration)
    {
        if (messageText == null) return;

        messageText.text = msg;
        IsShowing = true;

        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), duration);
    }

    void HideMessage()
    {
        if (messageText == null) return;

        messageText.text = "";
        IsShowing = false;
    }
}
