using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MovementEffect : MonoBehaviour
{
    private float currentTime;
    private float effectDuration;
    private bool isEffectRunning;
    private bool isSmooth;
    private bool isLooped;
    private Vector3 targetPosition;
    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.position;
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

            
            if (progress < 0.5f)
            {
                transform.position = Vector3.Lerp(originalPosition, targetPosition, progress / 0.5f);
            }
            else
            {
                transform.position = Vector3.Lerp(targetPosition, originalPosition, (progress - 0.5f) / 0.5f);
            }

            if (currentTime > effectDuration)
            {
                if (!isLooped)
                {
                    isEffectRunning = false;
                    enabled = false;
                }
                currentTime = 0.0f;
            }
        }

    }

    public void MoveToPointAndBack(Vector3 targetPoint, float duration, bool looped = true, bool smooth = true)
    {
        if (!isEffectRunning)
        {
            targetPosition = targetPoint;
            currentTime = 0f;
            isSmooth = smooth;
            isLooped = looped;
            effectDuration = duration;
            isEffectRunning = true;
            enabled = true;
        }
    }
}
