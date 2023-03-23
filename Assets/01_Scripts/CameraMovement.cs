using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraHolderTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform mapCameraTransform;
    [SerializeField] private Transform desiredCameraTransform;
    [SerializeField] private Transform desiredMapCameraTransform;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeAmplitude;

    [SerializeField] private float smoothTime;
    private Vector3 velocity = Vector3.zero;
    private Vector3 originalCameraPosition;
    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;
    private float shakeTimer;
    private bool canShake;
    private bool combat = false;

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
        Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.StartCombat, () => combat = true);
        EventManager.AddListener(EventType.ExitCombat, () => combat = false);
        EventManager.AddListener(EventType.ShakeCamera, ShakeCamera);
    }

    private void OnDisable()
    {
        EventManager.AddListener(EventType.StartCombat, () => combat = true);
        EventManager.AddListener(EventType.ExitCombat, () => combat = false);
        EventManager.AddListener(EventType.ShakeCamera, ShakeCamera);
    }

    private void ApplyForce()
    {
        moveDirection = playerTransform.forward * verticalInput + playerTransform.right * horizontalInput;
        rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);
    }

    private void Start()
    {
        originalCameraPosition = Vector3.zero;
    }

    private void Update()
    {
        if (combat) { return; }

        if (canShake)
        {
            StartCameraShakeEffect();
        }

        MyInput();
        SpeedControl();
        CameraPosition();
    }

    private void FixedUpdate()
    {
        if (combat) { return; }
        ApplyForce();
    }

    private void CameraPosition()
    {
        Vector3 smoothedPos = Vector3.SmoothDamp(cameraHolderTransform.position, desiredCameraTransform.position, ref velocity, smoothTime);
        cameraHolderTransform.SetPositionAndRotation(smoothedPos, desiredCameraTransform.rotation);

        Vector3 mapSmoothedPos = Vector3.SmoothDamp(mapCameraTransform.position, desiredMapCameraTransform.position, ref velocity, smoothTime);
        mapCameraTransform.SetPositionAndRotation(mapSmoothedPos, desiredMapCameraTransform.rotation);
    }

    public void ShakeCamera()
    {
        canShake = true;
        shakeTimer = shakeDuration;
    }

    public void StartCameraShakeEffect()
    {
        if (shakeTimer > 0)
        {
            cameraTransform.localPosition = originalCameraPosition + Random.insideUnitSphere * shakeAmplitude;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            canShake = false;
            shakeTimer = 0f;
            cameraTransform.localPosition = originalCameraPosition;
        }
    }
}
