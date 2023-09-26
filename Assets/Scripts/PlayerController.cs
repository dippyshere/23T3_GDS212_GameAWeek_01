using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;
    [SerializeField] private float rotationSpeed = 2.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2.0f;

    [Header("References")]
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private CameraController cameraItem;

    private Rigidbody rigidBody => GetComponent<Rigidbody>();
    private CapsuleCollider capsuleCollider => GetComponent<CapsuleCollider>();
    private bool isCrouching = false;
    private bool isAiming = false;
    private float cameraRotationX = 0f;
    private float cameraRotationY = 0f;
    private bool cursorVisible = true;

    // Start is called before the first frame update
    void Start()
    {
        LockMouse();
    }

    // Update is called once per frame
    void Update()
    {
        // Camera
        cameraRotationX -= Input.GetAxis("Mouse Y") * rotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -89f, 89f);
        cameraRotationY += Input.GetAxis("Mouse X") * rotationSpeed;
        cameraObject.transform.localRotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0f);

        // Movement
        float moveSpeed = walkSpeed;

        if (Input.GetKey(KeyCode.C))
        {
            Crouch();
        }
        else
        {
            Uncrouch();
        }

        if (Input.GetMouseButton(1))
        {
            AimCamera();
        }
        else
        {
            UnaimCamera();
        }

        if (Input.GetKey(KeyCode.LeftShift) & !isCrouching)
        {
            moveSpeed = walkSpeed * sprintMultiplier;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        // set rigidbody velocity based on current transforms
        // rigidBody.velocity = transform.right * horizontalInput * moveSpeed + transform.forward * verticalInput * moveSpeed + Vector3.up * rigidBody.velocity.y;
        // set rigidbody velocity based on camera transforms (but only the y rotation)
        rigidBody.velocity = Quaternion.Euler(0, cameraObject.transform.rotation.eulerAngles.y, 0) * new Vector3(horizontalInput * moveSpeed, rigidBody.velocity.y, verticalInput * moveSpeed);

        // Jumping
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!cursorVisible)
            {
                UnlockMouse();
            }
            else
            {
                LockMouse();
            }
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
        if (!isAiming)
        {
            cameraItem.Aim();
            isAiming = true;
        }
    }

    private void UnaimCamera()
    {
        if (isAiming)
        {
            cameraItem.Unaim();
            isAiming = false;
        }
    }

    void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorVisible = true;
    }

    void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorVisible = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - new Vector3(0, -0.5f, 0), transform.position - new Vector3(0, -0.5f, 0) + Vector3.down * 0.3f);
    }
}
