using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform desiredCameraTransform;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody rb;

    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private float smoothTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput == 0 && verticalInput == 0)
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void ApplyForce()
    {
        moveDirection = playerTransform.forward * verticalInput + playerTransform.right * horizontalInput;
        rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);
    }

    private void Update()
    {
        MyInput();
        SpeedControl();
    }

    private void FixedUpdate()
    {
        ApplyForce();
        CameraPosition();
    }

    private void CameraPosition()
    {
        Vector3 smoothedPos = Vector3.SmoothDamp(cameraTransform.position, desiredCameraTransform.position, ref velocity, smoothTime);
        cameraTransform.position = smoothedPos;
        cameraTransform.SetPositionAndRotation(smoothedPos, desiredCameraTransform.rotation);
    }
}
