using System.Collections;
using UnityEngine;

public class WormPincet : MonoBehaviour
{
    public float grabTime = 0.2f;
    public float backToPositionTime = 1f;
    public float successfulGrabDelay = 1f;
    public float successfulGrabMagnitude = 1f;

    private Vector3 startPosition;

    public IEnumerator GrabRoutine(GameObject obj, bool seccessfuly)
    {
        startPosition = transform.position;
        yield return MovementHelper.MoveObjectToTargetRoutine(transform, obj.transform.position, grabTime, false);

        if (seccessfuly)
        {
            yield return MovementHelper.ShakeRoutine(transform, successfulGrabMagnitude, successfulGrabDelay);
            obj.transform.parent = transform;
            Destroy(obj, 1);
        }

        yield return MovementHelper.MoveObjectToTargetRoutine(transform, startPosition, grabTime, true);
    }
}
