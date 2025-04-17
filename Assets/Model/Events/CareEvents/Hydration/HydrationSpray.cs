using System.Threading.Tasks;
using UnityEngine;

public class HydrationSpray : MonoBehaviour
{
    public Animator animator;

    public void MoveSprayRelatively(Transform relativePlantTransform, Vector3 position)
    {
        // Rework to define bounds and center direct spray to center of bounds
        transform.position = position + relativePlantTransform.position;
        transform.localScale = new Vector2(
            transform.localScale.x,
            -Mathf.Sign(transform.position.x) * Mathf.Abs(transform.localScale.y));

        var oppositeDirection = -position;
        var angle = Mathf.Atan2(oppositeDirection.y, oppositeDirection.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
    public Vector2 GenerateSprayPosition(Vector2 sprayDistanceRange)
    {
        var sprayPositionNormalized = new Vector2(
            Random.Range(-1f, 1f),
            Random.value).normalized;
        var index = Random.Range(sprayDistanceRange.x, sprayDistanceRange.y);

        return sprayPositionNormalized * index;
    }

    public async Task MakeSprayAsync(Transform relativePlantTransform, Vector2 sprayDistanceRange)
    {
        await AnimatorHelper.PlayAnimationForTheEndAsync(animator, "Spray");
        MoveSprayRelatively(
            relativePlantTransform,
            GenerateSprayPosition(sprayDistanceRange));
    }
}
