using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RotationEffect : MonoBehaviour
{
    private float currentTime;
    private float effectDuration;
    private bool isEffectRunning;
    private bool isSmooth;

    private Vector3 originalRotationEuler;
    private Vector3 targetRotationEuler;

    private void Awake()
    {
        originalRotationEuler = transform.eulerAngles;
    }

    private void Update()
    {
        if (isEffectRunning)
        {
            currentTime += Time.deltaTime;
            var progress = currentTime / effectDuration;
            if (isSmooth)
            {
                progress = Mathf.SmoothStep(0.0f, 1.0f, progress);
            }

            transform.eulerAngles = Vector3.Slerp(originalRotationEuler, targetRotationEuler, progress);

            if (currentTime > effectDuration)
            {
                isEffectRunning = false;
                enabled = false;
            }
        }
    }

    public void Rotate(Vector3 direction, float angle, float duration, bool smooth = true)
    {
        if (!isEffectRunning)
        {
            currentTime = 0f;
            isSmooth = smooth;
            effectDuration = duration;
            isEffectRunning = true;
            enabled = true;
            targetRotationEuler = originalRotationEuler + direction.normalized * angle;
        }
    }
}
