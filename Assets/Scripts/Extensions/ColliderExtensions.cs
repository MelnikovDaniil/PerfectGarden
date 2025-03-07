using UnityEngine;

public static class ColliderExtensions
{
    public static Vector3 GetRandomPointInCollider(this Collider collider)
    {
        var bounds = collider.bounds;

        for (var i = 0; i < 100; i++)
        {
            var localRandomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                0,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            var randomPoint = collider.transform.TransformPoint(localRandomPoint);
            // Check if the random point is inside the collider using Collider.ClosestPoint
            var closestPoint = collider.ClosestPoint(randomPoint);
            if (closestPoint == randomPoint) // If it's the same, it means the point is inside
            {
                return localRandomPoint;
            }
        }

        return Vector3.zero;
    }
}
