using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2.0f;

    [Header("References")]
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private CameraController cameraItem;
    public SaveLoad saveLoad;

    private Rigidbody rigidBody => GetComponent<Rigidbody>();
    private CapsuleCollider capsuleCollider => GetComponent<CapsuleCollider>();
    private bool isCrouching = false;
    private bool isAiming = false;
    private float cameraRotationX = 0f;
    private float cameraRotationY = 0f;
    public bool isUIOpen = false;

    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;
    InputAction crouchAction;
    InputAction sprintAction;
    InputAction aimAction;
    InputAction photoAction;
    InputAction SDCardAction;

    public PlayRandomSound captureSound;
    public PlayRandomSound aimSound;
    public PlayRandomSound sdCardSound;

    // Start is called before the first frame update
    void Start()
    {
        LockMouse();
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");
        crouchAction = InputSystem.actions.FindAction("Crouch");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        aimAction = InputSystem.actions.FindAction("Aim");
        photoAction = InputSystem.actions.FindAction("Photo");
        SDCardAction = InputSystem.actions.FindAction("SDCard");
    }

    // Update is called once per frame
    void Update()
    {
        if (SDCardAction.WasPressedThisFrame())
        {
            sdCardSound.PlayRandomAudioClip();
            if (isUIOpen)
            {
                saveLoad.HideImages();
                LockMouse();
            }
            else
            {
                saveLoad.LoadImages();
                UnlockMouse();
            }
        }
        if (isUIOpen)
        {
            rigidBody.linearVelocity = Vector3.zero;
            return;
        }
        // Camera
        cameraRotationX -= lookAction.ReadValue<Vector2>().y;
        cameraRotationY += lookAction.ReadValue<Vector2>().x;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -88f, 88f);
        cameraObject.transform.localRotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0f);

        // Movement
        float moveSpeed = walkSpeed;

        if (crouchAction.IsInProgress())
        {
            Crouch();
        }
        else
        {
            Uncrouch();
        }

        if (aimAction.IsInProgress())
        {
            AimCamera();
        }
        else
        {
            UnaimCamera();
        }
        if (photoAction.WasPressedThisFrame() & isAiming)
        {
            captureSound.PlayRandomAudioClip();
            cameraItem.TakePhoto();
        }

        if (sprintAction.IsInProgress() & !isCrouching)
        {
            moveSpeed = walkSpeed * sprintMultiplier;
        }

        float horizontalInput = moveAction.ReadValue<Vector2>().x;
        float verticalInput = moveAction.ReadValue<Vector2>().y;
        rigidBody.linearVelocity = Quaternion.Euler(0, cameraObject.transform.rotation.eulerAngles.y, 0) * new Vector3(horizontalInput * moveSpeed, rigidBody.linearVelocity.y, verticalInput * moveSpeed);

        // Jumping
        if (jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (photoAction.WasPressedThisFrame())
        {
            LockMouse();
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position - new Vector3(0, -0.5f, 0), Vector3.down, 0.3f);
    }

    private void Crouch()
    {
        if (!isCrouching)
        {
            capsuleCollider.height = crouchHeight;
            isCrouching = true;
            transform.position = new Vector3(transform.position.x, transform.position.y - (crouchHeight / 2), transform.position.z);
        }
    }

    private void Uncrouch()
    {
        if (isCrouching)
        {
            capsuleCollider.height = standingHeight;
            isCrouching = false;
        }
    }

    private void AimCamera()
    {
        if (isAiming)
        {
            return;
        }

        aimSound.PlayRandomAudioClip();
        cameraItem.Aim();
        isAiming = true;
    }

    private void UnaimCamera()
    {
        if (!isAiming)
        {
            return;
        }

        cameraItem.Unaim();
        isAiming = false;
    }

    public void UnlockMouse()
    {
        isUIOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockMouse()
    {
        isUIOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - new Vector3(0, -0.5f, 0), transform.position - new Vector3(0, -0.5f, 0) + Vector3.down * 0.3f);
    }
}
