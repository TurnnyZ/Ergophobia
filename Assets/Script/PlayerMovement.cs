using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private Animator animator;

    public Camera playerCamera;

    public float walkSpeed = 6f;      // ความเร็วเดินปกติ (จาก Inspector)
    public float gravity = 10f;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;    // ความเร็วย่อตัว

    public GameObject pauseMenuUI;
    public static bool GameIsPaused = false;

    public float bobbingSpeed = 1f;
    public float bobbingAmount = 0.4f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private bool canMove = true;

    private float defaultPosY = 0;
    private float timer = 0.0f;

    // ความเร็วที่ใช้จริงในเฟรมนี้ (ยืน/ย่อ)
    private float currentMoveSpeed;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        Debug.Log(animator);

        if (playerCamera != null)
            defaultPosY = playerCamera.transform.localPosition.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetPaused(!GameIsPaused);

        if (GameIsPaused)
            return;

        // 2. ทิศทางการเดิน
        float horizontalInput = Input.GetAxis("Horizontal"); // A D
        float verticalInput = Input.GetAxis("Vertical");   // W S

        Vector3 inputDir = new Vector3(horizontalInput, 0f, verticalInput);

        // ป้องกันเดินเฉียงแล้วเร็วขึ้น
        if (inputDir.magnitude > 1f)
            inputDir = inputDir.normalized;

        // แปลงจาก local เป็น world (ตามการหมุนตัวละคร)
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector3 desiredMoveDir = forward * inputDir.z + right * inputDir.x;

        // ย่อตัว (กด C) → เลือกความเร็ว
        if (Input.GetKey(KeyCode.C) && canMove)
        {
            characterController.height = crouchHeight;
            currentMoveSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            currentMoveSpeed = walkSpeed;    // ใช้ค่าจาก Inspector ตรง ๆ
        }

        float currentSpeed = canMove ? currentMoveSpeed : 0f;

        float movementDirectionY = moveDirection.y; // เก็บค่า Y เดิมไว้ก่อน
        moveDirection = desiredMoveDir * currentSpeed;
        moveDirection.y = movementDirectionY;

        // แรงโน้มถ่วง
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        // เคลื่อนที่
        characterController.Move(moveDirection * Time.deltaTime);

        // Animation เดิน
        bool isMoving = (inputDir.magnitude > 0.1f);
        if (animator != null)
            animator.SetBool("IsWalking", isMoving);

        // HEAD BOBBING
        float targetBobAmount;
        float targetBobSpeed;

        if (isMoving && characterController.isGrounded)
        {
            targetBobAmount = bobbingAmount;
            targetBobSpeed = bobbingSpeed * (currentMoveSpeed / 6f); // ใช้ currentMoveSpeed
        }
        else
        {
            targetBobAmount = bobbingAmount * 0.3f;
            targetBobSpeed = bobbingSpeed * 0.5f;
        }

        timer += Time.deltaTime * targetBobSpeed;

        float bobY = defaultPosY + Mathf.Sin(timer * 6f) * targetBobAmount * 1.2f;
        float bobX = Mathf.Sin(timer * 3f) * targetBobAmount * 0.6f;

        Vector3 targetPos = new Vector3(bobX, bobY, playerCamera.transform.localPosition.z);

        playerCamera.transform.localPosition = Vector3.Lerp(
            playerCamera.transform.localPosition,
            targetPos,
            Time.deltaTime * 10f
        );

        // มองรอบตัว
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    public void SetPaused(bool isPaused)
    {
        GameIsPaused = isPaused;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
            canMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            canMove = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
