using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SeedPackage : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _seedPackageRenderer;
    [SerializeField] private Seed _seed;
    [SerializeField] private ParticleSystem _unpackParticles;

    public bool interactable;
    private Sprite _defaultPackageSprite;
    private Sprite _defaultSeedSprite;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _defaultPackageSprite = _seedPackageRenderer.sprite;
        _defaultSeedSprite = _seed.seedRenderer.sprite;
    }

    public void SetUp(Sprite packageSprite, Sprite seedSprite)
    {
        _seedPackageRenderer.enabled = true;
        _collider.enabled = true;
        if (packageSprite != null)
        {
            _seedPackageRenderer.sprite = packageSprite;
        }
        else
        {
            _seedPackageRenderer.sprite = _defaultPackageSprite;
        }
        _seed.Setup();
        if (seedSprite != null)
        {
            _seed.seedRenderer.sprite = seedSprite;
        }
        else
        {
            _seed.seedRenderer.sprite = _defaultSeedSprite;
        }

        _seed.transform.localPosition = Vector3.zero;
    }

    private void OnMouseDown()
    {
        if (interactable)
        {
            _unpackParticles.Play();
            _seedPackageRenderer.enabled = false;
            _seed.Drop();
            interactable = false;
        }
    }
}
