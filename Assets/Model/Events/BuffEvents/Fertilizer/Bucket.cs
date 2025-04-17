using System;
using UnityEngine;

public class Bucket : MonoBehaviour
{
    public event Action<float> OnWatering;
    public Action OnBucketUp;
    public float pouringSpeed = 1.0f;
    public float rotationSpeed = 5.0f;

    private bool isPouring = false;
    private Quaternion initialRotation;
    private Quaternion pouringRotation;
    private bool ableToInteract = false;

    void Start()
    {
        initialRotation = transform.localRotation;
        pouringRotation = Quaternion.Euler(0, 0, 45f);
    }

    public void SetUp()
    {
        ableToInteract = true;
    }

    public void Hide()
    {
        ableToInteract = false;
        isPouring = false;
    }

    void Update()
    {
        if (isPouring)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, pouringRotation, Time.deltaTime * rotationSpeed);
            OnWatering?.Invoke(pouringSpeed * Time.deltaTime);
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void OnMouseDown()
    {
        if (ableToInteract)
        {
            isPouring = true;
        }
    }

    private void OnMouseUp()
    {
        if (ableToInteract)
        {
            OnBucketUp?.Invoke();
            isPouring = false;
        }
    }

    public void SetColor(Color color)
    {
        var renderer = GetComponentInChildren<Renderer>();
        var material = new Material(renderer.material)
        {
            color = color
        };
        renderer.material = material;
    }
}
