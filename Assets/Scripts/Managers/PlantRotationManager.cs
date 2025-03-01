using UnityEngine;

public class PlantRotationManager : MonoBehaviour
{
    public Transform plantPlace;
    public float rotationSpeed = 100.0f;
    private bool isDragging = false;

    void Update()
    {
        if (CareManager.CareInProcess)
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
}
