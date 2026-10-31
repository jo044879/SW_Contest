using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;        // Player
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float distance = 5f;
    [SerializeField] private float minVerticalAngle = -20f;
    [SerializeField] private float maxVerticalAngle = 60f;

    private float horizontalAngle = 0f;
    private float verticalAngle = 20f;

    void LateUpdate()
    {
        if (target == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        horizontalAngle += mouseX;
        verticalAngle -= mouseY;
        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);

        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        transform.position = target.position + Vector3.up * 1.5f + offset;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
