using UnityEngine;

public class Weed : MonoBehaviour
{
    public SpriteRenderer renderer;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PullOutAndDestroy()
    {
        _animator.Play("Weed_PullOut", 0, 0);
        Destroy(gameObject, 1);
    }
}
