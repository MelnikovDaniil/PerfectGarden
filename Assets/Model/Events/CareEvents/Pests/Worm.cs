using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Worm : MonoBehaviour
{
    public Animator animator;
    [NonSerialized] public bool isCatched;

    private bool isAppeared;
    private float appearanceRate;
    private float appearanceTime;
    private Coroutine appearanceRoutine;

    public void SetUp(float appearanceRate, float appearanceTime)
    {
        isCatched = false;
        isAppeared = false;
        this.appearanceRate = appearanceRate;
        this.appearanceTime = appearanceTime;
        isAppeared = false;
        if (appearanceRoutine != null)
        {
            StopCoroutine(appearanceRoutine);
        }

        if (gameObject.activeInHierarchy)
        {
            appearanceRoutine = StartCoroutine(StartApperanceRoutine());
        }
    }

    private IEnumerator StartApperanceRoutine()
    {
        while (!isCatched)
        {
            yield return new WaitForSeconds(appearanceRate);
            animator.Play("Worm_Appearance", 0, 0);
            isAppeared = true;
            yield return new WaitForSeconds(appearanceTime);
            isAppeared = false;
            animator.Play("Worm_Disappearance", 0, 0);
        }
    }
    public bool TryPullOut()
    {
        if (isAppeared)
        {
            isCatched = true;
            StopAllCoroutines();
            animator.Play("Worm_PullOut", 0, 0);
            return true;
        }

        return false;
    }

    void OnEnable()
    {
        if (appearanceRoutine != null)
        {
            StopAllCoroutines();
        }
        appearanceRoutine = StartCoroutine(StartApperanceRoutine());
    }

    void OnDisable()
    {
        if (appearanceRoutine != null)
        {
            StopCoroutine(appearanceRoutine);
        }
    }

}
