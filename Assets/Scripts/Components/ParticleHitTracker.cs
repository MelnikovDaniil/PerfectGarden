using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ParticleHitTracker : MonoBehaviour
{
    public Action<GameObject> OnHit;

    private Rigidbody rigidbody;
    private bool isActive = false;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rigidbody.isKinematic = true;
    }

    public void StartTracking()
    {
        isActive = true;
    }

    public void StopTracking()
    {
        isActive = false;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!isActive) return;
        OnHit?.Invoke(other);
    }

    private void OnDestroy()
    {
        Destroy(rigidbody);
    }
}
