using UnityEngine;

public class PlantRotationManager : MonoBehaviour
{
    public static PlantRotationManager Instance;
    public Transform plantPlace;
    public float rotationSpeed = 100.0f;
    private bool isDragging = false;

    private bool rotationEnabled;
    private Quaternion startRotation;

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
                isDragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
                plantPlace.Rotate(Vector3.up, -mouseX);
            }
        }
        else if (isDragging)
        {
            isDragging = false;
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
