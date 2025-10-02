using System;
using UnityEngine;

public class ParticleHitTracker : MonoBehaviour
{
    public Action<GameObject> OnHit;

    private Rigidbody rb;
    private Rigidbody newRb;
    private bool cachedKinematic;
    private bool isActive = false;

    private void Start()
    {
        if (!gameObject.TryGetComponent(out rb))
        {
            newRb = gameObject.AddComponent<Rigidbody>();
            rb = newRb;
        }
        else
        {
            cachedKinematic = rb.isKinematic;
        }
        rb.isKinematic = true;
    }

    public void StartTracking()
    {
        isActive = true;
    }

    public void StopTracking()
    {
        isActive = false;
        if (newRb != null)
        {
            Destroy(newRb);
        }
        else
        {
            rb.isKinematic = cachedKinematic;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!isActive) return;
        OnHit?.Invoke(other);
    }

    private void OnDestroy()
    {
    }
}
