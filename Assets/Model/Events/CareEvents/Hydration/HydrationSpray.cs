using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HydrationSpray : MonoBehaviour
{
    public Animator animator;
    public Transform hintPlace;

    public void MoveSprayRelatively(Transform relativePlantTransform, Vector3 position, Vector3 offset)
    {
        // Rework to define bounds and center direct spray to center of bounds
        transform.position = position + relativePlantTransform.position + offset;
        transform.localRotation = Quaternion.Inverse(Quaternion.Euler(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y - 45, transform.parent.eulerAngles.z));
        transform.localScale = new Vector2(
            transform.localScale.x,
            -Mathf.Sign(transform.position.x) * Mathf.Abs(transform.localScale.y));

        var shouldRotate = position.x > 0 ? 180 : 0;
        var oppositeDirection = -position;
        var angle = Mathf.Atan2(oppositeDirection.y, oppositeDirection.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(shouldRotate, transform.localRotation.eulerAngles.y, position.x <= 0 ? angle :  360 - angle);
    }
    public Vector2 GenerateSprayPosition(Vector2 sprayDistanceRange)
    {
        var sprayPositionNormalized = new Vector2(
            Random.Range(-1f, 1f),
            Random.value).normalized;
        var index = Random.Range(sprayDistanceRange.x, sprayDistanceRange.y);

        return sprayPositionNormalized * index;
    }

    public async Task MakeSprayAsync(Transform relativePlantTransform, Vector2 sprayDistanceRange, Vector2 offset)
    {
        await AnimatorHelper.PlayAnimationForTheEndAsync(animator, "Spray");
        MoveSprayRelatively(
            relativePlantTransform,
            GenerateSprayPosition(sprayDistanceRange),
            offset);
    }
}
