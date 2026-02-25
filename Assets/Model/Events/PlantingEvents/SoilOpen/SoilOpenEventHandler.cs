using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoilOpenPlantingEvent : PlantEventHandler
{
    public override PlantingEvent EventName => PlantingEvent.SoilOpen;

    public RectTransform sliderCanvas;
    public Slider slider;
    public Animator soilOpenAnimator;
    public AudioClip soilAppearanceClip;

    private bool isOpened;
    private float progress;

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderMove);
    }

    private void Start()
    {
        Clear();
        soilOpenAnimator.speed = 0;
    }

    private void Update()
    {
        if (Status == HandlingStatus.Started)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (progress != 1)
                {
                    progress = 0;
                    slider.value = 0;
                }
                else
                {
                    isOpened = true;
                }
            }
        }
    }

    protected override async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        isOpened = false;
        progress = 0;
        slider.value = 0;
        sliderCanvas.gameObject.SetActive(true);
        soilOpenAnimator.gameObject.SetActive(true);
        soilOpenAnimator.SetFloat("progress", 0);
        SoundManager.PlaySound(soilAppearanceClip);
        await Task.Yield();
        await base.PrepareHandlingAsync();
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        _ = TutorialManager.Instance.SetSwipeAsync(slider.handleRect.gameObject, Vector2.right, 1f, false, token);
        while (!isOpened)
        {
            await Task.Yield();
        }

        await Task.CompletedTask;
    }

    public override void Clear()
    {
        isOpened = false;
        slider.value = 0;
        sliderCanvas.gameObject.SetActive(false);
        soilOpenAnimator.gameObject.SetActive(false);
        soilOpenAnimator.SetFloat("progress", 0);
    }

    public void OnSliderMove(float value)
    {
        if (value > progress)
        {
            progress = value;
        }
        else
        {
            slider.value = progress;
        }

        soilOpenAnimator.SetFloat("progress", progress);
    }
}
