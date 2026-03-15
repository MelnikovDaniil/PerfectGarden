using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormPincet : MonoBehaviour
{
    public float grabTime = 0.2f;
    public float backToPositionTime = 1f;
    public float successfulGrabDelay = 1f;
    public float successfulGrabMagnitude = 1f;
    public AudioClip pincetClip;
    public AudioClip warmExtractionClip;
    public List<AudioClip> warmRabbingClips;
    public List<AudioClip> pincetFailedClips;

    private Vector3 startPosition;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public IEnumerator GrabRoutine(GameObject obj, bool seccessfuly)
    {
        startPosition = transform.position;
        animator.SetTrigger("grab");
        yield return MovementHelper.MoveObjectToTargetRoutine(transform, obj.transform.position, grabTime, false);
        SoundManager.PlaySound(pincetClip);

        if (seccessfuly)
        {
            SoundManager.PlaySound(warmRabbingClips.GetRandom());
            yield return MovementHelper.ShakeRoutine(transform, successfulGrabMagnitude, successfulGrabDelay);
            obj.transform.parent = transform;
            SoundManager.PlaySound(warmExtractionClip);
            yield return MovementHelper.MoveObjectToTargetRoutine(transform, startPosition + Vector3.up*2, grabTime, true);
            obj.transform.parent = null;
            Destroy(obj, 1);
        }
        else
        {
            SoundManager.PlaySound(pincetFailedClips.GetRandom());
        }

        animator.SetTrigger("release");
        yield return MovementHelper.MoveObjectToTargetRoutine(transform, startPosition, grabTime, true);
    }
}
