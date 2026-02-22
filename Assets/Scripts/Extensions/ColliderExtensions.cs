using UnityEngine;

public static class ColliderExtensions
{
    public static Vector3 GetRandomPointInCollider(this Collider collider)
    {
        var worldBounds = collider.bounds;

        for (var i = 0; i < 100; i++)
        {
            var randomPoint = new Vector3(
                Random.Range(-worldBounds.size.x, worldBounds.size.x) / 4,
                worldBounds.max.y,
                Random.Range(-worldBounds.size.z, worldBounds.size.z) / 4
            );
            var worldRandomPoint = randomPoint + collider.transform.position;
            var closestPoint = collider.ClosestPoint(worldRandomPoint);

            var distance = Vector3.Distance(new Vector3(closestPoint.x, 0, closestPoint.z), new Vector3(worldRandomPoint.x, 0, worldRandomPoint.z));
            if (distance <= 0.0001)
            {
                randomPoint.y = 0;
                return randomPoint;
            }
        }

        return worldBounds.center;
    }}
