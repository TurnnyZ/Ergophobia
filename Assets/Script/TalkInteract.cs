using UnityEngine;

public class TalkInteract : MonoBehaviour
{
    public MessageDisplay messageDisplay; // อ้างถึง TalkDisplay
    public string message = "Hello";
    public float duration = 2f;          // ปรับได้ใน Inspector ของ object นี้

    public void ShowTalk()
    {
        if (messageDisplay != null)
        {
            messageDisplay.ShowMessage(message, duration);
        }
    }
}
