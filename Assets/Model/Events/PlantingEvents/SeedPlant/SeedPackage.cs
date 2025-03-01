using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SeedPackage : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _seedPackageRenderer;
    [SerializeField] private Seed _seed;
    [SerializeField] private ParticleSystem _unpackParticles;

    public bool interactable;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _seedPackageRenderer.enabled = true;
        _collider.enabled = true;
    }

    public void SetUp(Sprite packageSprite, Sprite seedSprite)
    {
        _seedPackageRenderer.sprite = packageSprite ?? _seedPackageRenderer.sprite;
        _seed.seedRenderer.sprite = seedSprite ?? _seed.seedRenderer.sprite;
        _seed.transform.localPosition = Vector3.zero;
    }

    private void OnMouseDown()
    {
        if (interactable)
        {
            _unpackParticles.Play();
            _seedPackageRenderer.enabled = false;
            _seed.Drop();
            _collider.enabled = false;
            interactable = false;
        }
    }
}
