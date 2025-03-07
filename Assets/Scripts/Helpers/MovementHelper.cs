using System.Collections;
using UnityEngine;

public static class MovementHelper
{
    public static IEnumerator MoveObjectToTargetRoutine(Transform obj, Vector3 target, float movementTime, bool smooth = false)
    {
        var startPosition = obj.position;
        var elapsedTime = 0f;

        while (elapsedTime < movementTime)
        {
            var t = elapsedTime / movementTime;
            if (smooth)
            {
                t = Mathf.SmoothStep(0, 1.0f, t);
            }
            obj.position = Vector3.Lerp(startPosition, target, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    public static IEnumerator ShakeRoutine(Transform obj, float magnitude, float duration)
    {
        var elapsed = 0.0f;
        var originalPosition = obj.position;
        while (elapsed < duration)
        {
            var x = Random.Range(-1f, 1f) * magnitude;
            var y = Random.Range(-1f, 1f) * magnitude;

            obj.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        obj.position = originalPosition;
    }
}
