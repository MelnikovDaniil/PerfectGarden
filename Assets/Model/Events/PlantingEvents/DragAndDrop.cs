using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DragAndDrop : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 _mouseOffset;
    private Plane _dragPlane;
    private bool isActive = true;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void SetEnabled(bool enabled)
    {
        isActive = enabled;
    }

    private void OnMouseDown()
    {
        if (isActive)
        {
            // Create a plane at object position, aligned with camera forward direction
            _dragPlane = new Plane(Camera.main.transform.forward, transform.position);

            // Calculate the offset from the mouse to the object's position
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float planeDistance;
            _dragPlane.Raycast(camRay, out planeDistance);

            _mouseOffset = transform.position - camRay.GetPoint(planeDistance);
        }
    }

    private void OnMouseDrag()
    {
        if (isActive)
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float planeDistance;

            if (_dragPlane.Raycast(camRay, out planeDistance))
            {
                Vector3 newPosition = camRay.GetPoint(planeDistance) + _mouseOffset;
                _rigidbody.MovePosition(newPosition);

                // Update previous position
                Debug.Log(newPosition);
            }
        }
    }
}