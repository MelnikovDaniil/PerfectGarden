using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(DragAndDrop))]
public class WateringCan : MonoBehaviour
{
    public ParticleSystem waterParticles;
    public float rotationAngle = 15f;
    public float rotationSpeed = 5f;

    private DragAndDrop _dragAndDrop;
    private Rigidbody _rigidbody;

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private bool isWatering = false;
    private bool ableToInteract = false;

    private void Awake()
    {
        _dragAndDrop = GetComponent<DragAndDrop>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        initialRotation = transform.rotation;
        targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + rotationAngle);
    }

    void Update()
    {
        if (ableToInteract)
        {
            Quaternion target = isWatering ? targetRotation : initialRotation;
            Quaternion newRotation = Quaternion.Lerp(_rigidbody.rotation, target, Time.deltaTime * rotationSpeed);
            _rigidbody.MoveRotation(newRotation);
        }
    }

    public void StartWaterging()
    {
        ableToInteract = true;
        if (_dragAndDrop == null)
        {
            _dragAndDrop = GetComponent<DragAndDrop>();
        }
        _dragAndDrop.enabled = true;
    }

    public void StopWatering()
    {
        ableToInteract = false;
        if (_dragAndDrop == null)
        {
            _dragAndDrop = GetComponent<DragAndDrop>();
        }
        _dragAndDrop.enabled = false;
    }

    private void OnMouseDown()
    {
        isWatering = true;
        waterParticles.Play();
    }

    private void OnMouseUp()
    {
        isWatering = false;
        waterParticles.Stop();
    }
}
