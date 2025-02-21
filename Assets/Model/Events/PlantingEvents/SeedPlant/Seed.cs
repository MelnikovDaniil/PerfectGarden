using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider), typeof(SpriteRenderer))]
public class Seed : MonoBehaviour
{
    public SpriteRenderer seedRenderer;
    private Rigidbody _rigidbody;


    private void Awake()
    {
        transform.localPosition = Vector3.zero;
        seedRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody>();

        gameObject.SetActive(false);
    }

    public void Drop()
    {
        gameObject.SetActive(true);
        _rigidbody.isKinematic = false;
    }
}
