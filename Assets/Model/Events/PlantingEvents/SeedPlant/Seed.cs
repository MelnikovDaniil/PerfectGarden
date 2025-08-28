using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider), typeof(SpriteRenderer))]
public class Seed : MonoBehaviour
{
    public SpriteRenderer seedRenderer;
    private Rigidbody _rigidbody;


    private void Awake()
    {
        seedRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody>();

    }

    public void Setup()
    {
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(true);
        _rigidbody.isKinematic = true;
    }

    public void Drop()
    {
        gameObject.SetActive(true);
        _rigidbody.isKinematic = false;
    }
}
