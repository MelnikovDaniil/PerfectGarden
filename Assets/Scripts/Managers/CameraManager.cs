using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instanse;
    public Transform target;
    public Camera mainCamera;
    public float transitionDuration = 1.0f;

    [Header("Test settings")]
    public GameObject objToLook;
    public float testDistance;
    public Vector3 testOffset;


    private Coroutine currentTransition;
    private float originalOrthographicSize;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Awake()
    {
        Instanse = this;
    }

    private void Start()
    {
        originalOrthographicSize = mainCamera.orthographicSize;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }
    public void ReturnToOriginalPosition()
    {
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }

        var lookAtPosition = originalPosition + originalRotation * Vector3.forward;

        currentTransition = StartCoroutine(MoveCamera(originalPosition, lookAtPosition, originalOrthographicSize));
    }

    public void LookAtObjectPresetup()
    {
        LookAtPoint(objToLook.transform.position, testDistance, testOffset);
    }

    public void LookAtPoint(Vector3 pointToLook, float zoom, Vector3 offset)
    {
        var targetPosition = GetTargetPosition(pointToLook, offset, zoom);
        var targetOrthographicSize = originalOrthographicSize * (1.0f - zoom / 10.0f);

        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }

        currentTransition = StartCoroutine(MoveCamera(targetPosition, pointToLook, targetOrthographicSize));
    }

    private IEnumerator MoveCamera(Vector3 targetPosition, Vector3 lookAtPosition, float targetOrthographicSize)
    {
        var startPosition = transform.position;
        var startRotation = transform.rotation;
        var targetRotation = Quaternion.LookRotation(lookAtPosition - targetPosition);
        var startOrthographicSize = mainCamera.orthographicSize;

        var elapsedTime = 0.0f;

        while (elapsedTime < transitionDuration)
        {
            var t = elapsedTime / transitionDuration;
            t = Mathf.SmoothStep(0.0f, 1.0f, t);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            mainCamera.orthographicSize = Mathf.Lerp(startOrthographicSize, targetOrthographicSize, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        transform.LookAt(lookAtPosition);
        mainCamera.orthographicSize = targetOrthographicSize;
    }

    private Vector3 GetTargetPosition(Vector3 objPosition, Vector3 offset, float distance)
    {
        var targetToObject = objPosition - target.position;
        var directionInPlane = new Vector3(targetToObject.x, 0, targetToObject.z).normalized;

        var perpendicularDirection = new Vector3(-directionInPlane.z, 0, directionInPlane.x);

        var offsetPos = objPosition + perpendicularDirection * offset.z;

        if (offset != Vector3.zero)
        {
            offsetPos += directionInPlane * offset.x + Vector3.up * offset.y;
        }

        offsetPos += targetToObject.normalized * distance * 0.5f;

        return offsetPos;
    }

    public void OnDrawGizmos()
    {
        if (objToLook != null)
        {
            Gizmos.color = Color.blue;
            var targetPosition = GetTargetPosition(objToLook.transform.position, testOffset, testDistance);
            Gizmos.DrawLine(objToLook.transform.position, targetPosition);
        }
    }
}

