using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ParticleHitTracker : MonoBehaviour
{
    public Action<GameObject> OnHit;
    private bool isActive = false;

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
}
