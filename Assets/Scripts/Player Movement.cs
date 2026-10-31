using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float jumpButtonGracePeriod = 0.2f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform cameraTransform;

    // 카메라 Bob 설정
    [SerializeField] private float bobFrequency = 5f;
    [SerializeField] private float bobAmplitude = 0.08f;
    [SerializeField] private float runBobFrequency = 10f;
    [SerializeField] private float runBobAmplitude = 0.15f;

    private Animator animator;
    private CharacterController characterController;
    private float ySpeed;
    private float xRotation = 0f;
    private float bobTimer = 0f;
    private Vector3 cameraOriginalLocalPos;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private bool isJumping;
    private bool isGrounded;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        cameraOriginalLocalPos = cameraTransform.localPosition;
        lastGroundedTime = Time.time;
        isGrounded = true;
        if (animator != null)
            animator.SetBool("IsGrounded", true);
    }

    void Update()
    {
        // 마우스로 카메라 상하 회전
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // WASD 이동
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentSpeed = isRunning ? runSpeed : speed;
        float inputMagnitude = Mathf.Clamp01(new Vector3(horizontalInput, 0, verticalInput).magnitude);

        Vector3 moveDirection = transform.right * horizontalInput + transform.forward * verticalInput;
        moveDirection.y = 0f;

        // 중력
        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
            if (ySpeed < 0)
                ySpeed = -2f;
        }

        if (Input.GetButtonDown("Jump"))
            jumpButtonPressedTime = Time.time;

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = 0.3f;
            isGrounded = true;
            if (animator != null)
            {
                animator.SetBool("IsGrounded", true);
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", false);
            }
            isJumping = false;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;
                isJumping = true;
                if (animator != null)
                    animator.SetBool("IsJumping", true);
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            characterController.stepOffset = 0;
            isGrounded = false;
            if (animator != null)
            {
                animator.SetBool("IsGrounded", false);
                if ((isJumping && ySpeed < 0) || ySpeed < -2)
                    animator.SetBool("IsFalling", true);
            }
        }

        // 애니메이션 파라미터
        if (animator != null)
        {
            animator.SetBool("IsMoving", inputMagnitude > 0.1f);
            animator.SetBool("IsRunning", isRunning && inputMagnitude > 0.1f);
            animator.SetBool("IsBackward", verticalInput < -0.1f);
            animator.SetBool("IsStrafingLeft", horizontalInput < -0.1f && Mathf.Abs(verticalInput) < 0.1f);
            animator.SetBool("IsStrafingRight", horizontalInput > 0.1f && Mathf.Abs(verticalInput) < 0.1f);
            animator.SetFloat("InputMagnitude", inputMagnitude, 0.05f, Time.deltaTime);
        }

        Vector3 velocity = moveDirection.normalized * currentSpeed;
        velocity.y = ySpeed;
        characterController.Move(velocity * Time.deltaTime);

        // 카메라 Bob 효과
        bool isMoving = inputMagnitude > 0.1f && characterController.isGrounded;
        if (isMoving)
        {
            float freq = isRunning ? runBobFrequency : bobFrequency;
            float amp = isRunning ? runBobAmplitude : bobAmplitude;
            bobTimer += Time.deltaTime * freq;
            Vector3 bobOffset = new Vector3(0, Mathf.Sin(bobTimer) * amp, 0);
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraOriginalLocalPos + bobOffset, Time.deltaTime * 10f);
        }
        else
        {
            bobTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraOriginalLocalPos, Time.deltaTime * 10f);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
