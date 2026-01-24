using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instanse;
    public Transform target;
    public Camera mainCamera;
    public float defaultTransitionDuration = 1.0f;

    [Header("Test settings")]
    public GameObject objToLook;
    public float testDistance;
    public Vector3 testOffset;

    [Header("Camera Shake Settings")]
    public float defaultShakeIntensity = 0.1f;
    public float defaultShakeDuration = 0.5f;
    public float defaultShakeFrequency = 10f;
    public Vector3 positionShakeMultiplier = Vector3.one;
    public Vector3 rotationShakeMultiplier = new Vector3(1f, 1f, 0.5f);

    private Coroutine currentTransition;
    private Coroutine currentShakeCoroutine;
    private float originalOrthographicSize;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 basePosition;
    private Quaternion baseRotation;
    private bool isShaking = false;

    private void Awake()
    {
        Instanse = this;
    }

    private void Start()
    {
        originalOrthographicSize = mainCamera.orthographicSize;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        basePosition = originalPosition;
        baseRotation = originalRotation;
    }

    private void Update()
    {
        if (!isShaking)
        {
            basePosition = transform.position;
            baseRotation = transform.rotation;
        }
    }

    public void ReturnToOriginalPosition(float? transitionDuration = null)
    {
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }

        var lookAtPosition = originalPosition + originalRotation * Vector3.forward;

        currentTransition = StartCoroutine(MoveCamera(originalPosition, lookAtPosition, originalOrthographicSize, transitionDuration));
    }

    public void LookAtObjectPresetup()
    {
        LookAtPoint(objToLook.transform.position, testDistance, testOffset);
    }

    public void LookAtPoint(Vector3 pointToLook, float zoom, Vector3 offset, float? transitionDuration = null)
    {
        var targetPosition = GetTargetPosition(pointToLook, offset, zoom);
        var targetOrthographicSize = originalOrthographicSize * (1.0f - zoom / 10.0f);

        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }

        currentTransition = StartCoroutine(MoveCamera(targetPosition, pointToLook, targetOrthographicSize, transitionDuration));
    }

    public void ShakeCamera()
    {
        ShakeCamera(defaultShakeIntensity, defaultShakeDuration, defaultShakeFrequency, 0f, 1f);
    }

    public void ShakeCamera(float intensity)
    {
        ShakeCamera(intensity, defaultShakeDuration, defaultShakeFrequency, 0f, 1f);
    }

    public void ShakeCamera(float intensity, float duration)
    {
        ShakeCamera(intensity, duration, defaultShakeFrequency, 0f, 1f);
    }

    public void ShakeCamera(float intensity, float duration, float frequency)
    {
        ShakeCamera(intensity, duration, frequency, 0f, 1f);
    }

    public void ShakeCamera(float intensity, float duration, float frequency, float intensityStart, float intensityEnd)
    {
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
        }
        currentShakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration, frequency, intensityStart, intensityEnd));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration, float frequency, float intensityStart, float intensityEnd)
    {
        isShaking = true;

        float elapsed = 0f;
        float noiseSeed = Random.Range(0f, 100f);

        Vector3 originalShakePosition = basePosition;
        Quaternion originalShakeRotation = baseRotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float currentIntensity = Mathf.Lerp(intensity * intensityStart, intensity * intensityEnd, elapsed / duration);

            float timeMultiplier = elapsed * frequency;

            float posX = (Mathf.PerlinNoise(noiseSeed, timeMultiplier) - 0.5f) * 2f;
            float posY = (Mathf.PerlinNoise(noiseSeed + 1f, timeMultiplier) - 0.5f) * 2f;
            float posZ = (Mathf.PerlinNoise(noiseSeed + 2f, timeMultiplier) - 0.5f) * 2f;

            float rotX = (Mathf.PerlinNoise(noiseSeed + 3f, timeMultiplier) - 0.5f) * 2f;
            float rotY = (Mathf.PerlinNoise(noiseSeed + 4f, timeMultiplier) - 0.5f) * 2f;
            float rotZ = (Mathf.PerlinNoise(noiseSeed + 5f, timeMultiplier) - 0.5f) * 2f;

            Vector3 positionOffset = new Vector3(posX, posY, posZ) * currentIntensity;
            positionOffset = Vector3.Scale(positionOffset, positionShakeMultiplier);
            transform.position = originalShakePosition + positionOffset;

            Vector3 rotationOffset = new Vector3(rotX, rotY, rotZ) * currentIntensity * 2f;
            rotationOffset = Vector3.Scale(rotationOffset, rotationShakeMultiplier);
            transform.rotation = originalShakeRotation * Quaternion.Euler(rotationOffset);

            yield return null;
        }

        float returnTime = 0.1f;
        float returnElapsed = 0f;
        Vector3 returnStartPosition = transform.position;
        Quaternion returnStartRotation = transform.rotation;

        while (returnElapsed < returnTime)
        {
            returnElapsed += Time.deltaTime;
            float t = returnElapsed / returnTime;

            transform.position = Vector3.Lerp(returnStartPosition, basePosition, t);
            transform.rotation = Quaternion.Slerp(returnStartRotation, baseRotation, t);

            yield return null;
        }

        transform.position = basePosition;
        transform.rotation = baseRotation;

        isShaking = false;
        currentShakeCoroutine = null;
    }

    private IEnumerator MoveCamera(Vector3 targetPosition, Vector3 lookAtPosition, float targetOrthographicSize, float? transitionDuration = null)
    {
        var startPosition = transform.position;
        var startRotation = transform.rotation;
        var targetRotation = Quaternion.LookRotation(lookAtPosition - targetPosition);
        var startOrthographicSize = mainCamera.orthographicSize;

        var elapsedTime = 0.0f;

        transitionDuration = transitionDuration ?? defaultTransitionDuration;

        while (elapsedTime < transitionDuration)
        {
            var t = elapsedTime / transitionDuration.Value;
            t = Mathf.SmoothStep(0.0f, 1.0f, t);

            if (!isShaking)
            {
                basePosition = Vector3.Lerp(startPosition, targetPosition, t);
                baseRotation = Quaternion.Slerp(startRotation, targetRotation, t);
                transform.position = basePosition;
                transform.rotation = baseRotation;
            }
            else
            {
                basePosition = Vector3.Lerp(startPosition, targetPosition, t);
                baseRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            }

            mainCamera.orthographicSize = Mathf.Lerp(startOrthographicSize, targetOrthographicSize, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!isShaking)
        {
            transform.position = targetPosition;
            transform.LookAt(lookAtPosition);
        }
        else
        {
            basePosition = targetPosition;
            baseRotation = Quaternion.LookRotation(lookAtPosition - targetPosition);
        }

        mainCamera.orthographicSize = targetOrthographicSize;
    }

    private Vector3 GetTargetPosition(Vector3 pointToLook, Vector3 offset, float zoom)
    {
        if (offset == Vector3.zero)
        {
            Vector3 directionToTarget = (pointToLook - transform.position).normalized;
            float distance = CalculateDistanceForZoom(zoom);
            return pointToLook - directionToTarget * distance;
        }
        else
        {
            Vector3 offsetPosition = pointToLook + offset.normalized * CalculateDistanceForZoom(zoom);
            return offsetPosition;
        }
    }

    private float CalculateDistanceForZoom(float zoom)
    {
        float baseDistance = 10f;
        return baseDistance * (1.0f - zoom / 10.0f);
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