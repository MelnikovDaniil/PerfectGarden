using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shakeable : MonoBehaviour
{
    public event Action OnShake;

    [SerializeField]
    private float shakeThreshold = 1.0f;

    [SerializeField]
    private bool isShakeEnabled = true;


    private Vector3 lastPosition;
    private float shakeAmount;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (isShakeEnabled)
        {
            shakeAmount = (transform.position - lastPosition).magnitude / Time.deltaTime;

            if (shakeAmount >= shakeThreshold)
            {
                OnShake?.Invoke();
            }

            lastPosition = transform.position;
        }
    }

    public void SetShakeEnabled(bool isEnabled)
    {
        isShakeEnabled = isEnabled;
    }
}
