using UnityEngine;

public class PlantRotationManager : MonoBehaviour
{
    public static PlantRotationManager Instance;
    public Transform plantPlace;
    public float rotationSpeed = 100.0f;
    public float dragThreshold = 10.0f;
    public float maxVerticalRatio = 0.3f;

    private bool isDragging = false;
    private bool rotationEnabled;
    private Quaternion startRotation;

    private Vector2 touchStartPos;
    private bool thresholdPassed = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rotationEnabled = true;
        startRotation = plantPlace.rotation;
    }

    void Update()
    {
        if (rotationEnabled && CareManager.CareInProcess)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
                thresholdPassed = false;
                isDragging = false;
            }

            if (Input.GetMouseButton(0))
            {
                if (!thresholdPassed)
                {
                    Vector2 currentPos = Input.mousePosition;
                    Vector2 delta = currentPos - touchStartPos;

                    float horizontalDistance = Mathf.Abs(delta.x);
                    float verticalDistance = Mathf.Abs(delta.y);

                    if (horizontalDistance > dragThreshold)
                    {
                        if (verticalDistance <= horizontalDistance * maxVerticalRatio)
                        {
                            thresholdPassed = true;
                            isDragging = true;
                        }
                    }
                }

                if (isDragging)
                {
                    float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
                    plantPlace.Rotate(Vector3.up, -mouseX);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                thresholdPassed = false;
            }
        }
        else if (isDragging)
        {
            isDragging = false;
            thresholdPassed = false;
        }
    }

    public void SetRotationEnabled(bool enabled)
    {
        rotationEnabled = enabled;
        if (rotationEnabled)
        {
            plantPlace.rotation = startRotation;
        }
    }
}