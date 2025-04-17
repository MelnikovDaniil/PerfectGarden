using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(DragAndDrop))]
public class ShakingSoil : MonoBehaviour
{
    public ParticleSystem dirtParticles;
    public float maxRotationSpeed = 360f;
    public float minAngle = -45f;
    public float maxAngle = 45f;
    public float damping = 0.1f;
    public float stopThreshold = 0.1f;
    public float accelerationThreshold = 1f;
    public float reverseAngle = 10f;

    private bool isShaking;

    private float previousVelocityY;
    private Rigidbody _rigidbody;
    private DragAndDrop _dragAndDrop;
    private float _currentVelocityAngle;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _dragAndDrop = GetComponent<DragAndDrop>();
    }

    void Start()
    {
        previousVelocityY = _rigidbody.linearVelocity.y;
    }

    void FixedUpdate()
    {
        if (isShaking)
        {
            var currentVelocityY = _rigidbody.linearVelocity.y;
            var accelerationY = currentVelocityY - previousVelocityY;

            var targetAngle = MapVelocityToAngle(currentVelocityY);
            var currentAngle = transform.eulerAngles.z;

            if (currentVelocityY < stopThreshold && accelerationY > accelerationThreshold)
            {
                targetAngle += reverseAngle * Mathf.Sign(currentVelocityY);
                dirtParticles.Play();
            }

            targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);
            var smoothAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref _currentVelocityAngle, damping, maxRotationSpeed);
            var newRotation = Quaternion.Euler(0, 45f, smoothAngle);

            _rigidbody.MoveRotation(newRotation);

            previousVelocityY = currentVelocityY;
        }
    }

    public void StartShaking()
    {
        isShaking = true;
        if (_dragAndDrop == null)
        {
            _dragAndDrop = GetComponent<DragAndDrop>();
        }
        _dragAndDrop.enabled = true;
    }

    public void StopShaking()
    {
        isShaking = false;
        if (_dragAndDrop == null)
        {
            _dragAndDrop = GetComponent<DragAndDrop>();
        }
        _dragAndDrop.enabled = false;
    }

    private float MapVelocityToAngle(float velocityY)
    {
        float maxVelocity = 10f;
        float minVelocity = -10f;

        return Mathf.Lerp(minAngle, maxAngle, Mathf.InverseLerp(minVelocity, maxVelocity, velocityY));
    }
}