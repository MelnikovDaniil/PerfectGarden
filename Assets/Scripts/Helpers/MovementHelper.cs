using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public static class MovementHelper
{
    public const int AwayMovementDistance = 12;

    public static IEnumerator MoveObjectToBasePositionRoutine(Transform obj, float movementTime, bool smooth = false)
    {
        var target = obj.parent.position;

        yield return MoveObjectToTargetRoutine(obj, target, movementTime, smooth);
    }

    public static async Task MoveObjectToBasePositionAsync(Transform obj, float movementTime, bool smooth = false)
    {
        var target = obj.parent.position;

        await MoveObjectToTargetAsync(obj, target, movementTime, smooth);
    }

    public static IEnumerator MoveObjectAwayRoutine(Transform obj, Vector3 direction, float movementTime, bool smooth = false)
    {
        var target = obj.position + direction.normalized * AwayMovementDistance;

        yield return MoveObjectToTargetRoutine(obj, target, movementTime, smooth);
    }

    public static async Task MoveObjectAwayAsync(Transform obj, Vector3 direction, float movementTime, bool smooth = false)
    {
        var target = obj.position + direction.normalized * AwayMovementDistance;

        await MoveObjectToTargetAsync(obj, target, movementTime, smooth);
    }

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

    public static async Task MoveObjectToTargetAsync(Transform obj, Vector3 target, float movementTime, bool smooth = false)
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
            await Task.Yield();
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

    public static async Task ShakeAsync(Transform obj, float magnitude, float duration)
    {
        var elapsed = 0.0f;
        var originalPosition = obj.position;

        while (elapsed < duration)
        {
            var x = Random.Range(-1f, 1f) * magnitude;
            var y = Random.Range(-1f, 1f) * magnitude;

            obj.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            await Task.Yield();
        }

        obj.position = originalPosition;
    }
}
