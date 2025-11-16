using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    public float interactRange = 3f;
    public LayerMask interactLayer;
    public Image crosshairImage;
    public GameObject interactPrompt;
    public MessageDisplay messageDisplay;

    private InteractableObject currentObject;

    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        InteractableObject hitObj = null;

        if (Physics.Raycast(ray, out hit, interactRange, interactLayer))
        {
            hitObj = hit.collider.GetComponent<InteractableObject>();
        }

        // ถ้าเปลี่ยนเป้าหมายที่เล็ง
        if (hitObj != currentObject)
        {
            if (currentObject != null)
            {
                currentObject.FocusExit();      // ปิด outline อันเก่า
                interactPrompt.SetActive(false);
            }

            currentObject = hitObj;

            if (currentObject != null && (messageDisplay == null || !messageDisplay.IsShowing))
            {
                currentObject.FocusEnter();     // เปิด outline อันใหม่
                interactPrompt.SetActive(true); // โชว์ปุ่ม E เฉพาะเมื่อไม่มีข้อความ
            }
        }
        else
        {
            // เล็งอันเดิมอยู่: แค่เปิด/ปิดปุ่ม E ตามสถานะข้อความ
            if (currentObject != null)
            {
                bool canShowPrompt = (messageDisplay == null || !messageDisplay.IsShowing);
                interactPrompt.SetActive(canShowPrompt);
            }
        }

        // กด E
        if (currentObject != null && Input.GetKeyDown(KeyCode.E))
        {
            // ถ้ากำลังโชว์ข้อความอยู่ ไม่ให้กดซ้ำ
            if (messageDisplay != null && messageDisplay.IsShowing)
                return;

            currentObject.Interact();

            // ถ้า Interact แล้วมีข้อความขึ้น → ปิดปุ่ม E ทันที
            if (messageDisplay != null && messageDisplay.IsShowing)
            {
                interactPrompt.SetActive(false);
            }
        }
    }
}
